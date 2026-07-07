using System;
using System.Drawing;
using System.Windows.Forms;
using ElstanLab.Models;
using ElstanLab.Services;
using ElstanLab.UI;
using System.Collections.Generic;

namespace ElstanLab.Pages.ShortCircuitPage
{
   /* public class ShortCircuitSnapshot
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
        // Calculated
        //------------------------------------------------

        public double UkPercent;

        public double Zk;

        public double Rk;

        public double Xk;

        //------------------------------------------------
        // Expected
        //------------------------------------------------

        public double NominalCurrent;

        public double ExpectedUkVoltage;

        //------------------------------------------------
        // IEC
        //------------------------------------------------

        public bool Passed;

        //////////////////////////////////////////////////
        // Test mode
        //////////////////////////////////////////////////

        public bool Recalculated;

        public double CurrentPercent;

        //////////////////////////////////////////////////
        // Corrected
        //////////////////////////////////////////////////

        public double CorrectedUkPercent;

        public double CorrectedLosses;

        public double UkPassp;
        public double PkPassp;
        public double UkOtklon;
        public double PkOtklon;
    }
    */

    public class ShortCircuitPageBuilder
    {
        //------------------------------------------------
        // UI
        //------------------------------------------------

        private readonly TabPage page;

        private DataGridView grid;

        private ShortCircuitSnapshot currentData = LabStorage.CurrentKz;

        private List<ShortCircuitSnapshot> snapshots = LabStorage.KzSnapshots;

        //private ShortCircuitSnapshot currentData = new ShortCircuitSnapshot();

        //private readonly List<ShortCircuitSnapshot> snapshots = new List<ShortCircuitSnapshot>();


        //------------------------------------------------
        // Labels
        //------------------------------------------------

        private Label lblUab;
        private Label lblUbc;
        private Label lblUca;
        private Label lblUavg;
        private Label lblUdelta;


        private Label lblIa;
        private Label lblIb;
        private Label lblIc;
        private Label lblIavg;
        private Label lblIdelta;

        private Label lblPa;
        private Label lblPb;
        private Label lblPc;
        private Label lblPtotal;

        private Label lblUk;
        private Label lblZk;
        private Label lblRk;
        private Label lblXk;

        private Label lblInom;
        private Label lblUkExpected;

        //////////////////////////////////////////////////
        // Mode
        //////////////////////////////////////////////////

        private Label lblCurrentPercent;
        
        private Label lblUk1;

        private Label lblPtotal1;

        private Label lblCorrectedPk;

        private Label lblCorrectedUk;

        private Label lblUkPassp;

        private Label lblPkPassp;

        private Label lblUkOtklon;

        private Label lblPkOtklon;

        //------------------------------------------------
        // ctor
        //------------------------------------------------

        public ShortCircuitPageBuilder(
            TabPage tabPage)
        {
            page = tabPage;

            Build();

            DataModelService.DataUpdated += OnDataUpdated;
        }

        //------------------------------------------------
        // Build
        //------------------------------------------------

        private void Build()
        {
            TableLayoutPanel main = new TableLayoutPanel();

            main.Dock = DockStyle.Fill;

            main.RowCount = 2;

            main.RowStyles.Add(new RowStyle(SizeType.Percent, 60));

            main.RowStyles.Add(new RowStyle(SizeType.Percent, 40));

            page.Controls.Add(main);

            //////////////////////////////////////////////////
            // Top
            //////////////////////////////////////////////////

            TableLayoutPanel top = BuildTopPanel();

            main.Controls.Add(top, 0, 0);

            //////////////////////////////////////////////////
            // Grid
            //////////////////////////////////////////////////

            grid = BuildGrid();

            main.Controls.Add(grid, 0, 1);

            grid.CellValueChanged += Grid_CellValueChanged;
            grid.CurrentCellDirtyStateChanged += Grid_CurrentCellDirtyStateChanged;

            Button btnSnapshot = new Button();

            btnSnapshot.Text = "SNAPSHOT";

            btnSnapshot.Height = 40;

            btnSnapshot.Dock = DockStyle.Bottom;

            btnSnapshot.Click += BtnSnapshot_Click;

            page.Controls.Add(btnSnapshot);
        }


