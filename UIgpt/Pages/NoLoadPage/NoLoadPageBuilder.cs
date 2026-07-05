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
        public double deltaU;

        //------------------------------------------------
        // Currents
        //------------------------------------------------

        public double Ia;
        public double Ib;
        public double Ic;
        public double Iavg;
        public double deltaI;
        
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
        public double Inom;

        public double I0;
        public double P0;
        public double I0Passp;
        public double P0Passp;
        public double I0Otklon;
        public double P0Otklon;

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
        private Label lblP0;

        private Label lblI0Passp;
        private Label lblP0Passp;

        private Label lblI0Otklon;
        private Label lblP0Otklon;

        private Label lblPa;
        private Label lblPb;
        private Label lblPc;
        private Label lblPtotal;

        private Label lblCos;

        private Label lblDeltaI;

        private Label lblDeltaU;

        //------------------------------------------------
        // Storage
        //------------------------------------------------

        private NoLoadSnapshot current = new NoLoadSnapshot();

        private readonly List<NoLoadSnapshot> snapshots = new List<NoLoadSnapshot>();

        //------------------------------------------------
        // ctor
        //------------------------------------------------

        public NoLoadPageBuilder(TabPage tabPage)
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

            main.RowCount = 3;

            main.RowStyles.Add( new RowStyle(SizeType.Percent, 40));

            main.RowStyles.Add( new RowStyle(SizeType.Percent, 60));

            main.RowStyles.Add( new RowStyle(SizeType.Absolute, 50));

            page.Controls.Add(main);

            //////////////////////////////////////////////////
            // Top
            //////////////////////////////////////////////////

            main.Controls.Add(BuildTopPanel(), 0,0);

            //////////////////////////////////////////////////
            // Grid
            //////////////////////////////////////////////////

            grid = BuildGrid();

            main.Controls.Add(grid, 0, 1);

            grid.CellValueChanged += Grid_CellValueChanged;
            grid.CurrentCellDirtyStateChanged += Grid_CurrentCellDirtyStateChanged;

            //////////////////////////////////////////////////
            // Snapshot
            //////////////////////////////////////////////////

            Button btnSnapshot = new Button();

            btnSnapshot.Text = "SNAPSHOT";

            btnSnapshot.Dock = DockStyle.Fill;

            btnSnapshot.Height = 40;

            btnSnapshot.Click += BtnSnapshot_Click;

            main.Controls.Add(btnSnapshot, 0, 2);
        }

        private Label CreateValueLabel()
        {
            Label l = new Label();

            l.Text = "-";

            //l.Dock = DockStyle.Fill;
            l.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;

            l.TextAlign = ContentAlignment.MiddleCenter;

            l.AutoSize = false;

            l.Font
                = new Font(
                    "Consolas",
                    11,
                    FontStyle.Bold);

            return l;
        }

        private TableLayoutPanel BuildTopPanel()
        {
            TableLayoutPanel top = new TableLayoutPanel();

            top.Dock = DockStyle.Fill;

            top.ColumnCount = 2;

            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            top.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            top.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            top.Controls.Add(BuildVoltageGroup(), 0, 0);

            top.Controls.Add(BuildCurrentGroup(), 1, 0);

            top.Controls.Add(BuildPowerGroup(), 0, 1);

            top.Controls.Add(BuildCalcGroup(), 1, 1);

            return top;
        }

        private DataGridView BuildGrid()
        {
            DataGridView g = new DataGridView();

            g.Dock = DockStyle.Fill;

            g.AllowUserToAddRows = false;

            g.RowHeadersVisible = false;

            g.EnableHeadersVisualStyles = false;

            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);

            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            g.RowTemplate.Height = 32;

            g.DefaultCellStyle.Font = new Font("Segoe UI", 10);

            g.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            g.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            g.Columns.Add(new DataGridViewCheckBoxColumn(){HeaderText = "✓"});

            g.Columns.Add("Uavg", "Uavg");

            g.Columns.Add("Iavg", "I0 avg");

            g.Columns.Add("I0", "I0 %");

            g.Columns.Add("P0", "P0");

            g.Columns.Add("Cos", "Cosφ");

            g.Columns.Add("IEC", "IEC");

            foreach (DataGridViewColumn c in g.Columns)
            {
                c.ReadOnly = true;
                c.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            }

            g.Columns[0].ReadOnly = false;

            return g;
        }

        private void BtnSnapshot_Click(object sender, EventArgs e)
        {
            NoLoadSnapshot s = new NoLoadSnapshot();

            s.Time = current.Time;

            s.Uab = current.Uab;
            s.Ubc = current.Ubc;
            s.Uca = current.Uca;
            s.Uavg = current.Uavg;
            s.deltaU = current.deltaU;


            s.Ia = current.Ia;
            s.Ib = current.Ib;
            s.Ic = current.Ic;
            s.Iavg = current.Iavg;
            s.deltaI = current.deltaI;
            
            s.Pa = current.Pa;
            s.Pb = current.Pb;
            s.Pc = current.Pc;
            s.Ptotal = current.Ptotal;
            
            s.CosPhi = current.CosPhi;
            
            s.I0 = current.I0;
            s.I0Otklon = current.I0Otklon;
            s.P0Otklon = current.P0Otklon;
            
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

                s.I0.ToString("F2"),

                s.Ptotal.ToString("F0"),

                s.CosPhi.ToString("F3"),

                snapshots.Count.ToString("F0"),

                s.Passed
                    ? "OK"
                    : "FAIL");
        }


        private GroupBox BuildVoltageGroup()
        {
            GroupBox gb = new GroupBox();

            gb.Text = "Напряжения";

            gb.Dock = DockStyle.Fill;

            TableLayoutPanel t = new TableLayoutPanel();

            t.Dock = DockStyle.Fill;

            gb.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));

            t.ColumnCount = 5;

            t.RowCount = 2;

            t.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            gb.Controls.Add(t);

            t.Controls.Add(new Label(){Text = "AB, В", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter}, 0, 0);
            t.Controls.Add(new Label(){Text = "BC, В", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter}, 1, 0);
            t.Controls.Add(new Label(){Text = "CA, В", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter}, 2, 0);
            t.Controls.Add(new Label(){Text = "AVG, В", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter}, 3, 0);
            t.Controls.Add(new Label(){Text = "ΔU %",   Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter}, 4, 0);

            lblUab = CreateValueLabel();
            lblUbc = CreateValueLabel();
            lblUca = CreateValueLabel();
            lblUavg = CreateValueLabel();
            lblDeltaU = CreateValueLabel();

            t.Controls.Add(lblUab, 0, 1);
            t.Controls.Add(lblUbc, 1, 1);
            t.Controls.Add(lblUca, 2, 1);
            t.Controls.Add(lblUavg, 3, 1);
            t.Controls.Add(lblDeltaU, 4, 1);

            return gb;
        }

        private GroupBox BuildCurrentGroup()
        {
            GroupBox gb = new GroupBox();

            gb.Text = "Токи ХХ";

            gb.Dock = DockStyle.Fill;

            gb.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            TableLayoutPanel t = new TableLayoutPanel();

            t.Dock = DockStyle.Fill;

            t.ColumnCount = 5;

            t.RowCount = 2;

            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            t.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            gb.Controls.Add(t);

            t.Controls.Add(new Label() { Text = "A, А", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 0, 0);
            t.Controls.Add(new Label() { Text = "B, А", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 1, 0);
            t.Controls.Add(new Label() { Text = "C, А", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 2, 0);
            t.Controls.Add(new Label() { Text = "AVG, А", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 3, 0);
            t.Controls.Add(new Label() { Text = "ΔI %", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 4, 0);

            lblIa = CreateValueLabel();
            lblIb = CreateValueLabel();
            lblIc = CreateValueLabel();
            lblIavg = CreateValueLabel();
            lblDeltaI = CreateValueLabel();
            
            t.Controls.Add(lblIa, 0, 1);
            t.Controls.Add(lblIb, 1, 1);
            t.Controls.Add(lblIc, 2, 1);
            t.Controls.Add(lblIavg, 3, 1);
            t.Controls.Add(lblDeltaI, 4, 1);

            return gb;
        }

        private GroupBox BuildPowerGroup()
        {
            GroupBox gb = new GroupBox();

            gb.Text = "Потери ХХ";

            gb.Dock = DockStyle.Fill;

            gb.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            TableLayoutPanel t = new TableLayoutPanel();

            t.Dock = DockStyle.Fill;

            t.ColumnCount = 5;

            t.RowCount = 2;

            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));

            t.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            gb.Controls.Add(t);

            t.Controls.Add(new Label() { Text = "Pa, Вт", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 0, 0);
            t.Controls.Add(new Label() { Text = "Pb, Вт", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 1, 0);
            t.Controls.Add(new Label() { Text = "Pc, Вт", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 2, 0);
            t.Controls.Add(new Label() { Text = "PΣ, Вт", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 3, 0);
            t.Controls.Add(new Label() { Text = "Cosφ", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 4, 0);

            lblPa = CreateValueLabel();
            lblPb = CreateValueLabel();
            lblPc = CreateValueLabel();
            lblPtotal = CreateValueLabel();
            lblCos = CreateValueLabel();

            t.Controls.Add(lblPa, 0, 1);
            t.Controls.Add(lblPb, 1, 1);
            t.Controls.Add(lblPc, 2, 1);
            t.Controls.Add(lblPtotal, 3, 1);
            t.Controls.Add(lblCos, 4, 1);

            return gb;
        }

        private GroupBox BuildCalcGroup()
        {
            GroupBox gb = new GroupBox();

            gb.Text = "Результаты испытания";

            gb.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            gb.Dock = DockStyle.Fill;

            TableLayoutPanel t = new TableLayoutPanel();

            t.Dock = DockStyle.Fill;

            t.ColumnCount = 4;

            t.RowCount = 3;

            t.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            t.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            t.RowStyles.Add(new RowStyle(SizeType.Percent, 35));
            t.RowStyles.Add(new RowStyle(SizeType.Percent, 35));

            gb.Controls.Add(t);

            t.Controls.Add(new Label() { Text = "Параметр", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 0, 0);
            t.Controls.Add(new Label() { Text = "Измерено", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 1, 0);            
            t.Controls.Add(new Label() { Text = "Паспорт", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 2, 0);
            t.Controls.Add(new Label() { Text = "Отклонение, %", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 3, 0);                       

            t.Controls.Add(new Label() { Text = "Pхх, Вт", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 0, 1);

            lblP0 = CreateValueLabel();
            t.Controls.Add(lblP0, 1, 1);

            lblP0Passp = CreateValueLabel();
            t.Controls.Add(lblP0Passp, 2, 1);

            lblP0Otklon = CreateValueLabel();
            t.Controls.Add(lblP0Otklon, 3, 1);

            //////////////////////////////////////////////////
            // Corrected Pk
            //////////////////////////////////////////////////

            t.Controls.Add(new Label() { Text = "Iхх, %", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 0, 2);

            lblI0 = CreateValueLabel();
            t.Controls.Add(lblI0, 1, 2);

            lblI0Passp = CreateValueLabel();
            t.Controls.Add(lblI0Passp, 2, 2);

            lblI0Otklon = CreateValueLabel();
            t.Controls.Add(lblI0Otklon, 3, 2);

            return gb;
        }
       
        private void Grid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            DataGridView g = sender as DataGridView;

            if (g.CurrentCell is DataGridViewCheckBoxCell)
            {
                g.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void Grid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView g = sender as DataGridView;

            if (e.RowIndex < 0 || e.ColumnIndex != 0)
                return;

            bool isChecked =
                Convert.ToBoolean(g.Rows[e.RowIndex].Cells[0].Value);

            if (!isChecked)
                return;

            foreach (DataGridViewRow row in g.Rows)
            {
                if (row.Index != e.RowIndex)
                {
                    row.Cells[0].Value = false;
                }
            }
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

            if (((TabControl)page.Parent).SelectedTab != page) return;

            //////////////////////////////////////////////////
            // Voltage
            //////////////////////////////////////////////////

            lblUab.Text = p.UL1_AB.ToString("F1");

            lblUbc.Text = p.UL1_BC.ToString("F1");

            lblUca.Text = p.UL1_CA.ToString("F1");

            double uavg = NoLoadCalculator.CalcVoltageAvg(p);

            lblUavg.Text = uavg.ToString("F1");

            //////////////////////////////////////////////////
            // Current
            //////////////////////////////////////////////////

            lblIa.Text = p.I1_A.ToString("F2");

            lblIb.Text =  p.I1_B.ToString("F2");

            lblIc.Text = p.I1_C.ToString("F2");

            double iavg = NoLoadCalculator.CalcCurrentAvg(p);

            lblIavg.Text = iavg.ToString("F2");

            //////////////////////////////////////////////////
            // Passport
            //////////////////////////////////////////////////

            double lvVoltage = GetDouble("numLVVoltage") * 1000.0;
            
            double powerKva = GetDouble("numPower");

            current.P0Passp = GetDouble("P0Passp");
            current.I0Passp = GetDouble("I0Passp");

            //////////////////////////////////////////////////
            // Nominal current
            //////////////////////////////////////////////////

            double inom = NoLoadCalculator.CalcNominalCurrent(powerKva, lvVoltage);
            
            //////////////////////////////////////////////////
            // Power
            //////////////////////////////////////////////////

            lblPa.Text = p.P1_A.ToString("F0");

            lblPb.Text = p.P1_B.ToString("F0");

            lblPc.Text = p.P1_C.ToString("F0");

            lblPtotal.Text = p.PTOTAL.ToString("F0");
            

            //////////////////////////////////////////////////
            // Cos
            //////////////////////////////////////////////////

            double cos = NoLoadCalculator.CalcCosPhi(p.PTOTAL, uavg,iavg);

            lblCos.Text = cos.ToString("F3");

            //////////////////////////////////////////////////
            // Delta I
            //////////////////////////////////////////////////

            double deltaI = NoLoadCalculator.CalcDeltaCurrent(p, iavg);

            lblDeltaI.Text = deltaI.ToString("F2");

            bool ok = Math.Abs(deltaI) <= 2;

            lblDeltaI.ForeColor = ok
                ? Color.LimeGreen
                : Color.Red;

            //////////////////////////////////////////////////
            // Delta U
            //////////////////////////////////////////////////

            double deltaU = NoLoadCalculator.CalcDeltaVoltage(p, uavg);

            lblDeltaU.Text = deltaU.ToString("F2");

            bool ok1 = Math.Abs(deltaU) <= 2;

            lblDeltaU.ForeColor = ok1
                ? Color.LimeGreen
                : Color.Red;
            //////////////////////////////////////////////////
            //  Result mode
            //////////////////////////////////////////////////
            current.I0 = NoLoadCalculator.CalcI0Percent(iavg, inom);

            lblP0.Text = p.PTOTAL.ToString("F0");
            lblI0.Text = current.I0.ToString("F2");

            lblP0Passp.Text = current.P0Passp.ToString("F0");
            lblI0Passp.Text = current.I0Passp.ToString("F2");

            current.P0Otklon = ((p.PTOTAL - current.P0Passp) / current.P0Passp) * 100;
            current.I0Otklon = ((current.I0 - current.I0Passp) / current.I0Passp) * 100;

            lblP0Otklon.Text = current.P0Otklon.ToString("F1");
            lblI0Otklon.Text = current.I0Otklon.ToString("F1");

            bool ok2 = Math.Abs(current.P0Otklon) <= 10;
            lblP0Otklon.ForeColor = ok2 ? Color.LimeGreen : Color.Red;
            bool ok3 = Math.Abs(current.I0Otklon) <= 10;
            lblI0Otklon.ForeColor = ok3 ? Color.LimeGreen : Color.Red;



            //////////////////////////////////////////////////
            // Storage
            //////////////////////////////////////////////////

            current.Time = DateTime.Now;

            current.Uab = p.UL1_AB;

            current.Ubc = p.UL1_BC;

            current.Uca = p.UL1_CA;

            current.Uavg = uavg;

            current.deltaU = deltaU;

            current.Ia = p.I1_A;

            current.Ib = p.I1_B;

            current.Ic = p.I1_C;

            current.Iavg = iavg;

            current.deltaI = deltaI;

            current.Pa = p.P1_A;

            current.Pb = p.P1_B;

            current.Pc = p.P1_C;

            current.Ptotal = p.PTOTAL;

            current.CosPhi = cos;
            

        current.Passed = ok && ok1 && ok2 && ok3;
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