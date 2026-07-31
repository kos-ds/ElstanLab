using DocumentFormat.OpenXml.Drawing;
using ElstanLab.Models;
using ElstanLab.Services;
using ElstanLab.Themes;
using ElstanLab.UI;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using FontStyle = System.Drawing.FontStyle;
using Label = System.Windows.Forms.Label;
using Timer = System.Windows.Forms.Timer;

namespace ElstanLab.Pages.AVPage
{
    public class AVPageBuilder
    {
        // Данные текущей записи (временно, пока идёт Recording)
        private readonly List<double> recTimes = new List<double>();
        private readonly List<double> recUa = new List<double>();
        private readonly List<double> recUb = new List<double>();
        private readonly List<double> recUc = new List<double>();
        private readonly List<double> recIa = new List<double>();
        private readonly List<double> recIb = new List<double>();
        private readonly List<double> recIc = new List<double>();
        private Label lblStatus;   // результат испытания
        private MeterPacket lastPacket;
        private double offsetU = 10;          // стартовое значение
        private double offsetI = 2;
        private bool offsetsInitialized = false;

        private const double MinGapU = 5.0;
        private const double MinGapI = .0;
        private const double Factor = 1.3;    // насколько больше разброса фаз брать

        // ===== UI =====
        private Label lblUa, lblUb, lblUc;
        private Label lblIa, lblIb, lblIc;
        private Label lblTimer;
        private Button btnStart, btnStop, btnReset;

        private FormsPlot plotU;
        private FormsPlot plotI;

        // ===== DataLoggers (3 фазы напряжения + 3 фазы тока) =====
        private DataLogger logUa, logUb, logUc;
        private DataLogger logIa, logIb, logIc;

        // ===== Состояние =====
        private enum ChartMode { Idle, Recording, Stopped }
        private ChartMode mode = ChartMode.Idle;

        private readonly Stopwatch sw = new Stopwatch();   // общий таймер времени
        private double t0 = 0;                             // момент нажатия ПУСК (в секундах от sw)
        private const double HalfWindow = 90.0;            // ±90 сек
        private const double RecordDuration = 60.0;        // запись 60 сек

        private Timer uiTimer;                            // обновление таймера и Refresh

        private readonly TabPage page;

        public AVPageBuilder(TabPage tabPage)
        {
            page = tabPage;
            Build();
            DataModelService.DataUpdated += OnDataUpdated;

            sw.Start();
            uiTimer = new Timer();
            uiTimer.Interval = 100; // 10 Гц достаточно
            uiTimer.Tick += UiTimer_Tick;
            uiTimer.Start();
        }

        //------------------------------------------------
        // UI
        //------------------------------------------------
        private Label CreateValueLabel()
        {
            return new Label()
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Font = new Font("Consolas", 12, FontStyle.Bold),
                Text = "---"
            };
        }

        private void Build()
        {
            page.Controls.Clear();

            TableLayoutPanel main = new TableLayoutPanel();
            main.Dock = DockStyle.Fill;
            main.ColumnCount = 1;
            main.RowCount = 4;
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
            page.Controls.Add(main);

            // Top
            main.Controls.Add(BuildTopPanel(), 0, 0);

            // Charts
            plotU = BuildChart("Напряжение");
            plotI = BuildChart("Ток");
            main.Controls.Add(plotU, 0, 1);
            main.Controls.Add(plotI, 0, 2);

            InitLoggers();

            // Buttons
            main.Controls.Add(BuildBottomPanel(), 0, 3);
        }

        private TableLayoutPanel BuildTopPanel()
        {
            TableLayoutPanel top = new TableLayoutPanel();
            top.Dock = DockStyle.Fill;
            top.ColumnCount = 2;
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            top.Controls.Add(BuildVoltageGroup(), 0, 0);
            top.Controls.Add(BuildCurrentGroup(), 1, 0);
            return top;
        }

