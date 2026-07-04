using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ElstanLab.Models;
using ElstanLab.Services;
using ElstanLab.UI;

namespace ElstanLab.Pages.NoLoadPage
{
    public class NoLoadSnapshot
    {
        //------------------------------------------------
        // Time
        //------------------------------------------------

        public DateTime Time;

        //------------------------------------------------
        // Voltages
        //------------------------------------------------

        public double Uab;
        public double Ubc;
        public double Uca;

        public double Uavg;

        //------------------------------------------------
        // Currents
        //------------------------------------------------

        public double Ia;
        public double Ib;
        public double Ic;

        public double Iavg;

        //------------------------------------------------
        // Current %
        //------------------------------------------------

        public double I0Percent;

        //------------------------------------------------
        // Power
        //------------------------------------------------

        public double Pa;
        public double Pb;
        public double Pc;

        public double Ptotal;

        //------------------------------------------------
        // Cos
        //------------------------------------------------

        public double CosPhi;

        //------------------------------------------------
        // IEC
        //------------------------------------------------

        public bool Passed;
    }

    public class NoLoadPageBuilder
    {
        //------------------------------------------------
        // UI
        //------------------------------------------------

        private readonly TabPage page;

        private DataGridView grid;

        //------------------------------------------------
        // Realtime
        //------------------------------------------------

        private Label lblUab;
        private Label lblUbc;
        private Label lblUca;
        private Label lblUavg;

        private Label lblIa;
        private Label lblIb;
        private Label lblIc;
        private Label lblIavg;

        private Label lblI0;

        private Label lblPa;
        private Label lblPb;
        private Label lblPc;
        private Label lblPtotal;

        private Label lblCos;

        private Label lblIEC;

        private Label lblDeltaI;

        private Label lblDeltaU;

        //------------------------------------------------
        // Storage
        //------------------------------------------------

        private NoLoadSnapshot current
            = new NoLoadSnapshot();

        private readonly List<NoLoadSnapshot>
            snapshots
            = new List<NoLoadSnapshot>();

        //------------------------------------------------
        // ctor
        //------------------------------------------------

        public NoLoadPageBuilder(
            TabPage tabPage)
        {
            page = tabPage;

            Build();

            DataModelService.DataUpdated
                += OnDataUpdated;
        }

        private void Build()
        {
            TableLayoutPanel main
                = new TableLayoutPanel();

            main.Dock = DockStyle.Fill;

            main.RowCount = 3;

            main.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 170));