        private TableLayoutPanel BuildTopPanel()
        {
            TableLayoutPanel top = new TableLayoutPanel();

            top.Dock = DockStyle.Fill;

            top.ColumnCount = 2;//5

            top.RowCount = 3;

            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            top.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
            top.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
            top.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
            
            //////////////////////////////////////////////////
            // Voltages
            //////////////////////////////////////////////////

            top.Controls.Add(BuildVoltageGroup(),0,0);

            //////////////////////////////////////////////////
            // Currents
            //////////////////////////////////////////////////

            top.Controls.Add(BuildCurrentGroup(),1,0);

            //////////////////////////////////////////////////
            // Power
            //////////////////////////////////////////////////

            top.Controls.Add(BuildPowerGroup(),0,1);

            //////////////////////////////////////////////////
            // Calculations
            //////////////////////////////////////////////////

            top.Controls.Add(BuildCalcGroup(),1,1);

            //////////////////////////////////////////////////
            // Expected
            //////////////////////////////////////////////////
            //GroupBox grpExpected = BuildExpectedGroup();
            //top.Controls.Add(grpExpected, 0,2);
            //top.SetColumnSpan(grpExpected, 2);

            top.Controls.Add(BuildExpectedGroup(), 0, 2);

            top.Controls.Add(BuildResultGroup(), 1, 2);

            return top;
        }

