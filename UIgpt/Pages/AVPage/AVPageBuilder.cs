using ElstanLab.Models;
using ElstanLab.Services;
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
        // ===== Данные текущей записи =====
        private readonly List<double> recTimes = new List<double>();
        private readonly List<double> recUa = new List<double>();
        private readonly List<double> recIa = new List<double>();

        private Label lblStatus;
        private MeterPacket lastPacket;

        // ===== UI =====
        private Label lblUa, lblUaP, lblIa;
        private Label lblRequired, lblPercent, lblWindingInfo;
        private ComboBox cmbWinding;
        private Label lblTimer;
        private Button btnStart, btnStop, btnReset, btnToReport;
        private FormsPlot plotU, plotI;
        private DataGridView gridResults;

        // ===== Логгеры =====
        private DataLogger logUa, logIa;

        // ===== Состояние =====
        private enum ChartMode { Idle, Recording, Stopped }
        private ChartMode mode = ChartMode.Idle;
        private readonly Stopwatch sw = new Stopwatch();
        private double t0 = 0;
        private const double HalfWindow = 90.0;
        private double RecordDuration = LabStorage.labsett.AVTime;
        private Timer uiTimer;
        private readonly TabPage page;

        // Коэффициент делителя (напряжение приложенное = поданное * K)
        private const double DividerK = 500.0;

        public AVPageBuilder(TabPage tabPage)
        {
            page = tabPage;
            Build();
            DataModelService.DataUpdated += OnDataUpdated;
            sw.Start();

            uiTimer = new Timer { Interval = 100 };
            uiTimer.Tick += UiTimer_Tick;
            uiTimer.Start();
        }

        //================================================
        // UI
        //================================================
        private Label CreateValueLabel(float fontSize = 12)
        {
            return new Label
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Font = new Font("Consolas", fontSize, FontStyle.Bold),
                Text = "---"
            };
        }

        private void Build()
        {
            page.Controls.Clear();

            TableLayoutPanel main = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5
            };
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 145));   // верхняя панель
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 42));     // график U
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 42));     // график I
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 160));   // таблица результатов
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));    // кнопки
            page.Controls.Add(main);

            main.Controls.Add(BuildTopPanel(), 0, 0);

            plotU = BuildChart("Напряжение поданное / приложенное");
            plotI = BuildChart("Ток утечки");
            main.Controls.Add(plotU, 0, 1);
            main.Controls.Add(plotI, 0, 2);

            InitLoggers();

            main.Controls.Add(BuildResultsGrid(), 0, 3);
            main.Controls.Add(BuildBottomPanel(), 0, 4);
        }

        private TableLayoutPanel BuildTopPanel()
        {
            TableLayoutPanel top = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));

            top.Controls.Add(BuildMeasurementsGroup(), 0, 0);
            top.Controls.Add(BuildWindingGroup(), 1, 0);
            return top;
        }

        private GroupBox BuildMeasurementsGroup()
        {
            GroupBox gb = new GroupBox
            {
                Text = "Текущие измерения",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            TableLayoutPanel t = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 2,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
            };
            for (int i = 0; i < 3; i++)
                t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));

            t.Controls.Add(new Label { Text = "Поданное, В", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 0, 0);
            t.Controls.Add(new Label { Text = "Приложенное, кВ", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 1, 0);
            t.Controls.Add(new Label { Text = "Ток, мА", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 2, 0);

            lblUa = CreateValueLabel();
            lblUaP = CreateValueLabel();
            lblIa = CreateValueLabel();

            t.Controls.Add(lblUa, 0, 1);
            t.Controls.Add(lblUaP, 1, 1);
            t.Controls.Add(lblIa, 2, 1);

            gb.Controls.Add(t);
            return gb;
        }

        private GroupBox BuildWindingGroup()
        {
            GroupBox gb = new GroupBox
            {
                Text = "Испытываемая обмотка",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            TableLayoutPanel t = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                Padding = new Padding(6)
            };
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));

            t.Controls.Add(new Label { Text = "Обмотка:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);

            cmbWinding = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11)
            };
            cmbWinding.Items.AddRange(new object[] { "ВН", "НН" });
            cmbWinding.SelectedIndex = 0;
            cmbWinding.SelectedIndexChanged += (s, e) => UpdateRequiredVoltage();
            t.Controls.Add(cmbWinding, 1, 0);

            t.Controls.Add(new Label { Text = "Норма Uисп:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
            lblRequired = CreateValueLabel(11);
            lblRequired.ForeColor = Color.DarkBlue;
            t.Controls.Add(lblRequired, 1, 1);

            t.Controls.Add(new Label { Text = "% от нормы:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
            lblPercent = CreateValueLabel(14);
            lblPercent.ForeColor = Color.DarkGreen;
            t.Controls.Add(lblPercent, 1, 2);

            lblWindingInfo = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.DimGray
            };
            t.Controls.Add(lblWindingInfo, 0, 3);
            t.SetColumnSpan(lblWindingInfo, 2);

            gb.Controls.Add(t);
            UpdateRequiredVoltage();
            return gb;
        }

        private DataGridView BuildResultsGrid()
        {
            gridResults = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = true,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                BackgroundColor = Color.White
                
            };

            gridResults.MultiSelect = true;
            gridResults.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            gridResults.Columns.Add("Time", "Время");
            gridResults.Columns.Add("Winding", "Обмотка");
            gridResults.Columns.Add("RequiredU", "Норма, кВ");
            gridResults.Columns.Add("Uavg", "Uср, кВ");
            gridResults.Columns.Add("Percent", "%");
            gridResults.Columns.Add("Iavg", "Iср, мА");
            gridResults.Columns.Add("MaxUDev", "ΔU, %");
            gridResults.Columns.Add("MaxIDev", "ΔI, %");
            gridResults.Columns.Add("Result", "Результат");

            gridResults.Columns["Time"].FillWeight = 90;
            gridResults.Columns["Winding"].FillWeight = 55;
            gridResults.Columns["Result"].FillWeight = 70;

            // Храним полный snapshot в Tag строки
            return gridResults;
        }

        private TableLayoutPanel BuildBottomPanel()
        {
            TableLayoutPanel p = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 7
            };
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130)); // в отчёт
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));  // таймер
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160)); // статус
            p.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            btnStart = new Button { Text = "ПУСК", Dock = DockStyle.Fill };
            btnStart.Click += BtnStart_Click;

            btnStop = new Button { Text = "СТОП", Dock = DockStyle.Fill };
            btnStop.Click += BtnStop_Click;

            btnReset = new Button { Text = "RESET", Dock = DockStyle.Fill };
            btnReset.Click += BtnReset_Click;

            btnToReport = new Button { Text = "В ОТЧЁТ", Dock = DockStyle.Fill };
            btnToReport.Click += BtnToReport_Click;

            lblTimer = CreateValueLabel(14);
            lblTimer.Text = "00:00";

            lblStatus = CreateValueLabel(11);
            lblStatus.Text = "";

            p.Controls.Add(btnStart, 0, 0);
            p.Controls.Add(btnStop, 1, 0);
            p.Controls.Add(btnReset, 2, 0);
            p.Controls.Add(btnToReport, 3, 0);
            p.Controls.Add(lblTimer, 4, 0);
            p.Controls.Add(lblStatus, 5, 0);

            return p;
        }

        private FormsPlot BuildChart(string title)
        {
            FormsPlot fp = new FormsPlot { Dock = DockStyle.Fill };
            fp.Plot.Title(title);
            fp.Plot.Axes.Bottom.Label.Text = "Время, сек";
            fp.Plot.ShowGrid();
            fp.UserInputProcessor.Disable();
            return fp;
        }

        private void InitLoggers()
        {
            logUa = plotU.Plot.Add.DataLogger();
            logUa.LegendText = "U поданное";
            logUa.Color = Colors.Red;
            logUa.ManageAxisLimits = false;

            logIa = plotI.Plot.Add.DataLogger();
            logIa.LegendText = "I";
            logIa.Color = Colors.DodgerBlue;
            logIa.ManageAxisLimits = false;

            plotU.Plot.ShowLegend(Alignment.UpperRight);
            plotI.Plot.ShowLegend(Alignment.UpperRight);

            plotU.Plot.Add.VerticalLine(0, color: Colors.Gray.WithAlpha(0.6f), width: 1.5f, pattern: LinePattern.Dashed);
            plotI.Plot.Add.VerticalLine(0, color: Colors.Gray.WithAlpha(0.6f), width: 1.5f, pattern: LinePattern.Dashed);

            SetIdleLimits();
        }

        //================================================
        // Нормы испытательного напряжения
        //================================================
        private double GetRequiredAppliedVoltage(string winding)
        {
            PassportModel p = LabStorage.Passport;
            double classKV = (winding == "ВН") ? p.HVVoltage : p.LVVoltage;

            // Заводские значения (нормальная изоляция) по ГОСТ / ПУЭ
            if (classKV <= 0.69) return 5.0;
            if (classKV <= 3.0) return 18.0;
            if (classKV <= 6.0) return 25.0;
            if (classKV <= 10.0) return 35.0;
            if (classKV <= 15.0) return 45.0;
            if (classKV <= 20.0) return 55.0;
            if (classKV <= 35.0) return 85.0;
            if (classKV <= 110) return 200.0;
            if (classKV <= 150) return 230.0;
            if (classKV <= 220) return 325.0;

            return 0; // выше — смотреть уровень изоляции
        }

        private void UpdateRequiredVoltage()
        {
            string wind = cmbWinding.SelectedItem?.ToString() ?? "ВН";
            double req = GetRequiredAppliedVoltage(wind);
            lblRequired.Text = req > 0 ? $"{req:F1} кВ" : "—";

            PassportModel p = LabStorage.Passport;
            double classKV = (wind == "ВН") ? p.HVVoltage : p.LVVoltage;
            lblWindingInfo.Text = $"Класс обмотки: {classKV:F2} кВ";
        }

        //================================================
        // Логика кнопок
        //================================================
        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (mode == ChartMode.Recording) return;

            recTimes.Clear();
            recUa.Clear();
            recIa.Clear();

            t0 = sw.Elapsed.TotalSeconds;
            mode = ChartMode.Recording;
            SetRecordingLimits();

            btnStart.Enabled = false;
            btnStop.Enabled = true;
            cmbWinding.Enabled = false;

            lblStatus.Text = "Идёт запись...";
            lblStatus.ForeColor = Color.DarkOrange;
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            if (mode != ChartMode.Recording) return;

            mode = ChartMode.Stopped;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            cmbWinding.Enabled = true;

            AnalyzeAndSave();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            mode = ChartMode.Idle;

            logUa.Data.Coordinates.Clear();
            logIa.Data.Coordinates.Clear();

            recTimes.Clear();
            recUa.Clear();
            recIa.Clear();

            lblStatus.Text = "";
            sw.Restart();
            t0 = 0;

            SetIdleLimits();
            btnStart.Enabled = true;
            btnStop.Enabled = true;
            cmbWinding.Enabled = true;
            lblTimer.Text = "00:00";

            plotU.Refresh();
            plotI.Refresh();
        }

        private void BtnToReport_Click(object sender, EventArgs e)
        {
            if (gridResults.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите одну или несколько строк в таблице результатов.",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Очищаем список для отчёта
            LabStorage.AVSnapshotsForReport.Clear();

            foreach (DataGridViewRow row in gridResults.SelectedRows)
            {
                if (row.Tag is AVSnapshot snap)
                {
                    LabStorage.AVSnapshotsForReport.Add(snap);
                }
            }

            // Для совместимости со старым кодом тоже заполняем CurrentAV (последний выбранный)
            if (LabStorage.AVSnapshotsForReport.Count > 0)
                LabStorage.CurrentAV = LabStorage.AVSnapshotsForReport[0];

            string msg = LabStorage.AVSnapshotsForReport.Count == 1
                ? "Выбрана 1 запись для отчёта."
                : $"Выбрано записей для отчёта: {LabStorage.AVSnapshotsForReport.Count}";

            MessageBox.Show(msg, "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void BtnToReport_Click1(object sender, EventArgs e)
        {
            if (gridResults.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите строку в таблице результатов.", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = gridResults.SelectedRows[0];
            if (row.Tag is AVSnapshot snap)
            {
                LabStorage.CurrentAV = snap; // или Clone, если нужно
                MessageBox.Show($"Запись «{snap.Winding}» загружена в отчёт.", "Готово",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //================================================
        // Анализ и сохранение в таблицу
        //================================================
        private void AnalyzeAndSave()
        {
            if (recTimes.Count < 5)
            {
                SetStatus(false, "Мало данных");
                return;
            }

            string winding = cmbWinding.SelectedItem?.ToString() ?? "ВН";
            double requiredU = GetRequiredAppliedVoltage(winding);

            // Переводим поданное напряжение в приложенное (кВ)
            List<double> appliedUa = new List<double>();
            foreach (var v in recUa)
                appliedUa.Add(v * DividerK / 1000.0); // В → кВ

            double uaMean = Mean(appliedUa);
            double iaMean = Mean(recIa);

            double uaDev = MaxRelativeDeviation(appliedUa, uaMean);
            double iaDev = MaxRelativeDeviation(recIa, iaMean);

            double limit = LabStorage.labsett.AVDeviation;
            bool okU = uaDev <= limit;
            bool okI = iaDev <= limit;
            bool passed = okU && okI && (requiredU <= 0 || Math.Abs(uaMean - requiredU) / requiredU * 100 < 15); // ±15% от нормы как доп. критерий

            // Создаём snapshot
            var snap = new AVSnapshot
            {
                Time = DateTime.Now,
                Winding = winding,
                RequiredU = requiredU,
                UaMean = uaMean,
                Uavg = uaMean,
                IaMean = iaMean,
                Iavg = iaMean,
                UaDev = uaDev,
                MaxUDev = uaDev,
                IaDev = iaDev,
                MaxIDev = iaDev,
                Passed = passed
            };

            snap.Times.Clear();
            snap.Ua.Clear();
            snap.Ia.Clear();
            snap.Times.AddRange(recTimes);
            // сохраняем уже в кВ
            snap.Ua.AddRange(appliedUa);
            snap.Ia.AddRange(recIa);

            // Добавляем в таблицу
            int idx = gridResults.Rows.Add(
                snap.Time.ToString("HH:mm:ss"),
                snap.Winding,
                snap.RequiredU.ToString("F1"),
                snap.Uavg.ToString("F2"),
                (requiredU > 0 ? (snap.Uavg / requiredU * 100).ToString("F1") : "—"),
                snap.Iavg.ToString("F2"),
                snap.MaxUDev.ToString("F2"),
                snap.MaxIDev.ToString("F2"),
                snap.Passed ? "ПРОЙДЕНО" : "НЕ ПРОЙДЕНО"
            );

            gridResults.Rows[idx].Tag = snap;
            gridResults.Rows[idx].DefaultCellStyle.ForeColor = snap.Passed ? Color.DarkGreen : Color.DarkRed;

            // Можно сразу сделать текущим
            LabStorage.CurrentAV = snap;

            SetStatus(snap.Passed, snap.Passed ? "ПРОЙДЕНО" : "НЕ ПРОЙДЕНО");
        }

        private double Mean(List<double> values)
        {
            if (values.Count == 0) return 0;
            double sum = 0;
            foreach (var v in values) sum += v;
            return sum / values.Count;
        }

        private double MaxRelativeDeviation(List<double> values, double mean)
        {
            if (values.Count == 0 || Math.Abs(mean) < 1e-9) return 999.0;
            double maxDev = 0;
            foreach (var v in values)
            {
                double dev = Math.Abs(v - mean);
                if (dev > maxDev) maxDev = dev;
            }
            return (maxDev / Math.Abs(mean)) * 100.0;
        }

        private void SetStatus(bool passed, string text)
        {
            lblStatus.Text = text;
            lblStatus.ForeColor = passed ? Color.LimeGreen : Color.OrangeRed;
        }

        //================================================
        // Таймер UI
        //================================================
        private void UiTimer_Tick(object sender, EventArgs e)
        {
            if (mode == ChartMode.Idle)
            {
                SetIdleLimits();
                AutoScaleY(true);
                plotU.Refresh();
                plotI.Refresh();
                lblTimer.Text = "00:00";
            }
            else if (mode == ChartMode.Recording)
            {
                double elapsed = sw.Elapsed.TotalSeconds - t0;
                if (elapsed >= RecordDuration)
                {
                    mode = ChartMode.Stopped;
                    btnStart.Enabled = true;
                    btnStop.Enabled = false;
                    cmbWinding.Enabled = true;
                    AnalyzeAndSave();
                }

                int sec = (int)elapsed;
                lblTimer.Text = $"{sec / 60:00}:{sec % 60:00}";
                AutoScaleY(false);
                plotU.Refresh();
                plotI.Refresh();
            }
        }

        private void AutoScaleY(bool allowShrink)
        {
            plotU.Plot.Axes.AutoScaleY();
            plotI.Plot.Axes.AutoScaleY();

            var limU = plotU.Plot.Axes.GetLimits();
            var limI = plotI.Plot.Axes.GetLimits();

            double padU = Math.Max((limU.Top - limU.Bottom) * 0.08, 2);
            double padI = Math.Max((limI.Top - limI.Bottom) * 0.08, 0.5);

            if (allowShrink)
            {
                plotU.Plot.Axes.SetLimitsY(limU.Bottom - padU, limU.Top + padU);
                plotI.Plot.Axes.SetLimitsY(limI.Bottom - padI, limI.Top + padI);
            }
            else
            {
                var curU = plotU.Plot.Axes.GetLimits();
                var curI = plotI.Plot.Axes.GetLimits();
                plotU.Plot.Axes.SetLimitsY(Math.Min(curU.Bottom, limU.Bottom - padU), Math.Max(curU.Top, limU.Top + padU));
                plotI.Plot.Axes.SetLimitsY(Math.Min(curI.Bottom, limI.Bottom - padI), Math.Max(curI.Top, limI.Top + padI));
            }
        }

        private void SetIdleLimits()
        {
            double now = sw.Elapsed.TotalSeconds;
            plotU.Plot.Axes.SetLimitsX(now - HalfWindow, now + HalfWindow);
            plotI.Plot.Axes.SetLimitsX(now - HalfWindow, now + HalfWindow);
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
            plotU.Plot.Remove<VerticalLine>();
            plotI.Plot.Remove<VerticalLine>();
            plotU.Plot.Add.VerticalLine(x, color: Colors.Gray.WithAlpha(0.7f), width: 1.5f, pattern: LinePattern.Dashed);
            plotI.Plot.Add.VerticalLine(x, color: Colors.Gray.WithAlpha(0.7f), width: 1.5f, pattern: LinePattern.Dashed);
        }

        //================================================
        // Приход данных
        //================================================
        private void OnDataUpdated(MeterPacket p)
        {
            if (page.InvokeRequired)
            {
                page.BeginInvoke(new Action(() => OnDataUpdated(p)));
                return;
            }
            if (((TabControl)page.Parent).SelectedTab != page) return;

            LabStorage.labsett.sendData = "k";

            lastPacket = p;
            RecordDuration = LabStorage.labsett.AVTime;

            // Цифры
            double supplied = p.UL1_AB;                 // поданное, В
            double appliedKV = supplied * DividerK / 1000.0; // приложенное, кВ

            lblUa.Text = supplied.ToString("F1");
            lblUaP.Text = appliedKV.ToString("F2");
            lblIa.Text = p.I1_A.ToString("F2");

            // % от нормы
            string wind = cmbWinding.SelectedItem?.ToString() ?? "ВН";
            double req = GetRequiredAppliedVoltage(wind);
            if (req > 0)
            {
                double pct = appliedKV / req * 100.0;
                lblPercent.Text = $"{pct:F1} %";
                lblPercent.ForeColor = pct > 105 ? Color.OrangeRed :
                                       pct > 95 ? Color.DarkGreen : Color.DarkOrange;
            }
            else
            {
                lblPercent.Text = "—";
            }

            if (mode == ChartMode.Stopped) return;

            double t = sw.Elapsed.TotalSeconds;

            if (mode == ChartMode.Idle)
            {
                double minKeep = t - HalfWindow - 5;
                TrimOld(logUa, minKeep);
                TrimOld(logIa, minKeep);
            }

            if (mode == ChartMode.Recording)
            {
                recTimes.Add(t);
                recUa.Add(supplied);   // сохраняем поданное
                recIa.Add(p.I1_A);
            }

            logUa.Add(t, supplied);
            logIa.Add(t, p.I1_A);
        }

        private void TrimOld(DataLogger logger, double minX)
        {
            var coords = logger.Data.Coordinates;
            int removeCount = 0;
            for (int i = 0; i < coords.Count; i++)
            {
                if (coords[i].X < minX) removeCount++;
                else break;
            }
            if (removeCount > 0)
                coords.RemoveRange(0, removeCount);
        }
    }
}