            main.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            main.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 50));

            page.Controls.Add(main);

            //////////////////////////////////////////////////
            // Top
            //////////////////////////////////////////////////

            main.Controls.Add(
                BuildTopPanel(),
                0,
                0);

            //////////////////////////////////////////////////
            // Grid
            //////////////////////////////////////////////////

            grid = BuildGrid();

            main.Controls.Add(
                grid,
                0,
                1);

            //////////////////////////////////////////////////
            // Snapshot
            //////////////////////////////////////////////////

            Button btnSnapshot
                = new Button();

            btnSnapshot.Text = "SNAPSHOT";

            btnSnapshot.Dock = DockStyle.Fill;

            btnSnapshot.Height = 40;

            btnSnapshot.Click += BtnSnapshot_Click;

            main.Controls.Add(
                btnSnapshot,
                0,
                2);
        }

        private Label CreateValueLabel()
        {
            Label l = new Label();

            l.Text = "-";

            l.Dock = DockStyle.Fill;

            l.TextAlign
                = ContentAlignment.MiddleCenter;

            l.Font
                = new Font(
                    "Consolas",
                    11,
                    FontStyle.Bold);

            return l;
        }

        private TableLayoutPanel BuildTopPanel()
        {
            TableLayoutPanel top
                = new TableLayoutPanel();

            top.Dock = DockStyle.Fill;

            top.ColumnCount = 4;

            top.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 25));

            top.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 25));

            top.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 25));

            top.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 25));

            top.Controls.Add(
                BuildVoltageGroup(),
                0,
                0);

            top.Controls.Add(
                BuildCurrentGroup(),
                1,
                0);

            top.Controls.Add(
                BuildPowerGroup(),
                2,
                0);

            top.Controls.Add(
                BuildCalcGroup(),
                3,
                0);

            return top;
        }

        private DataGridView BuildGrid()
        {
            DataGridView g
                = new DataGridView();

            g.Dock = DockStyle.Fill;

            g.AllowUserToAddRows = false;

            g.RowHeadersVisible = false;

            g.AutoSizeColumnsMode
                = DataGridViewAutoSizeColumnsMode.AllCells;

            g.Columns.Add(
                new DataGridViewCheckBoxColumn()
                {
                    HeaderText = "✓"
                });

            g.Columns.Add("Uavg", "Uavg");

            g.Columns.Add("Iavg", "I0 avg");

            g.Columns.Add("I0", "I0 %");

            g.Columns.Add("P0", "P0");

            g.Columns.Add("Cos", "Cosφ");

            g.Columns.Add("IEC", "IEC");

            return g;
        }

        private void BtnSnapshot_Click(
    object sender,
    EventArgs e)
        {
            NoLoadSnapshot s
                = new NoLoadSnapshot();

            s.Time = current.Time;

            s.Uab = current.Uab;
            s.Ubc = current.Ubc;
            s.Uca = current.Uca;

            s.Uavg = current.Uavg;

            s.Ia = current.Ia;
            s.Ib = current.Ib;
            s.Ic = current.Ic;

            s.Iavg = current.Iavg;

            s.I0Percent
                = current.I0Percent;

            s.Pa = current.Pa;
            s.Pb = current.Pb;
            s.Pc = current.Pc;

            s.Ptotal = current.Ptotal;

            s.CosPhi = current.CosPhi;

            s.Passed = current.Passed;

            snapshots.Add(s);

            AddSnapshotToGrid(s);
        }

        private void AddSnapshotToGrid(NoLoadSnapshot s)
        {
            grid.Rows.Add(
                false,

                s.Uavg.ToString("F1"),

                s.Iavg.ToString("F2"),

                s.I0Percent.ToString("F2"),

                s.Ptotal.ToString("F0"),

                s.CosPhi.ToString("F3"),

                s.Passed
                    ? "OK"
                    : "FAIL");
        }


        private GroupBox BuildVoltageGroup()
        {
            GroupBox gb = new GroupBox();

            gb.Text = "Напряжения";

            gb.Dock = DockStyle.Fill;

            TableLayoutPanel t
                = new TableLayoutPanel();

            t.Dock = DockStyle.Fill;

            t.ColumnCount = 4;

            t.RowCount = 2;

            gb.Controls.Add(t);

            t.Controls.Add(
                new Label()
                {
                    Text = "AB",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                },
                0,
                0);

            t.Controls.Add(
                new Label()
                {
                    Text = "BC",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                },
                1,
                0);

            t.Controls.Add(
                new Label()
                {
                    Text = "CA",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                },
                2,
                0);

            t.Controls.Add(
                new Label()
                {
                    Text = "AVG",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                },
                3,
                0);

            lblUab = CreateValueLabel();
            lblUbc = CreateValueLabel();
            lblUca = CreateValueLabel();
            lblUavg = CreateValueLabel();

            t.Controls.Add(lblUab, 0, 1);
            t.Controls.Add(lblUbc, 1, 1);
            t.Controls.Add(lblUca, 2, 1);
            t.Controls.Add(lblUavg, 3, 1);

            return gb;
        }

        private GroupBox BuildCurrentGroup()
        {
            GroupBox gb = new GroupBox();

            gb.Text = "Токи ХХ";

            gb.Dock = DockStyle.Fill;

            TableLayoutPanel t
                = new TableLayoutPanel();

            t.Dock = DockStyle.Fill;

            t.ColumnCount = 4;

            t.RowCount = 3;

            gb.Controls.Add(t);

            t.Controls.Add(new Label() { Text = "A", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 0, 0);
            t.Controls.Add(new Label() { Text = "B", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 1, 0);
            t.Controls.Add(new Label() { Text = "C", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 2, 0);
            t.Controls.Add(new Label() { Text = "AVG", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 3, 0);

            lblIa = CreateValueLabel();
            lblIb = CreateValueLabel();
            lblIc = CreateValueLabel();
            lblIavg = CreateValueLabel();

            t.Controls.Add(lblIa, 0, 1);
            t.Controls.Add(lblIb, 1, 1);
            t.Controls.Add(lblIc, 2, 1);
            t.Controls.Add(lblIavg, 3, 1);

            t.Controls.Add(
                new Label()
                {
                    Text = "I0 %",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                },
                0,
                2);

            lblI0 = CreateValueLabel();

            t.Controls.Add(lblI0, 1, 2);

            return gb;
        }

        private GroupBox BuildPowerGroup()
        {
            GroupBox gb = new GroupBox();

            gb.Text = "Потери ХХ";

            gb.Dock = DockStyle.Fill;

            TableLayoutPanel t
                = new TableLayoutPanel();

            t.Dock = DockStyle.Fill;

            t.ColumnCount = 4;

            t.RowCount = 2;

            gb.Controls.Add(t);

            t.Controls.Add(new Label() { Text = "Pa", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 0, 0);
            t.Controls.Add(new Label() { Text = "Pb", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 1, 0);
            t.Controls.Add(new Label() { Text = "Pc", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 2, 0);
            t.Controls.Add(new Label() { Text = "PΣ", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 3, 0);

            lblPa = CreateValueLabel();
            lblPb = CreateValueLabel();
            lblPc = CreateValueLabel();
            lblPtotal = CreateValueLabel();

            t.Controls.Add(lblPa, 0, 1);
            t.Controls.Add(lblPb, 1, 1);
            t.Controls.Add(lblPc, 2, 1);
            t.Controls.Add(lblPtotal, 3, 1);

            return gb;
        }

        private GroupBox BuildCalcGroup1()
        {
            GroupBox gb = new GroupBox();

            gb.Text = "Расчет";

            gb.Dock = DockStyle.Fill;

            TableLayoutPanel t
                = new TableLayoutPanel();

            t.Dock = DockStyle.Fill;

            t.ColumnCount = 2;

            t.RowCount = 4;

            gb.Controls.Add(t);

            t.Controls.Add(
                new Label()
                {
                    Text = "Cosφ",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft
                },
                0,
                0);

            lblCos = CreateValueLabel();

            t.Controls.Add(lblCos, 1, 0);

            t.Controls.Add(
                new Label()
                {
                    Text = "IEC",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft
                },
                0,
                1);

            lblIEC = CreateValueLabel();

            t.Controls.Add(lblIEC, 1, 1);

            return gb;
        }

        private GroupBox BuildCalcGroup()
        {
            GroupBox gb = new GroupBox();

            gb.Text = "Расчет";

            gb.Dock = DockStyle.Fill;

            TableLayoutPanel t
                = new TableLayoutPanel();

            t.Dock = DockStyle.Fill;

            t.ColumnCount = 2;

            t.RowCount = 7;

            t.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 55));

            t.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 45));

            gb.Controls.Add(t);

            //////////////////////////////////////////////////
            // I0 avg
            //////////////////////////////////////////////////

            t.Controls.Add(
                new Label()
                {
                    Text = "I0 avg",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft
                },
                0,
                0);

            Label lblI0avg =
                CreateValueLabel();

            lblI0avg.Name =
                "lblI0avg";

            t.Controls.Add(lblI0avg, 1, 0);

            //////////////////////////////////////////////////
            // I0 %
            //////////////////////////////////////////////////

            t.Controls.Add(
                new Label()
                {
                    Text = "I0 %",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft
                },
                0,
                1);

            t.Controls.Add(lblI0, 1, 1);

            //////////////////////////////////////////////////
            // P0
            //////////////////////////////////////////////////

            t.Controls.Add(
                new Label()
                {
                    Text = "P0",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft
                },
                0,
                2);

            Label lblP0 =
                CreateValueLabel();

            lblP0.Name =
                "lblP0";

            t.Controls.Add(lblP0, 1, 2);

            //////////////////////////////////////////////////
            // Cos
            //////////////////////////////////////////////////

            t.Controls.Add(
                new Label()
                {
                    Text = "Cosφ",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft
                },
                0,
                3);

            lblCos = CreateValueLabel();
            t.Controls.Add(lblCos, 1, 3);

            //////////////////////////////////////////////////
            // Delta I
            //////////////////////////////////////////////////

            t.Controls.Add(
                new Label()
                {
                    Text = "ΔI %",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft
                },
                0,
                4);

            lblDeltaI =
                CreateValueLabel();

            t.Controls.Add(lblDeltaI, 1, 4);

            //////////////////////////////////////////////////
            // Delta U
            //////////////////////////////////////////////////

            t.Controls.Add(
                new Label()
                {
                    Text = "ΔU %",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft
                },
                0,
                5);

            lblDeltaU =
                CreateValueLabel();

            t.Controls.Add(lblDeltaU, 1, 5);

            //////////////////////////////////////////////////
            // IEC
            //////////////////////////////////////////////////

            t.Controls.Add(
                new Label()
                {
                    Text = "IEC",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft
                },
                0,
                6);

            lblIEC = CreateValueLabel();
            t.Controls.Add(lblIEC, 1, 6);

            return gb;
        }

        private void OnDataUpdated(
    MeterPacket p)
        {
            if (page.InvokeRequired)
            {
                page.BeginInvoke((Action)(() =>
                {
                    OnDataUpdated(p);
                }));

                return;
            }

            //////////////////////////////////////////////////
            // Voltage
            //////////////////////////////////////////////////

            lblUab.Text =
                p.UL1_AB.ToString("F1");

            lblUbc.Text =
                p.UL1_BC.ToString("F1");

            lblUca.Text =
                p.UL1_CA.ToString("F1");

            double uavg =
                NoLoadCalculator
                .CalcVoltageAvg(p);

            lblUavg.Text =
                uavg.ToString("F1");

            //////////////////////////////////////////////////
            // Current
            //////////////////////////////////////////////////

            lblIa.Text =
                p.I1_A.ToString("F2");

            lblIb.Text =
                p.I1_B.ToString("F2");

            lblIc.Text =
                p.I1_C.ToString("F2");

            double iavg =
                NoLoadCalculator
                .CalcCurrentAvg(p);

            lblIavg.Text =
                iavg.ToString("F2");

            //////////////////////////////////////////////////
            // Passport
            //////////////////////////////////////////////////

            double hvVoltage = GetDouble("numHVVoltage") * 1000.0;
            
            double powerKva = GetDouble("numPower");

            //////////////////////////////////////////////////
            // Nominal current
            //////////////////////////////////////////////////

            double inom =
                NoLoadCalculator
                .CalcNominalCurrent(
                    powerKva,
                    hvVoltage);

            //////////////////////////////////////////////////
            // I0 %
            //////////////////////////////////////////////////

            double i0 =
                NoLoadCalculator
                .CalcI0Percent(
                    iavg,
                    inom);

            lblI0.Text =
                i0.ToString("F2")
                + " %";

            //////////////////////////////////////////////////
            // Power
            //////////////////////////////////////////////////

            lblPa.Text =
                p.P1_A.ToString("F0");

            lblPb.Text =
                p.P1_B.ToString("F0");

            lblPc.Text =
                p.P1_C.ToString("F0");

            lblPtotal.Text =
                p.PTOTAL.ToString("F0");

            Label lblP0 =
    page.Controls.Find(
        "lblP0",
        true)[0] as Label;

            lblP0.Text =
                p.PTOTAL.ToString("F0");

            Label lblI0avg =
                page.Controls.Find(
                    "lblI0avg",
                    true)[0] as Label;

            lblI0avg.Text =
                iavg.ToString("F2");

            //////////////////////////////////////////////////
            // Cos
            //////////////////////////////////////////////////

            double cos =
                NoLoadCalculator
                .CalcCosPhi(
                    p.PTOTAL,
                    uavg,
                    iavg);

            lblCos.Text =
                cos.ToString("F3");

            //////////////////////////////////////////////////
            // Delta I
            //////////////////////////////////////////////////

            double deltaI =
                NoLoadCalculator
                .CalcDeltaCurrent(
                    p,
                    iavg);

            lblDeltaI.Text =
                deltaI.ToString("F2")
                + " %";

            //////////////////////////////////////////////////
            // Delta U
            //////////////////////////////////////////////////

            double deltaU =
                NoLoadCalculator
                .CalcDeltaVoltage(
                    p,
                    uavg);

            lblDeltaU.Text =
                deltaU.ToString("F2")
                + " %";

            //////////////////////////////////////////////////
            // IEC
            //////////////////////////////////////////////////

            bool ok =
                i0 <= 30.0;

            lblIEC.Text =
                ok
                ? "OK"
                : "FAIL";

            lblIEC.ForeColor =
                ok
                ? Color.LimeGreen
                : Color.Red;

            //////////////////////////////////////////////////
            // Storage
            //////////////////////////////////////////////////

            current.Time =
                DateTime.Now;

            current.Uab =
                p.UL1_AB;

            current.Ubc =
                p.UL1_BC;

            current.Uca =
                p.UL1_CA;

            current.Uavg =
                uavg;

            current.Ia =
                p.I1_A;

            current.Ib =
                p.I1_B;

            current.Ic =
                p.I1_C;

            current.Iavg =
                iavg;

            current.I0Percent =
                i0;

            current.Pa =
                p.P1_A;

            current.Pb =
                p.P1_B;

            current.Pc =
                p.P1_C;

            current.Ptotal =
                p.PTOTAL;

            current.CosPhi =
                cos;

            current.Passed =
                ok;
        }

        private double GetDouble(string controlName)
        {
            NumericUpDown tb = ControlRegistry.Get<NumericUpDown>(controlName);

            double value = 0;

            double.TryParse(
                tb.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out value);

            return value;
        }




    }
}