        private Label CreateValueLabel()
        {
            Label l = new Label();

            l.Text = "-";

            // l.Dock = DockStyle.Fill;

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

        private GroupBox BuildVoltageGroup()
        {
            GroupBox gb = new GroupBox();

            gb.Text = "Напряжения КЗ";

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

            t.Controls.Add(new Label() { Text = "AB, В", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 0, 0);
            t.Controls.Add(new Label() { Text = "BC, В", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 1, 0);
            t.Controls.Add(new Label() { Text = "CA, В", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 2, 0);
            t.Controls.Add(new Label() { Text = "AVG, В", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 3, 0);
            t.Controls.Add(new Label() { Text = "ΔU, % (±2%)", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 4, 0);

            lblUab = CreateValueLabel();
            lblUbc = CreateValueLabel();
            lblUca = CreateValueLabel();
            lblUavg = CreateValueLabel();
            lblUdelta = CreateValueLabel();

            t.Controls.Add(lblUab, 0, 1);
            t.Controls.Add(lblUbc, 1, 1);
            t.Controls.Add(lblUca, 2, 1);
            t.Controls.Add(lblUavg, 3, 1);
            t.Controls.Add(lblUdelta, 4, 1);

            return gb;
        }

        private GroupBox BuildCurrentGroup()
        {
            GroupBox gb = new GroupBox();

            gb.Text = "Токи";

            gb.Dock = DockStyle.Fill;

            gb.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            TableLayoutPanel t = new TableLayoutPanel();

            t.Dock = DockStyle.Fill;

            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));

            t.ColumnCount = 5;

            t.RowCount = 2;

            t.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            gb.Controls.Add(t);

            t.Controls.Add(new Label() { Text = "A, А", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 0, 0);
            t.Controls.Add(new Label() { Text = "B, А", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 1, 0);
            t.Controls.Add(new Label() { Text = "C, А", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 2, 0);
            t.Controls.Add(new Label() { Text = "AVG, А", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 3, 0);
            t.Controls.Add(new Label() { Text = "ΔIб % (±2%)", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 4, 0);

            lblIa = CreateValueLabel();
            lblIb = CreateValueLabel();
            lblIc = CreateValueLabel();
            lblIavg = CreateValueLabel();
            lblIdelta = CreateValueLabel();

            t.Controls.Add(lblIa, 0, 1);
            t.Controls.Add(lblIb, 1, 1);
            t.Controls.Add(lblIc, 2, 1);
            t.Controls.Add(lblIavg, 3, 1);
            t.Controls.Add(lblIdelta, 4, 1);

            return gb;
        }

        private GroupBox BuildPowerGroup()
        {
            GroupBox gb = new GroupBox();

            gb.Text = "Потери";

            gb.Dock = DockStyle.Fill;

            gb.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            TableLayoutPanel t = new TableLayoutPanel();
            
            t.Dock = DockStyle.Fill;

            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            t.ColumnCount = 4;

            t.RowCount = 2;

            t.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            gb.Controls.Add(t);

            t.Controls.Add(new Label() { Text = "Pa, Вт", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 0, 0);
            t.Controls.Add(new Label() { Text = "Pb, Вт", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 1, 0);
            t.Controls.Add(new Label() { Text = "Pc, Вт", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 2, 0);
            t.Controls.Add(new Label() { Text = "PΣ, Вт", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 3, 0);

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

        private GroupBox BuildCalcGroup()
        {
            GroupBox gb = new GroupBox();

            gb.Text = "Расчет";

            gb.Dock = DockStyle.Fill;

            gb.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            TableLayoutPanel t = new TableLayoutPanel();

            t.Dock = DockStyle.Fill;

            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            t.ColumnCount = 4;

            t.RowCount = 2;

            t.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            gb.Controls.Add(t);

            t.Controls.Add(new Label() { Text = "Uk%", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 0, 0);
            t.Controls.Add(new Label() { Text = "Zk, Ом", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 1, 0);
            t.Controls.Add(new Label() { Text = "Rk, Ом", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 2, 0);
            t.Controls.Add(new Label() { Text = "Xk, Ом", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 3, 0);

            lblUk = CreateValueLabel();
            lblZk = CreateValueLabel();
            lblRk = CreateValueLabel();
            lblXk = CreateValueLabel();

            t.Controls.Add(lblUk, 0, 1);
            t.Controls.Add(lblZk, 1, 1);
            t.Controls.Add(lblRk, 2, 1);
            t.Controls.Add(lblXk, 3, 1);

            return gb;
        }

        private GroupBox BuildExpectedGroup()
        {
            GroupBox gb = new GroupBox();

            gb.Text = "Режим испытания";

            gb.Dock = DockStyle.Fill;

            gb.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            TableLayoutPanel t = new TableLayoutPanel();

            t.Dock = DockStyle.Fill;

            t.ColumnCount = 3;

            t.RowCount = 2;

            t.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));

            gb.Controls.Add(t);

            t.Controls.Add(new Label() { Text = "Iном", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 0, 0);
            t.Controls.Add(new Label() { Text = "Uкз ожидаемое", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 1, 0);
            t.Controls.Add(new Label() { Text = "Ток %", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 2, 0);
            

            lblInom = CreateValueLabel();
            lblUkExpected = CreateValueLabel();
            lblCurrentPercent = CreateValueLabel();
          
            t.Controls.Add(lblInom, 0, 1);
            t.Controls.Add(lblUkExpected, 1, 1);
            t.Controls.Add(lblCurrentPercent, 2, 1);
         
            return gb;
        }        

        private GroupBox BuildResultGroup()
        {
            GroupBox gb = new GroupBox();

            gb.Text = "Результаты испытания";

            gb.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            gb.Dock = DockStyle.Fill;

            TableLayoutPanel t = new TableLayoutPanel();

            t.Dock = DockStyle.Fill;

            t.ColumnCount = 5;

            t.RowCount = 3;

            t.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));

            t.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            t.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            t.RowStyles.Add(new RowStyle(SizeType.Percent, 40));

            gb.Controls.Add(t);

            t.Controls.Add(new Label() { Text = "Параметр", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 0, 0);
            t.Controls.Add(new Label() { Text = "Измерено", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 1, 0);
            t.Controls.Add(new Label() { Text = "Приведенное", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 2, 0);
            t.Controls.Add(new Label() { Text = "Паспорт", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 3, 0);
            t.Controls.Add(new Label() { Text = "Отклонение, %", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 4, 0);

            //////////////////////////////////////////////////
            // Corrected Uk
            //////////////////////////////////////////////////

            t.Controls.Add(new Label() { Text = "Uk, %", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter }, 0, 1);

            lblUk1 = CreateValueLabel();
            t.Controls.Add(lblUk1, 1, 1);

            lblCorrectedUk = CreateValueLabel();
            t.Controls.Add(lblCorrectedUk, 2, 1);

            lblUkPassp = CreateValueLabel();
            t.Controls.Add(lblUkPassp, 3, 1);            

            lblUkOtklon = CreateValueLabel();
            t.Controls.Add(lblUkOtklon, 4, 1);

            //////////////////////////////////////////////////
            // Corrected Pk
            //////////////////////////////////////////////////

            t.Controls.Add(new Label() {Text = "Pk, Вт", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter}, 0, 2);

            lblPtotal1 = CreateValueLabel();
            t.Controls.Add(lblPtotal1, 1, 2);

            lblCorrectedPk = CreateValueLabel();
            t.Controls.Add( lblCorrectedPk, 2, 2);

            lblPkPassp = CreateValueLabel();
            t.Controls.Add(lblPkPassp, 3, 2);

            lblPkOtklon = CreateValueLabel();
            t.Controls.Add(lblPkOtklon, 4, 2);

            return gb;
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


            g.Columns.Add( new DataGridViewCheckBoxColumn() {HeaderText = "✓"});

            g.Columns.Add("Uavg", "Uavg, В");

            g.Columns.Add("Iavg", "Iavg, А");

            g.Columns.Add("Ptotal", "PΣ, Вт");

            g.Columns.Add("Uk", "Uk%");

            g.Columns.Add("Zk", "Zk, Ом");

            g.Columns.Add("Rk", "Rk, Ом");

            g.Columns.Add("Xk", "Xk, Ом");

            g.Columns.Add("Mode", "Mode");

            g.Columns.Add("PkCorr, Вт", "Pk corr");

            g.Columns.Add("UkCorr, %", "Uk corr");

            g.Columns.Add("IEC", "Результат");

            foreach (DataGridViewColumn c in g.Columns)
            {
                c.ReadOnly = true;
                c.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            }

            g.Columns[0].ReadOnly = false;

            return g;
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

            currentData.rowcheckid = e.RowIndex;

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

            double uavg =
                ShortCircuitCalculator
                .CalcVoltageAvg(p);

            lblUavg.Text =
                uavg.ToString("F1");

            double uDelta = ShortCircuitCalculator.CalcVoltageDelta(p);

            lblUdelta.Text = uDelta.ToString("F2");

            bool ok1 = Math.Abs(uDelta) <= 2;

            lblUdelta.ForeColor = ok1
                ? Color.LimeGreen
                : Color.Red;

            //////////////////////////////////////////////////
            // Current
            //////////////////////////////////////////////////

            lblIa.Text = p.I1_A.ToString("F2");
            lblIb.Text = p.I1_B.ToString("F2");
            lblIc.Text = p.I1_C.ToString("F2");
            

            double iavg = ShortCircuitCalculator.CalcCurrentAvg(p);

            lblIavg.Text = iavg.ToString("F2");

            double iDelta = ShortCircuitCalculator.CalcCurrentDelta(p);

            lblIdelta.Text = iDelta.ToString("F2");

            bool ok = Math.Abs(iDelta) <= 2;

            lblIdelta.ForeColor = ok
                ? Color.LimeGreen
                : Color.Red;

            //////////////////////////////////////////////////
            // Power
            //////////////////////////////////////////////////

            lblPa.Text = p.P1_A.ToString("F0");
            lblPb.Text = p.P1_B.ToString("F0");
            lblPc.Text = p.P1_C.ToString("F0");

            lblPtotal.Text = p.PTOTAL.ToString("F0");

            //////////////////////////////////////////////////
            // Passport
            //////////////////////////////////////////////////
            
            double hvVoltage = GetDouble("numHVVoltage") * 1000.0;
            double powerKva = GetDouble("numPower") ;
            currentData.UkPassp = GetDouble("UkPassport");
            currentData.PkPassp = GetDouble("PkPassport");
            //////////////////////////////////////////////////
            // Expected
            //////////////////////////////////////////////////

            double inom = ShortCircuitCalculator.CalcNominalHVCurrent(powerKva, hvVoltage);

            double ukExpected = ShortCircuitCalculator.CalcExpectedUkVoltage(hvVoltage,4.5);
            
            lblInom.Text = inom.ToString("F2") + " A";

            lblUkExpected.Text = ukExpected.ToString("F0") + " V";
            
            //////////////////////////////////////////////////
            // Calculations
            //////////////////////////////////////////////////

            double uk = ShortCircuitCalculator.CalcUkPercent(uavg, hvVoltage);

            double zk = ShortCircuitCalculator.CalcZk( uavg, iavg);

            double rk = ShortCircuitCalculator.CalcRk(p.PTOTAL, iavg);

            double xk = ShortCircuitCalculator.CalcXk( zk, rk);

            lblUk.Text = uk.ToString("F2");

            lblZk.Text = zk.ToString("F3");

            lblRk.Text = rk.ToString("F3");

            lblXk.Text = xk.ToString("F3");

            //////////////////////////////////////////////////
            // Result mode
            //////////////////////////////////////////////////


            lblUk1.Text = uk.ToString("F2");
            lblPtotal1.Text = p.PTOTAL.ToString("F0");

            double correctedUk = ShortCircuitCalculator.RecalculateUk(uk, iavg, inom);
            double correctedPk = ShortCircuitCalculator.RecalculateLosses(p.PTOTAL, iavg, inom);

            lblCorrectedUk.Text = correctedUk.ToString("F2") + " %";

            lblCorrectedPk.Text = correctedPk.ToString("F0");

            lblUkPassp.Text = currentData.UkPassp.ToString("F2");
            lblPkPassp.Text = currentData.PkPassp.ToString("F0");

            currentData.UkOtklon = Math.Abs((correctedUk - currentData.UkPassp) / currentData.UkPassp) * 100.0;
            currentData.PkOtklon = Math.Abs((correctedPk - currentData.PkPassp) / currentData.PkPassp) * 100.0;

            lblUkOtklon.Text = currentData.UkOtklon.ToString("F1");
            lblPkOtklon.Text = currentData.PkOtklon.ToString("F1");

            bool ok2 = Math.Abs(currentData.UkOtklon) <= 10;
            lblUkOtklon.ForeColor = ok2 ? Color.LimeGreen: Color.Red;
            bool ok3 = Math.Abs(currentData.PkOtklon) <= 10;
            lblPkOtklon.ForeColor = ok3 ? Color.LimeGreen : Color.Red;

            //////////////////////////////////////////////////
            // Recalculation mode
            //////////////////////////////////////////////////

            double currentPercent = iavg / inom * 100.0;

            bool recalc = currentPercent < 95.0;
            
            lblCurrentPercent.Text =
                currentPercent.ToString("F1")
                + " %";

            //////////////////////////////////////////////////
            // Store recalculation data
            //////////////////////////////////////////////////

            currentData.Recalculated = recalc;

            currentData.CurrentPercent = currentPercent;

            currentData.CorrectedLosses = correctedPk;

            currentData.CorrectedUkPercent = correctedUk;

            //////////////////////////////////////////////////
            // Storage
            //////////////////////////////////////////////////

            currentData.Time = DateTime.Now;

            //////////////////////////////////////////////////
            // Voltages
            //////////////////////////////////////////////////

            currentData.Uab = p.UL1_AB;
            currentData.Ubc = p.UL1_BC;
            currentData.Uca = p.UL1_CA;
            currentData.Uavg = uavg;
            currentData.deltaU = uDelta;

            //////////////////////////////////////////////////
            // Currents
            //////////////////////////////////////////////////

            currentData.Ia = p.I1_A;
            currentData.Ib = p.I1_B;
            currentData.Ic = p.I1_C;
            currentData.Iavg = iavg;
            currentData.deltaI = iDelta;

            //////////////////////////////////////////////////
            // Power
            //////////////////////////////////////////////////

            currentData.Pa = p.P1_A;
            currentData.Pb = p.P1_B;
            currentData.Pc = p.P1_C;

            currentData.Ptotal = p.PTOTAL;

            //////////////////////////////////////////////////
            // Calculated
            //////////////////////////////////////////////////

            currentData.UkPercent = uk;

            currentData.Zk = zk;

            currentData.Rk = rk;

            currentData.Xk = xk;

            //////////////////////////////////////////////////
            // Expected
            //////////////////////////////////////////////////

            currentData.NominalCurrent  = inom;

            currentData.ExpectedUkVoltage = ukExpected;

            //////////////////////////////////////////////////
            // IEC
            //////////////////////////////////////////////////

            currentData.Passed = ok && ok1 && ok2 && ok3;
        }

        private void BtnSnapshot_Click(object sender,EventArgs e)
        {
            //////////////////////////////////////////////////
            // Clone
            //////////////////////////////////////////////////

            ShortCircuitSnapshot s = new ShortCircuitSnapshot();

            s.Time = currentData.Time;

            //////////////////////////////////////////////////
            // Voltages
            //////////////////////////////////////////////////

            s.Uab = currentData.Uab;
            s.Ubc = currentData.Ubc;
            s.Uca = currentData.Uca;
            s.Uavg = currentData.Uavg;
            s.deltaU = currentData.deltaU;

            //////////////////////////////////////////////////
            // Currents
            //////////////////////////////////////////////////

            s.Ia = currentData.Ia;
            s.Ib = currentData.Ib;
            s.Ic = currentData.Ic;
            s.Iavg = currentData.Iavg;
            s.deltaI = currentData.deltaI;

            //////////////////////////////////////////////////
            // Power
            //////////////////////////////////////////////////

            s.Pa = currentData.Pa;
            s.Pb = currentData.Pb;
            s.Pc = currentData.Pc;
            s.Ptotal = currentData.Ptotal;

            //////////////////////////////////////////////////
            // Calculated
            //////////////////////////////////////////////////

            s.UkPercent = currentData.UkPercent;

            s.Zk = currentData.Zk;

            s.Rk = currentData.Rk;

            s.Xk = currentData.Xk;

            //////////////////////////////////////////////////
            // Expected
            //////////////////////////////////////////////////

            s.NominalCurrent = currentData.NominalCurrent;

            s.ExpectedUkVoltage = currentData.ExpectedUkVoltage;

            //////////////////////////////////////////////////
            // IEC
            //////////////////////////////////////////////////

            s.Passed = currentData.Passed;

            //////////////////////////////////////////////////
            // Recalc
            //////////////////////////////////////////////////

            s.Recalculated = currentData.Recalculated;

            s.CurrentPercent = currentData.CurrentPercent;

            s.CorrectedLosses = currentData.CorrectedLosses;

            s.CorrectedUkPercent = currentData.CorrectedUkPercent;

            s.UkPassp = currentData.UkPassp;
            s.PkPassp = currentData.PkPassp;
            s.UkOtklon = currentData.UkOtklon;
            s.PkOtklon = currentData.PkOtklon;

            snapshots.Add(s);

            AddSnapshotToGrid(s);
        }

        private void AddSnapshotToGrid(ShortCircuitSnapshot s)
        {
            grid.Rows.Add(
                false,

                s.Uavg.ToString("F1"),

                s.Iavg.ToString("F2"),

                s.Ptotal.ToString("F0"),

                s.UkPercent.ToString("F2"),

                s.Zk.ToString("F3"),

                s.Rk.ToString("F3"),

                s.Xk.ToString("F3"),

                s.Recalculated
                    ? "RECALC"
                    : "DIRECT",
                
                s.CorrectedLosses
                    .ToString("F0"),
                
                s.CorrectedUkPercent
                    .ToString("F2"),

                s.Passed
                    ? "OK"
                    : "FAIL");
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