        private GroupBox BuildVoltageGroup()
        {
            GroupBox gb = new GroupBox();
            gb.Text = "Напряжение";
            gb.Dock = DockStyle.Fill;
            gb.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            TableLayoutPanel t = new TableLayoutPanel();
            t.Dock = DockStyle.Fill;
            t.ColumnCount = 3;
            t.RowCount = 2;
            t.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            for (int i = 0; i < 3; i++)
                t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));

            t.Controls.Add(new Label() { Text = "Ua", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 0, 0);
            t.Controls.Add(new Label() { Text = "Ub", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 1, 0);
            t.Controls.Add(new Label() { Text = "Uc", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 2, 0);

            lblUa = CreateValueLabel();
            lblUb = CreateValueLabel();
            lblUc = CreateValueLabel();
            t.Controls.Add(lblUa, 0, 1);
            t.Controls.Add(lblUb, 1, 1);
            t.Controls.Add(lblUc, 2, 1);

            gb.Controls.Add(t);
            return gb;
        }

        private GroupBox BuildCurrentGroup()
        {
            GroupBox gb = new GroupBox();
            gb.Text = "Ток";
            gb.Dock = DockStyle.Fill;
            gb.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            TableLayoutPanel t = new TableLayoutPanel();
            t.Dock = DockStyle.Fill;
            t.ColumnCount = 3;
            t.RowCount = 2;
            t.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            for (int i = 0; i < 3; i++)
                t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));

            t.Controls.Add(new Label() { Text = "Ia", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 0, 0);
            t.Controls.Add(new Label() { Text = "Ib", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 1, 0);
            t.Controls.Add(new Label() { Text = "Ic", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 2, 0);

            lblIa = CreateValueLabel();
            lblIb = CreateValueLabel();
            lblIc = CreateValueLabel();
            t.Controls.Add(lblIa, 0, 1);
            t.Controls.Add(lblIb, 1, 1);
            t.Controls.Add(lblIc, 2, 1);

            gb.Controls.Add(t);
            return gb;
        }

        private void RecalculateOffsets(MeterPacket p)
        {
            // ----- Напряжение -----
            double uMax = Math.Max(p.UL1_AB, Math.Max(p.UL1_BC, p.UL1_CA));
            double uMin = Math.Min(p.UL1_AB, Math.Min(p.UL1_BC, p.UL1_CA));
            double uRange = uMax - uMin;

            double neededU = Math.Max(uRange * Factor, MinGapU);
            if (neededU > offsetU)
                offsetU = neededU;

            // ----- Ток -----
            double iMax = Math.Max(p.I1_A, Math.Max(p.I1_B, p.I1_C));
            double iMin = Math.Min(p.I1_A, Math.Min(p.I1_B, p.I1_C));
            double iRange = iMax - iMin;

            double neededI = Math.Max(iRange * Factor, MinGapI);
            if (neededI > offsetI)
                offsetI = neededI;

            // Обновляем легенду
            logUa.LegendText = "Ua (+" + offsetU.ToString("F0") + ")";
            logUb.LegendText = "Ub";
            logUc.LegendText = "Uc (-" + offsetU.ToString("F0") + ")";

            logIa.LegendText = "Ia (+" + offsetI.ToString("F1") + ")";
            logIb.LegendText = "Ib";
            logIc.LegendText = "Ic (-" + offsetI.ToString("F1") + ")";
        }
        private FormsPlot BuildChart(string title)
        {
            FormsPlot fp = new FormsPlot();
            fp.Dock = DockStyle.Fill;
            fp.Plot.Title(title);
            fp.Plot.Axes.Bottom.Label.Text = "Время, сек";
            fp.Plot.ShowGrid();
            // отключаем интерактивность, чтобы пользователь не сдвигал оси во время записи
            fp.UserInputProcessor.Disable();
            return fp;
        }

        private void InitLoggers()
        {
            // ----- Напряжение -----
            logUa = plotU.Plot.Add.DataLogger();
            logUb = plotU.Plot.Add.DataLogger();
            logUc = plotU.Plot.Add.DataLogger();

            logUa.LegendText = "Ua (+" + offsetU + ")";
            logUb.LegendText = "Ub ";
            logUc.LegendText = "Uc (-" + offsetU + ")";

            logUa.Color = Colors.Red;
            logUb.Color = Colors.LimeGreen;
            logUc.Color = Colors.DodgerBlue;

            logUa.ManageAxisLimits = false;
            logUb.ManageAxisLimits = false;
            logUc.ManageAxisLimits = false;

            // ----- Ток -----
            logIa = plotI.Plot.Add.DataLogger();
            logIb = plotI.Plot.Add.DataLogger();
            logIc = plotI.Plot.Add.DataLogger();

            logIa.LegendText = "Ia (+" + offsetI + ")";
            logIb.LegendText = "Ib ";
            logIc.LegendText = "Ic (-" + offsetI + ")";

            logIa.Color = Colors.Red;
            logIb.Color = Colors.LimeGreen;
            logIc.Color = Colors.DodgerBlue;

            logIa.ManageAxisLimits = false;
            logIb.ManageAxisLimits = false;
            logIc.ManageAxisLimits = false;

            plotU.Plot.ShowLegend(Alignment.UpperRight);
            plotI.Plot.ShowLegend(Alignment.UpperRight);

            // Вертикальная линия центра
            plotU.Plot.Add.VerticalLine(0, color: Colors.Gray.WithAlpha(0.6f), width: 1.5f, pattern: LinePattern.Dashed);
            plotI.Plot.Add.VerticalLine(0, color: Colors.Gray.WithAlpha(0.6f), width: 1.5f, pattern: LinePattern.Dashed);

            SetIdleLimits();
        }
        private void InitLoggers1()
        {
            // Напряжение
            logUa = plotU.Plot.Add.DataLogger();
            logUb = plotU.Plot.Add.DataLogger();
            logUc = plotU.Plot.Add.DataLogger();

            logUa.LegendText = "Ua";
            logUb.LegendText = "Ub";
            logUc.LegendText = "Uc";

            logUa.Color = Colors.Red;
            logUb.Color = Colors.Green;
            logUc.Color = Colors.Blue;

            logUa.ManageAxisLimits = false;
            logUb.ManageAxisLimits = false;
            logUc.ManageAxisLimits = false;

            // Начальный диапазон Y (чтобы не было пустоты)
            plotU.Plot.Axes.SetLimitsY(0, 600);   // подстрой под свои напряжения
            plotI.Plot.Axes.SetLimitsY(0, 50);     // подстрой под свои токи

            // Ток
            logIa = plotI.Plot.Add.DataLogger();
            logIb = plotI.Plot.Add.DataLogger();
            logIc = plotI.Plot.Add.DataLogger();

            logIa.LegendText = "Ia";
            logIb.LegendText = "Ib";
            logIc.LegendText = "Ic";

            logIa.Color = Colors.Red;
            logIb.Color = Colors.Green;
            logIc.Color = Colors.Blue;

            logIa.ManageAxisLimits = false;
            logIb.ManageAxisLimits = false;
            logIc.ManageAxisLimits = false;

            plotU.Plot.ShowLegend();
            plotI.Plot.ShowLegend();

            // Вертикальная линия центра (будет двигаться в Idle, фиксироваться при Recording)
            plotU.Plot.Add.VerticalLine(0, color: Colors.Gray.WithAlpha(0.6f), width: 1.5f, pattern: LinePattern.Dashed);
            plotI.Plot.Add.VerticalLine(0, color: Colors.Gray.WithAlpha(0.6f), width: 1.5f, pattern: LinePattern.Dashed);

            SetIdleLimits();
        }
        private void AutoScaleY(bool allowShrink = false)
        {
            // Сначала обычный автомасштаб
            plotU.Plot.Axes.AutoScaleY();
            plotI.Plot.Axes.AutoScaleY();

            var limU = plotU.Plot.Axes.GetLimits();
            var limI = plotI.Plot.Axes.GetLimits();

            // Добавляем небольшой запас (padding)
            double padU = Math.Max((limU.Top - limU.Bottom) * 0.08, 5);
            double padI = Math.Max((limI.Top - limI.Bottom) * 0.08, 1);

            if (allowShrink)
            {
                // Можно и расширять, и сжимать (для Idle)
                plotU.Plot.Axes.SetLimitsY(limU.Bottom - padU, limU.Top + padU);
                plotI.Plot.Axes.SetLimitsY(limI.Bottom - padI, limI.Top + padI);
            }
            else
            {
                // Только расширяем (для Recording) — никогда не сжимаем
                var curU = plotU.Plot.Axes.GetLimits();
                var curI = plotI.Plot.Axes.GetLimits();

                double newBottomU = Math.Min(curU.Bottom, limU.Bottom - padU);
                double newTopU = Math.Max(curU.Top, limU.Top + padU);
                plotU.Plot.Axes.SetLimitsY(newBottomU, newTopU);

                double newBottomI = Math.Min(curI.Bottom, limI.Bottom - padI);
                double newTopI = Math.Max(curI.Top, limI.Top + padI);
                plotI.Plot.Axes.SetLimitsY(newBottomI, newTopI);
            }
        }
        private void AutoScaleY0()
        {
            // Автомасштаб только по Y, X не трогаем
            plotU.Plot.Axes.AutoScaleY();
            plotI.Plot.Axes.AutoScaleY();

            // Небольшой запас сверху/снизу (5%)
            var limU = plotU.Plot.Axes.GetLimits();
            double padU = (limU.Top - limU.Bottom) * 0.05;
            if (padU < 1) padU = 1;
            plotU.Plot.Axes.SetLimitsY(limU.Bottom - padU, limU.Top + padU);

            var limI = plotI.Plot.Axes.GetLimits();
            double padI = (limI.Top - limI.Bottom) * 0.05;
            if (padI < 0.5) padI = 0.5;
            plotI.Plot.Axes.SetLimitsY(limI.Bottom - padI, limI.Top + padI);
        }

        private TableLayoutPanel BuildBottomPanel1()
        {
            TableLayoutPanel p = new TableLayoutPanel();
            p.Dock = DockStyle.Fill;
            p.ColumnCount = 5;
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            btnStart = new Button() { Text = "ПУСК", Dock = DockStyle.Fill };
            btnStart.Click += BtnStart_Click;

            btnStop = new Button() { Text = "СТОП", Dock = DockStyle.Fill };
            btnStop.Click += BtnStop_Click;

            btnReset = new Button() { Text = "RESET", Dock = DockStyle.Fill };
            btnReset.Click += BtnReset_Click;

            lblTimer = CreateValueLabel();
            lblTimer.Text = "00:00";

            p.Controls.Add(btnStart, 0, 0);
            p.Controls.Add(btnStop, 1, 0);
            p.Controls.Add(btnReset, 2, 0);
            p.Controls.Add(lblTimer, 4, 0);

            return p;
        }

        private TableLayoutPanel BuildBottomPanel()
        {
            TableLayoutPanel p = new TableLayoutPanel();
            p.Dock = DockStyle.Fill;
            p.ColumnCount = 6;
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100)); // таймер
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160)); // статус
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            btnStart = new Button() { Text = "ПУСК", Dock = DockStyle.Fill };
            btnStart.Click += BtnStart_Click;

            btnStop = new Button() { Text = "СТОП", Dock = DockStyle.Fill };
            btnStop.Click += BtnStop_Click;

            btnReset = new Button() { Text = "RESET", Dock = DockStyle.Fill };
            btnReset.Click += BtnReset_Click;

            lblTimer = CreateValueLabel();
            lblTimer.Text = "00:00";
            lblTimer.Font = new Font("Consolas", 14, FontStyle.Bold);

            lblStatus = CreateValueLabel();
            lblStatus.Text = "";
            lblStatus.Font = new Font("Segoe UI", 11, FontStyle.Bold);

            p.Controls.Add(btnStart, 0, 0);
            p.Controls.Add(btnStop, 1, 0);
            p.Controls.Add(btnReset, 2, 0);
            p.Controls.Add(lblTimer, 3, 0);
            p.Controls.Add(lblStatus, 4, 0);

            return p;
        }
        //------------------------------------------------
        // Логика режимов
        //------------------------------------------------        
        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (mode == ChartMode.Recording) return;

            // Очищаем предыдущую запись
            recTimes.Clear();
            recUa.Clear(); recUb.Clear(); recUc.Clear();
            recIa.Clear(); recIb.Clear(); recIc.Clear();

            if (lastPacket != null)
                RecalculateOffsets(lastPacket);

            AutoScaleY(allowShrink: true);

            t0 = sw.Elapsed.TotalSeconds;
            mode = ChartMode.Recording;

            SetRecordingLimits();

            btnStart.Enabled = false;
            btnStop.Enabled = true;
            lblStatus.Text = "Идёт запись...";
            lblStatus.ForeColor = Color.DarkOrange;
        }

        private void AnalyzeRecording()
        {
            if (recTimes.Count < 10)
            {
                SetStatus(false, "Мало данных");
                return;
            }

            var snap = LabStorage.CurrentAV;
            snap.Time = DateTime.Now;

            // Копируем сырые данные
            snap.Times.Clear();
            snap.Ua.Clear(); snap.Ub.Clear(); snap.Uc.Clear();
            snap.Ia.Clear(); snap.Ib.Clear(); snap.Ic.Clear();

            snap.Times.AddRange(recTimes);
            snap.Ua.AddRange(recUa);
            snap.Ub.AddRange(recUb);
            snap.Uc.AddRange(recUc);
            snap.Ia.AddRange(recIa);
            snap.Ib.AddRange(recIb);
            snap.Ic.AddRange(recIc);

            // Расчёт средних и отклонений
            snap.UaMean = Mean(recUa);
            snap.UbMean = Mean(recUb);
            snap.UcMean = Mean(recUc);
            snap.Uavg = (snap.UaMean + snap.UbMean + snap.UcMean) / 3.0;

            snap.IaMean = Mean(recIa);
            snap.IbMean = Mean(recIb);
            snap.IcMean = Mean(recIc);
            snap.Iavg = (snap.IaMean + snap.IbMean + snap.IcMean) / 3.0;

            snap.UaDev = MaxRelativeDeviation(recUa, snap.UaMean);
            snap.UbDev = MaxRelativeDeviation(recUb, snap.UbMean);
            snap.UcDev = MaxRelativeDeviation(recUc, snap.UcMean);
            snap.MaxUDev = Math.Max(snap.UaDev, Math.Max(snap.UbDev, snap.UcDev));

            snap.IaDev = MaxRelativeDeviation(recIa, snap.IaMean);
            snap.IbDev = MaxRelativeDeviation(recIb, snap.IbMean);
            snap.IcDev = MaxRelativeDeviation(recIc, snap.IcMean);
            snap.MaxIDev = Math.Max(snap.IaDev, Math.Max(snap.IbDev, snap.IcDev));

            // Критерий 20 %
            const double limit = 20.0;   // %
            bool okU = snap.MaxUDev <= limit;
            bool okI = snap.MaxIDev <= limit;

            snap.Passed = okU && okI;

            // Можно сразу добавить в историю (как у вас сделано на других страницах)
            // LabStorage.AVSnapshots.Add(CloneSnapshot(snap)); // если нужно

            SetStatus(snap.Passed, snap.Passed ? "ПРОЙДЕНО" : "НЕ ПРОЙДЕНО");
        }

        private double Mean(List<double> values)
        {
            if (values.Count == 0) return 0;
            double sum = 0;
            foreach (var v in values) sum += v;
            return sum / values.Count;
        }

        /// <summary>
        /// Максимальное относительное отклонение от среднего в процентах
        /// </summary>
        private double MaxRelativeDeviation(List<double> values, double mean)
        {
            if (values.Count == 0 || Math.Abs(mean) < 1e-9)
                return 999.0;   // заведомо плохо

            double maxDev = 0;
            foreach (var v in values)
            {
                double dev = Math.Abs(v - mean);
                if (dev > maxDev) maxDev = dev;
            }
            return (maxDev / Math.Abs(mean)) * 100.0;
        }

        private void AnalyzeRecording0()
        {
            if (recTimes.Count < 10)   // слишком мало точек
            {
                SetStatus(false, "Мало данных");
                return;
            }

            bool okU = CheckDeviation(recUa, 0.20);
            bool okI = CheckDeviation(recIa, 0.20) &&
                       CheckDeviation(recIb, 0.20) &&
                       CheckDeviation(recIc, 0.20);

            // Можно проверять и напряжения пофазно:
            // bool okU = CheckDeviation(recUa, 0.20) &&
            //            CheckDeviation(recUb, 0.20) &&
            //            CheckDeviation(recUc, 0.20);

            bool passed = okU && okI;

            SetStatus(passed, passed ? "ПРОЙДЕНО" : "НЕ ПРОЙДЕНО");
        }

        /// <summary>
        /// Проверяет, что максимальное отклонение от среднего ≤ maxRel (0.20 = 20%)
        /// </summary>
        private bool CheckDeviation(List<double> values, double maxRel)
        {
            if (values.Count == 0) return false;

            double sum = 0;
            foreach (var v in values) sum += v;
            double mean = sum / values.Count;

            if (Math.Abs(mean) < 1e-6)   // почти ноль — считаем провалом
                return false;

            double maxDev = 0;
            foreach (var v in values)
            {
                double dev = Math.Abs(v - mean);
                if (dev > maxDev) maxDev = dev;
            }

            return (maxDev / Math.Abs(mean)) <= maxRel;
        }
        private void SetStatus(bool passed, string text)
        {
            lblStatus.Text = text;
            lblStatus.ForeColor = passed ? Color.LimeGreen : Color.OrangeRed;
        }

        private void AnalyzeRecording1()
        {
            if (recTimes.Count < 10)
            {
                SetStatus(false, "Мало данных");
                return;
            }

            var snap = LabStorage.CurrentAV;
            snap.Time = DateTime.Now;

            // Копируем сырые данные
            snap.Times.Clear();
            snap.Ua.Clear(); snap.Ub.Clear(); snap.Uc.Clear();
            snap.Ia.Clear(); snap.Ib.Clear(); snap.Ic.Clear();

            snap.Times.AddRange(recTimes);
            snap.Ua.AddRange(recUa);
            snap.Ub.AddRange(recUb);
            snap.Uc.AddRange(recUc);
            snap.Ia.AddRange(recIa);
            snap.Ib.AddRange(recIb);
            snap.Ic.AddRange(recIc);

            // Расчёт средних и отклонений
            snap.UaMean = Mean(recUa);
            snap.UbMean = Mean(recUb);
            snap.UcMean = Mean(recUc);
            snap.Uavg = (snap.UaMean + snap.UbMean + snap.UcMean) / 3.0;

            snap.IaMean = Mean(recIa);
            snap.IbMean = Mean(recIb);
            snap.IcMean = Mean(recIc);
            snap.Iavg = (snap.IaMean + snap.IbMean + snap.IcMean) / 3.0;

            snap.UaDev = MaxRelativeDeviation(recUa, snap.UaMean);
            snap.UbDev = MaxRelativeDeviation(recUb, snap.UbMean);
            snap.UcDev = MaxRelativeDeviation(recUc, snap.UcMean);
            snap.MaxUDev = Math.Max(snap.UaDev, Math.Max(snap.UbDev, snap.UcDev));

            snap.IaDev = MaxRelativeDeviation(recIa, snap.IaMean);
            snap.IbDev = MaxRelativeDeviation(recIb, snap.IbMean);
            snap.IcDev = MaxRelativeDeviation(recIc, snap.IcMean);
            snap.MaxIDev = Math.Max(snap.IaDev, Math.Max(snap.IbDev, snap.IcDev));

            // Критерий 20 %
            const double limit = 20.0;   // %
            bool okU = snap.MaxUDev <= limit;
            bool okI = snap.MaxIDev <= limit;

            snap.Passed = okU && okI;

            // Можно сразу добавить в историю (как у вас сделано на других страницах)
            // LabStorage.AVSnapshots.Add(CloneSnapshot(snap)); // если нужно

            SetStatus(snap.Passed, snap.Passed ? "ПРОЙДЕНО" : "НЕ ПРОЙДЕНО");
        }

       

       

        private void BtnStop_Click1(object sender, EventArgs e)
        {
            if (mode != ChartMode.Recording) return;

            mode = ChartMode.Stopped;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            if (mode != ChartMode.Recording) return;

            mode = ChartMode.Stopped;
            btnStart.Enabled = true;
            btnStop.Enabled = false;

            AnalyzeRecording();   // ← анализ
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            mode = ChartMode.Idle;

            // Очистка данных
            logUa.Data.Coordinates.Clear();
            logUb.Data.Coordinates.Clear();
            logUc.Data.Coordinates.Clear();
            logIa.Data.Coordinates.Clear();
            logIb.Data.Coordinates.Clear();
            logIc.Data.Coordinates.Clear();
            // в BtnReset_Click добавьте:
            recTimes.Clear();
            recUa.Clear(); recUb.Clear(); recUc.Clear();
            recIa.Clear(); recIb.Clear(); recIc.Clear();

            lblStatus.Text = "";
            // LabStorage.CurrentAV можно не трогать — он останется последним результатом
            // Сброс таймера времени (чтобы не накапливать огромные числа)
            sw.Restart();
            t0 = 0;

            // Сбросим offset в разумные значения по умолчанию
            offsetU = 10;
            offsetI = 5;
            offsetsInitialized = false;

            SetIdleLimits();

            btnStart.Enabled = true;
            btnStop.Enabled = true;
            lblTimer.Text = "00:00";

            plotU.Refresh();
            plotI.Refresh();
        }

        private void SetIdleLimits()
        {
            double now = sw.Elapsed.TotalSeconds;
            plotU.Plot.Axes.SetLimitsX(now - HalfWindow, now + HalfWindow);
            plotI.Plot.Axes.SetLimitsX(now - HalfWindow, now + HalfWindow);

            // Вертикальная линия в центре
            UpdateCenterLine(now);
        }

        private void SetRecordingLimits()
        {
            plotU.Plot.Axes.SetLimitsX(t0 - HalfWindow, t0 + HalfWindow);
            plotI.Plot.Axes.SetLimitsX(t0 - HalfWindow, t0 + HalfWindow);
            UpdateCenterLine(t0);
        }

        private void UpdateCenterLine(double x)
        {
            // Удаляем старые линии и добавляем новую (простой способ)
            plotU.Plot.Remove<VerticalLine>();
            plotI.Plot.Remove<VerticalLine>();

            plotU.Plot.Add.VerticalLine(x, color: Colors.Gray.WithAlpha(0.7f), width: 1.5f, pattern: LinePattern.Dashed);
            plotI.Plot.Add.VerticalLine(x, color: Colors.Gray.WithAlpha(0.7f), width: 1.5f, pattern: LinePattern.Dashed);
        }

        //------------------------------------------------
        // Таймер UI + авто-стоп
        //------------------------------------------------
        private void UiTimer_Tick(object sender, EventArgs e)
        {
            if (mode == ChartMode.Idle)
            {
                SetIdleLimits();
                //AutoScaleY();             // ← Y автомасштаб
                // Автомасштаб по Y (только по видимым данным)
                //plotU.Plot.Axes.AutoScaleY();
                //plotI.Plot.Axes.AutoScaleY();
                AutoScaleY(allowShrink: true);   // ← полный автомасштаб
                plotU.Refresh();
                plotI.Refresh();
                lblTimer.Text = "00:00";
            }
            else if (mode == ChartMode.Recording)
            {
                double elapsed = sw.Elapsed.TotalSeconds - t0;

                // Авто-стоп через 60 секунд
                if (elapsed >= RecordDuration)
                {
                    mode = ChartMode.Stopped;
                    btnStart.Enabled = true;
                    btnStop.Enabled = false;
                    AnalyzeRecording();          // ← анализ после авто-стопа
                }

                int sec = (int)elapsed;
                lblTimer.Text = string.Format("{0:00}:{1:00}", sec / 60, sec % 60);

                AutoScaleY(allowShrink: false);
                plotU.Refresh();
                plotI.Refresh();
            }
            // Stopped — ничего не делаем, график заморожен
        }

        //------------------------------------------------
        // Приход данных
        //------------------------------------------------
        private void OnDataUpdated(MeterPacket p)
        {
            if (page.InvokeRequired)
            {
                page.BeginInvoke(new Action(() => OnDataUpdated(p)));
                return;
            }

            if (((TabControl)page.Parent).SelectedTab != page)
                return;
            lastPacket = p;
            // Цифры
            lblUa.Text = p.UL1_AB.ToString("F1");
            lblUb.Text = p.UL1_BC.ToString("F1");
            lblUc.Text = p.UL1_CA.ToString("F1");
            lblIa.Text = p.I1_A.ToString("F2");
            lblIb.Text = p.I1_B.ToString("F2");
            lblIc.Text = p.I1_C.ToString("F2");

            // Графики — добавляем только в Idle и Recording
            if (mode == ChartMode.Stopped)
                return;

            // === Автоматический offset ===
            // Первый раз — всегда
            // Потом — только если сигнал вырос
            if (!offsetsInitialized || mode == ChartMode.Idle || mode == ChartMode.Recording)
            {
                RecalculateOffsets(p);
                offsetsInitialized = true;
            }

            double t = sw.Elapsed.TotalSeconds;


            // В Idle обрезаем старые точки (чтобы не рос бесконечно)
            if (mode == ChartMode.Idle)
            {
                double minKeep = t - HalfWindow - 5; // небольшой запас
                TrimOld(logUa, minKeep);
                TrimOld(logUb, minKeep);
                TrimOld(logUc, minKeep);
                TrimOld(logIa, minKeep);
                TrimOld(logIb, minKeep);
                TrimOld(logIc, minKeep);
            }

            // Сохраняем сырые значения только во время записи
            if (mode == ChartMode.Recording)
            {
                recTimes.Add(t);
                recUa.Add(p.UL1_AB);
                recUb.Add(p.UL1_BC);
                recUc.Add(p.UL1_CA);
                recIa.Add(p.I1_A);
                recIb.Add(p.I1_B);
                recIc.Add(p.I1_C);
            }

            // Напряжение — разносим по вертикали
            logUa.Add(t, p.UL1_AB + offsetU);
            logUb.Add(t, p.UL1_BC);
            logUc.Add(t, p.UL1_CA - offsetU);

            // Ток — разносим по вертикали
            logIa.Add(t, p.I1_A + offsetI);
            logIb.Add(t, p.I1_B);
            logIc.Add(t, p.I1_C - offsetI);
        }

        private void TrimOld(DataLogger logger, double minX)
        {
            var coords = logger.Data.Coordinates;
            int removeCount = 0;
            for (int i = 0; i < coords.Count; i++)
            {
                if (coords[i].X < minX)
                    removeCount++;
                else
                    break;
            }
            if (removeCount > 0)
                coords.RemoveRange(0, removeCount);
        }
    }
}