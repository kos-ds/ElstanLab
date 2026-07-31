using DocumentFormat.OpenXml.Drawing;
using ElstanLab.Models;
using ElstanLab.Services;
using ElstanLab.Themes;
using ElstanLab.UI;
using ScottPlot;
using ScottPlot.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FontStyle = System.Drawing.FontStyle;
using Label = System.Windows.Forms.Label;

namespace ElstanLab.Pages.IVWPage1
{

    public class IVWPageBuilder1
    {

        private bool testRunning = false;
        private DateTime testStartTime;

        private const double WindowSeconds = 300.0; // 5 минут

        private DateTime startTime;

        private ScottPlot.Plottables.DataLogger uaLog;
        private ScottPlot.Plottables.DataLogger ubLog;
        private ScottPlot.Plottables.DataLogger ucLog;

        private ScottPlot.Plottables.DataLogger iaLog;
        private ScottPlot.Plottables.DataLogger ibLog;
        private ScottPlot.Plottables.DataLogger icLog;

        private FormsPlot plotA;
        private FormsPlot plotB;
        private FormsPlot plotC;

        private Label lblUa;
        private Label lblUb;
        private Label lblUc;

        private Label lblIa;
        private Label lblIb;
        private Label lblIc;

        private Label lblTimer;

        private Button btnStart;
        private Button btnStop;
        private Button btnReset;
        private Button btnSnapshot;

        private Label CreateValueLabel()
        {
            return new Label()
            {
                //Dock = DockStyle.Fill,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Font = new Font("Consolas", 12, FontStyle.Bold),
                Text = "---"
            };
        }



        //------------------------------------------------
        // UI
        //------------------------------------------------

        private readonly TabPage page;
        public IVWPageBuilder1(TabPage tabPage)
        {
            page = tabPage;

            Build();

            DataModelService.DataUpdated += OnDataUpdated;
        }

        

        private void Build()
        {
            page.Controls.Clear();

            TableLayoutPanel main = new TableLayoutPanel();

            main.Dock = DockStyle.Fill;

            main.ColumnCount = 1;

            main.RowCount = 5;

            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 34));
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));

            page.Controls.Add(main);

            //////////////////////////////////////////////////////
            // Top
            //////////////////////////////////////////////////////

            main.Controls.Add(BuildTopPanel(), 0, 0);

            //////////////////////////////////////////////////////
            // Charts
            //////////////////////////////////////////////////////

            plotA = BuildChart("Фаза A");
            plotB = BuildChart("Фаза B");
            plotC = BuildChart("Фаза C");

            main.Controls.Add(plotA, 0, 1);
            main.Controls.Add(plotB, 0, 2);
            main.Controls.Add(plotC, 0, 3);

            //////////////////////////////////////////////////////
            // Buttons
            //////////////////////////////////////////////////////

            main.Controls.Add(BuildBottomPanel(), 0, 4);
            InitCharts();
        }

        private void InitCharts()
        {
            startTime = DateTime.Now;

            uaLog = plotA.Plot.Add.DataLogger();
            iaLog = plotA.Plot.Add.DataLogger();

            iaLog.Axes.YAxis = plotA.Plot.Axes.Right;

            ubLog = plotB.Plot.Add.DataLogger();
            ibLog = plotB.Plot.Add.DataLogger();

            ibLog.Axes.YAxis = plotB.Plot.Axes.Right;

            ucLog = plotC.Plot.Add.DataLogger();
            icLog = plotC.Plot.Add.DataLogger();

            icLog.Axes.YAxis = plotC.Plot.Axes.Right;
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

        
        private FormsPlot BuildChart(string title)
        {
            FormsPlot fp = new FormsPlot();

            fp.Dock = DockStyle.Fill;

           // fp.Plot.Title(title);

            //fp.Plot.XLabel("Время, с");

            fp.Plot.Axes.Left.Label.Text = "Напряжение, В";

            fp.Plot.Axes.Right.Label.Text = "Ток, А";

            fp.Plot.Axes.Right.IsVisible = true;
          //  fp.Plot.Layout.Fixed(new PixelPadding(45, 40, 20, 10));

            return fp;
        }

        private FormsPlot BuildChart1(string title)
        {
            FormsPlot fp = new FormsPlot();

            fp.Dock = DockStyle.Fill;

            fp.Plot.Title(title);
            fp.Plot.XLabel("Время, с");

            fp.Plot.Axes.Left.Label.Text = "U, В";
            fp.Plot.Axes.Right.Label.Text = "I, А";
            fp.Plot.Axes.Right.IsVisible = true;

            // уменьшить шрифты
        //    fp.Plot.Font.Size = 9;

            // убрать лишние поля
            fp.Plot.Layout.Fixed(new PixelPadding(45, 40, 20, 10));

            return fp;
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            testRunning = true;

            testStartTime = DateTime.Now;

            uaLog.Clear();
            ubLog.Clear();
            ucLog.Clear();

            iaLog.Clear();
            ibLog.Clear();
            icLog.Clear();

            plotA.Plot.Axes.SetLimitsX(0, WindowSeconds);
            plotB.Plot.Axes.SetLimitsX(0, WindowSeconds);
            plotC.Plot.Axes.SetLimitsX(0, WindowSeconds);

            plotA.Refresh();
            plotB.Refresh();
            plotC.Refresh();
        }

        private void BtnSnapshot_Click(object sender, EventArgs e)
        {
            if (!testRunning)
                return;

            double x = (DateTime.Now - testStartTime).TotalSeconds;

            plotA.Plot.Add.VerticalLine(x);
            plotB.Plot.Add.VerticalLine(x);
            plotC.Plot.Add.VerticalLine(x);

            plotA.Refresh();
            plotB.Refresh();
            plotC.Refresh();
        }

        private TableLayoutPanel BuildBottomPanel()
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

            btnSnapshot = new Button() { Text = "SNAPSHOT", Dock = DockStyle.Fill };
            btnSnapshot.Click += BtnSnapshot_Click;
            lblTimer = CreateValueLabel();

            lblTimer.Text = "00:00";

            p.Controls.Add(btnStart, 0, 0);
            p.Controls.Add(btnStop, 1, 0);
            p.Controls.Add(btnReset, 2, 0);
            p.Controls.Add(btnSnapshot, 3, 0);
            p.Controls.Add(lblTimer, 4, 0);

            return p;
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            testRunning = false;
        }

        private void OnDataUpdated(MeterPacket p)
        {
            if (page.InvokeRequired)
            {
                page.BeginInvoke((Action)(() =>
                {
                    OnDataUpdated(p);
                }));

                return;
            }
            //int txtt;
            if (((TabControl)page.Parent).SelectedTab != page) return;

            lblUa.Text = p.UL1_AB.ToString("F1");
            lblUb.Text = p.UL1_BC.ToString("F1");
            lblUc.Text = p.UL1_CA.ToString("F1");

            lblIa.Text = p.I1_A.ToString("F2");
            lblIb.Text = p.I1_B.ToString("F2");
            lblIc.Text = p.I1_C.ToString("F2");

            // double t = (DateTime.Now - startTime).TotalSeconds;
            double t;

            if (testRunning)
                t = (DateTime.Now - testStartTime).TotalSeconds;
            else
                t = (DateTime.Now - startTime).TotalSeconds;
            if (!testRunning)
            {
                double left = Math.Max(0, t - WindowSeconds);

                plotA.Plot.Axes.SetLimitsX(left, left + WindowSeconds);
                plotB.Plot.Axes.SetLimitsX(left, left + WindowSeconds);
                plotC.Plot.Axes.SetLimitsX(left, left + WindowSeconds);
            }

            if (testRunning)
            {
                if (t < WindowSeconds)
                {
                    plotA.Plot.Axes.SetLimitsX(0, WindowSeconds);
                    plotB.Plot.Axes.SetLimitsX(0, WindowSeconds);
                    plotC.Plot.Axes.SetLimitsX(0, WindowSeconds);
                }
                else
                {
                    plotA.Plot.Axes.SetLimitsX(t - WindowSeconds, t);
                    plotB.Plot.Axes.SetLimitsX(t - WindowSeconds, t);
                    plotC.Plot.Axes.SetLimitsX(t - WindowSeconds, t);
                }
            }

            uaLog.Add(t, p.UL1_AB);
            ubLog.Add(t, p.UL1_BC);
            ucLog.Add(t, p.UL1_CA);

            iaLog.Add(t, p.I1_A);
            ibLog.Add(t, p.I1_B);
            icLog.Add(t, p.I1_C);

            plotA.Plot.Axes.AutoScale();
            plotB.Plot.Axes.AutoScale();
            plotC.Plot.Axes.AutoScale();

            plotA.Refresh();
            plotB.Refresh();
            plotC.Refresh();



        }
        


    }
}