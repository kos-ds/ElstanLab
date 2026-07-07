using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ElstanLab.Models;
using ElstanLab.Services;
using ElstanLab.UI;

namespace ElstanLab.Pages.RatioPage
{
 
    public class RatioPageBuilder
    {

        //private RatioRealtimeData currentData = new RatioRealtimeData();

        private RatioRealtimeData currentData = LabStorage.CurrentKtr;

        private List<RatioRealtimeData> snapshots = LabStorage.KtrSnapshots;

        private int lastHvCount = -1;
        private int lastLvCount = -1;

        private double lastHvStep = -999;
        private double lastLvStep = -999;
        //------------------------------------------------
        // UI
        //------------------------------------------------

        private readonly TabPage page;        

        private DataGridView grid;

        private Button btnSnapshot;

        //------------------------------------------------
        // Realtime labels
        //------------------------------------------------

        //////////////////////////////////////////////////
        // ВН
        //////////////////////////////////////////////////

        private Label lblHvAB;
        private Label lblHvBC;
        private Label lblHvCA;
        private Label lblHvAVG;

        //////////////////////////////////////////////////
        // НН
        //////////////////////////////////////////////////

        private Label lblLvAB;
        private Label lblLvBC;
        private Label lblLvCA;
        private Label lblLvAVG;

        //////////////////////////////////////////////////
        // КТР
        //////////////////////////////////////////////////

        private Label lblKAB;
        private Label lblKBC;
        private Label lblKCA;

        private Label lblKAVG;

        private Label lblDelta;

        //////////////////////////////////////////////////
        // IEC
        //////////////////////////////////////////////////

        private Label lblError;

        private Label lblStatus;

        //------------------------------------------------
        // Positions
        //------------------------------------------------

        private List<RatioPosition> positions = new List<RatioPosition>();

        //------------------------------------------------
        // ctor
        //------------------------------------------------

        public RatioPageBuilder(TabPage tabPage)
        {
            page = tabPage;

            Build();

            LoadPositions();

            DataModelService.DataUpdated += OnDataUpdated;
        }

        //------------------------------------------------
        // Build
        //------------------------------------------------

        private void Build()
        {
            page.Controls.Clear();
            //------------------------------------------------
            // Main layout
            //------------------------------------------------

            TableLayoutPanel main = new TableLayoutPanel();

            main.Dock = DockStyle.Fill;

            main.RowCount = 3;            

            main.RowStyles.Add(new RowStyle(SizeType.Percent, 30));

            main.RowStyles.Add(new RowStyle(SizeType.Percent, 70));

            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

            page.Controls.Add(main);

            //------------------------------------------------
            // Realtime panel
            //------------------------------------------------

            main.Controls.Add(BuildRealtimePanel(), 0, 0);

            //------------------------------------------------
            // Grid
            //------------------------------------------------

            grid = BuildGrid();

            grid.CellClick += curgridind;

            main.Controls.Add(grid, 0, 1);

            //------------------------------------------------
            // Bottom panel
            //------------------------------------------------            

            btnSnapshot = new Button();

            btnSnapshot.Text = "SNAPSHOT";

            btnSnapshot.Dock = DockStyle.Fill;

            btnSnapshot.Height = 40;

            btnSnapshot.Click += BtnSnapshot_Click;

            main.Controls.Add(btnSnapshot, 0, 2);
        }


        private void curgridind(object sender, DataGridViewCellEventArgs e)
        {
            LabStorage.CurrentKtr.rowcheckid = e.RowIndex;
        }
            
        //------------------------------------------------
        // Realtime UI
        //------------------------------------------------

        private TableLayoutPanel BuildRealtimePanel()
        {
            TableLayoutPanel panel = new TableLayoutPanel();

            panel.Dock = DockStyle.Fill;

            panel.ColumnCount = 2;//4

            panel.RowCount = 2;//1
            
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            //////////////////////////////////////////////////
            // ВН
            //////////////////////////////////////////////////

            GroupBox gbHV = BuildVoltageGroup(
                "ВН линейные напряжения",
                out lblHvAB,
                out lblHvBC,
                out lblHvCA,
                out lblHvAVG,
                true);

            panel.Controls.Add(gbHV, 0, 0);

            //////////////////////////////////////////////////
            // НН
            //////////////////////////////////////////////////

            GroupBox gbLV = BuildVoltageGroup(
                "НН линейные напряжения",
                out lblLvAB,
                out lblLvBC,
                out lblLvCA,
                out lblLvAVG,
                false);

            panel.Controls.Add(gbLV, 1, 0);

            //////////////////////////////////////////////////
            // КТР
            //////////////////////////////////////////////////

            GroupBox gbK = BuildRatioGroup();

            panel.Controls.Add(gbK, 0, 1);

            //////////////////////////////////////////////////
            // IEC
            //////////////////////////////////////////////////

            GroupBox gbIEC = BuildIECGroup();

            panel.Controls.Add(gbIEC, 1, 1);

            return panel;
        }

        private GroupBox BuildVoltageGroup(
             string title,
             out Label ab,
             out Label bc,
             out Label ca,
             out Label avg,
             bool type)
        {
            GroupBox gb = new GroupBox();

            gb.Text = title;

            gb.Dock = DockStyle.Fill;

            TableLayoutPanel t = new TableLayoutPanel();

            t.Dock = DockStyle.Fill;

            t.ColumnCount = 4;

            t.RowCount = 2;

            t.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            gb.Controls.Add(t);

            if (type)
            {
                AddHeader(t, 0, "AB");
                AddHeader(t, 1, "BC");
                AddHeader(t, 2, "CA");
                AddHeader(t, 3, "AVG");
            }
            else
            {
                AddHeader(t, 0, "ab");
                AddHeader(t, 1, "bc");
                AddHeader(t, 2, "ca");
                AddHeader(t, 3, "AVG");
            }

            

            ab = AddValue(t, 0);
            bc = AddValue(t, 1);
            ca = AddValue(t, 2);
            avg = AddValue(t, 3);

            return gb;
        }

        private GroupBox BuildRatioGroup()
        {
            GroupBox gb = new GroupBox();

            gb.Text = "Коэффициенты";

            gb.Dock = DockStyle.Fill;

            TableLayoutPanel t = new TableLayoutPanel();

            t.Dock = DockStyle.Fill;

            t.ColumnCount = 5;

            t.RowCount = 2;

            t.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            gb.Controls.Add(t);

            AddHeader(t, 0, "KAB");
            AddHeader(t, 1, "KBC");
            AddHeader(t, 2, "KCA");
            AddHeader(t, 3, "Kavg");
            AddHeader(t, 4, "Δmax");

            lblKAB = AddValue(t, 0);
            lblKBC = AddValue(t, 1);
            lblKCA = AddValue(t, 2);

            lblKAVG = AddValue(t, 3);

            lblDelta = AddValue(t, 4);

            return gb;
        }

        private GroupBox BuildIECGroup()
        {
            GroupBox gb = new GroupBox();

            gb.Text = "IEC";

            gb.Dock = DockStyle.Fill;

            TableLayoutPanel t = new TableLayoutPanel();

            t.Dock = DockStyle.Fill;

            t.ColumnCount = 2;

            t.RowCount = 2;

            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            t.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            t.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            gb.Controls.Add(t);

            Label l1 = new Label();
            l1.Text = "Error % (±0.5)";
            l1.Dock = DockStyle.Fill;
            l1.TextAlign = ContentAlignment.MiddleCenter;
            l1.Font = new Font("Consolas",12,FontStyle.Bold);

            lblError = CreateValueLabel();

            Label l2 = new Label();
            l2.Text = "Status";
            l2.Dock = DockStyle.Fill;
            l2.TextAlign = ContentAlignment.MiddleCenter;
            l2.Font = new Font("Consolas", 12, FontStyle.Bold);

            lblStatus = CreateValueLabel();

            t.Controls.Add(l1, 0, 0);
            t.Controls.Add(lblError, 0, 1);

            t.Controls.Add(l2, 1, 0);
            t.Controls.Add(lblStatus, 1, 1);

            return gb;
        }

        private void AddHeader(TableLayoutPanel t, int col, string text)
        {
            Label l = new Label();

            l.Text = text;

            l.Dock = DockStyle.Fill;
            
            l.TextAlign = ContentAlignment.MiddleCenter;

            l.Font
                = new Font(
                    "Segoe UI",
                    9,
                    FontStyle.Bold);
            l.BackColor = Color.FromArgb(45, 45, 48);

            l.ForeColor = Color.White;

            t.Controls.Add(l, col, 0);
        }

        private Label AddValue(TableLayoutPanel t,int col)
        {
            Label l = CreateValueLabel();
            //l.BackColor = Color.Red;

            t.Controls.Add(l, col, 1);

            return l;
        }

        private Label CreateValueLabel()
        {
            Label l = new Label();

            l.Text = "-";

            // l.Dock = DockStyle.Fill;
            l.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;

         //   l.BackColor = Color.Black;
         //   l.ForeColor = Color.Lime;

            l.AutoSize = false;

            l.TextAlign = ContentAlignment.MiddleCenter;

            l.Font
                = new Font(
                    "Consolas",
                    12,
                    FontStyle.Bold);

            return l;
        }

        //------------------------------------------------
        // Cell
        //------------------------------------------------

        private Label AddRealtimeCell(
            TableLayoutPanel panel,
            int col,
            string title)
        {
            Label lblTitle = new Label();

            lblTitle.Text = title;

            lblTitle.Dock = DockStyle.Fill;

            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            lblTitle.Font
                = new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold);

            panel.Controls.Add(lblTitle, col, 0);

            Label lblValue = new Label();

            lblValue.Text = "-";

            lblValue.Dock = DockStyle.Fill;

            lblValue.TextAlign = ContentAlignment.MiddleCenter;

            lblValue.Font
                = new Font(
                    "Consolas",
                    14,
                    FontStyle.Bold);

            panel.Controls.Add(lblValue, col, 1);

            return lblValue;
        }

        //------------------------------------------------
        // Grid
        //------------------------------------------------

        private DataGridView BuildGrid()
        {
            DataGridView g = new DataGridView();

            g.Dock = DockStyle.Fill;

            g.AllowUserToAddRows = false;

            g.RowHeadersVisible = false;

            g.EnableHeadersVisualStyles = false;

            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);

            g.ColumnHeadersDefaultCellStyle.ForeColor =  Color.White;

            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            g.RowTemplate.Height = 32;

            g.DefaultCellStyle.Font = new Font("Segoe UI", 10);

            g.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            g.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            
           

            //------------------------------------------------
            // Columns
            //------------------------------------------------

            g.Columns.Add(
                new DataGridViewCheckBoxColumn()
                {
                    HeaderText = "✓"
                });

            g.Columns.Add(
                new DataGridViewComboBoxColumn()
                {
                    HeaderText = "Режим",
                    Name = "Mode",
                    Visible = false
                });

            g.Columns.Add("Hv", "ВН %");
            g.Columns.Add("Lv", "НН %");

            g.Columns.Add("Ka", "Ka");
            g.Columns.Add("Kb", "Kb");
            g.Columns.Add("Kc", "Kc");

            g.Columns.Add("Kavg", "Kavg");

            g.Columns.Add("Dev", "Dev %");

            g.Columns.Add("Err", "Err %");

            g.Columns.Add("IEC", "IEC");

            foreach (DataGridViewColumn c in g.Columns)
            {
                c.ReadOnly = true;    
                c.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            }

            g.Columns["Hv"].ReadOnly = false;
            g.Columns["Lv"].ReadOnly = false;

            g.Columns["Mode"].ReadOnly = false;

            g.Columns[0].ReadOnly = false;

            g.Columns[0].FillWeight = 30;

            g.Columns["Hv"].FillWeight = 60;
            g.Columns["Lv"].FillWeight = 60;

            g.Columns["Ka"].FillWeight = 70;
            g.Columns["Kb"].FillWeight = 70;
            g.Columns["Kc"].FillWeight = 70;

            g.Columns["Kavg"].FillWeight = 80;

            g.Columns["Dev"].FillWeight = 70;
            g.Columns["Err"].FillWeight = 70;

            g.Columns["IEC"].FillWeight = 60;

            g.RowTemplate.Height = 32;            

            return g;
        }

        //------------------------------------------------
        // Positions
        //------------------------------------------------

        private void LoadPositions()
        {

            grid.Rows.Clear();

            //////////////////////////////////////////////////
            // Passport values
            //////////////////////////////////////////////////

            int hvCount = LabStorage.Passport.HVTapCount;// GetInt("numHVTapCount");

            int lvCount = LabStorage.Passport.LVTapCount;//GetInt("numLVTapCount");

            double hvStep = LabStorage.Passport.HVPercent ; //GetDouble("percHV");

            double lvStep = LabStorage.Passport.LVPercent;//GetDouble("percLV");

            //////////////////////////////////////////////////
            // Generate
            //////////////////////////////////////////////////

            positions = RatioPositionGenerator.Generate(
                    hvCount,
                    hvStep,
                    lvCount,
                    lvStep);

            snapshots.Clear();

            foreach (RatioPosition p in positions)
            {
                snapshots.Add(new RatioRealtimeData());
            }
            //------------------------------------------------
            // Combo items
            //------------------------------------------------

            DataGridViewComboBoxColumn combo
                = (DataGridViewComboBoxColumn)
                grid.Columns["Mode"];

            combo.Items.Clear();

            foreach (RatioPosition p in positions)
            {
                combo.Items.Add(p.DisplayName);
            }

            //------------------------------------------------
            // Create rows
            //------------------------------------------------

            foreach (RatioPosition p in positions)
            {
                grid.Rows.Add(
                    false,
                    p.DisplayName,
                    p.HvPercent,
                    p.LvPercent,
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "");
            }
        }

        //------------------------------------------------
        // Realtime update
        //------------------------------------------------

        private void OnDataUpdated(MeterPacket p)
        {

            if (grid.CurrentRow == null)
                return;
            
            if (page.InvokeRequired)
            {
                page.BeginInvoke((Action)(() =>
                {
                    OnDataUpdated(p);
                }));

                return;
            }

            if (((TabControl)page.Parent).SelectedTab != page) return;

            int hvCount = GetInt("numHVTapCount");
            int lvCount = GetInt("numLVTapCount");

            double hvStep = GetDouble("percHV");
            double lvStep = GetDouble("percLV");

            bool changed =
                   hvCount != lastHvCount
                || lvCount != lastLvCount
                || hvStep != lastHvStep
                || lvStep != lastLvStep;

            if (changed)
            {
                lastHvCount = hvCount;
                lastLvCount = lvCount;

                lastHvStep = hvStep;
                lastLvStep = lvStep;

                LoadPositions();
            }

            double hvPercent = 0;
            double lvPercent = 0;

            double.TryParse(grid.CurrentRow.Cells["Hv"].Value?.ToString(), out hvPercent);

            double.TryParse(grid.CurrentRow.Cells["Lv"].Value?.ToString(), out lvPercent);

            double baseHv = GetDouble("numHVVoltage");

            double baseLv = GetDouble("numLVVoltage");

            currentData.HvPercent = hvPercent;
            currentData.LvPercent = lvPercent;            

            double nominal = RatioCalculator.CalcNominalRatio(
                 baseHv,
                 baseLv,
                 hvPercent,
                 lvPercent);
            
            //////////////////////////////////////////////////
            // ВН
            //////////////////////////////////////////////////

            lblHvAB.Text = p.UL1_AB.ToString("F2");
            lblHvBC.Text = p.UL1_BC.ToString("F2");
            lblHvCA.Text = p.UL1_CA.ToString("F2");

            double hvAvg =
                (p.UL1_AB
                + p.UL1_BC
                + p.UL1_CA)
                / 3.0;

            lblHvAVG.Text = hvAvg.ToString("F2");

            //////////////////////////////////////////////////
            // НН
            //////////////////////////////////////////////////

            lblLvAB.Text = p.UL2_AB.ToString("F2");
            lblLvBC.Text = p.UL2_BC.ToString("F2");
            lblLvCA.Text = p.UL2_CA.ToString("F2");

            double lvAvg =
                (p.UL2_AB
                + p.UL2_BC
                + p.UL2_CA)
                / 3.0;

            lblLvAVG.Text = lvAvg.ToString("F2");

            //////////////////////////////////////////////////
            // KTR
            //////////////////////////////////////////////////

            double kab = RatioCalculator.CalcKAB(p);

            double kbc = RatioCalculator.CalcKBC(p);

            double kca = RatioCalculator.CalcKCA(p);

            double kavg = RatioCalculator.CalcAverage(kab,kbc,kca);

            

            double dev =
                RatioCalculator.CalcDeviationPercent(
                    kab,
                    kbc,
                    kca,
                    kavg);

            lblKAB.Text = kab.ToString("F3");
            lblKBC.Text = kbc.ToString("F3");
            lblKCA.Text = kca.ToString("F3");

            lblKAVG.Text = kavg.ToString("F3");

            lblDelta.Text = dev.ToString("F3") + " %";

            //////////////////////////////////////////////////
            // IEC
            //////////////////////////////////////////////////

            //  lblError.Text = "0.00 %";
            double err = RatioCalculator.CalcErrorPercent(kavg, nominal);
            lblError.Text = err.ToString("F3") + " %";

            // bool ok = dev <= 0.5;
            bool ok = Math.Abs(err) <= 0.5 && dev <= 0.5;

            lblStatus.Text = ok
                ? "OK"
                : "FAIL";

            lblStatus.ForeColor = ok
                ? Color.LimeGreen
                : Color.Red;
            ///////////////////////////////////////////////////////
            // Save data
            //////////////////////////////////////////////////////
            currentData.HvAB = p.UL1_AB;
            currentData.HvBC = p.UL1_BC;
            currentData.HvCA = p.UL1_CA;
            currentData.HvAVG = hvAvg;

            currentData.LvAB = p.UL2_AB;
            currentData.LvBC = p.UL2_BC;
            currentData.LvCA = p.UL2_CA;
            currentData.LvAVG = lvAvg;

            currentData.KAB = kab;
            currentData.KBC = kbc;
            currentData.KCA = kca;

            currentData.KAVG = kavg;

            currentData.Dev = dev;
            currentData.Err = err;

            currentData.Passed = ok;
        }

        //------------------------------------------------
        // Snapshot
        //------------------------------------------------

        /*private void BtnSnapshot_Click(object sender,EventArgs e)
        {
            if (grid.CurrentRow == null)
                return;

            MeterPacket p = DataModelService.CurrentPacket;

            
            double ka
                = RatioCalculator.CalcKa(p);

            double kb
                = RatioCalculator.CalcKb(p);

            double kc
                = RatioCalculator.CalcKc(p);

            double avg
                = RatioCalculator.CalcAverage(
                    ka,
                    kb,
                    kc);

            double dev
                = RatioCalculator.CalcDeviationPercent(
                    ka,
                    kb,
                    kc,
                    avg);

            double err = 0.0;// RatioCalculator.CalcErrorPercent(avg, nominal);
            //------------------------------------------------
            // Fill row
            //------------------------------------------------

            DataGridViewRow row
                = grid.CurrentRow;

            row.Cells["Ka"].Value
                = ka.ToString("F3");

            row.Cells["Kb"].Value
                = kb.ToString("F3");

            row.Cells["Kc"].Value
                = kc.ToString("F3");

            row.Cells["Kavg"].Value
                = avg.ToString("F3");

            row.Cells["Dev"].Value
                = dev.ToString("F3");

            row.Cells["Err"].Value = err.ToString("F3");

            row.Cells["IEC"].Value
                = dev <= 0.5
                ? "OK"
                : "FAIL";
        }
        */

        private void BtnSnapshot_Click(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null)
                return;
            
            DataGridViewRow row = grid.CurrentRow;

            snapshots[row.Index] = new RatioRealtimeData()
            {
                HvAB = currentData.HvAB,
                HvBC = currentData.HvBC,
                HvCA = currentData.HvCA,

                HvAVG = currentData.HvAVG,

                LvAB = currentData.LvAB,
                LvBC = currentData.LvBC,
                LvCA = currentData.LvCA,

                LvAVG = currentData.LvAVG,

                KAB = currentData.KAB,
                KBC = currentData.KBC,
                KCA = currentData.KCA,

                KAVG = currentData.KAVG,

                Dev = currentData.Dev,
                Err = currentData.Err,
                Passed = currentData.Passed,
                HvPercent = currentData.HvPercent,
                LvPercent = currentData.LvPercent
            };

            row.Cells["Ka"].Value
                = currentData.KAB.ToString("F3");

            row.Cells["Kb"].Value
                = currentData.KBC.ToString("F3");

            row.Cells["Kc"].Value
                = currentData.KCA.ToString("F3");

            row.Cells["Kavg"].Value
                = currentData.KAVG.ToString("F3");

            row.Cells["Dev"].Value
                = currentData.Dev.ToString("F3");

            row.Cells["Err"].Value
                = currentData.Err.ToString("F3");

            row.Cells["IEC"].Value
                = currentData.Passed
                ? "OK"
                : "FAIL";
        }

        private int GetInt(string controlName)
        {
            NumericUpDown tb = ControlRegistry.Get<NumericUpDown>(controlName);

            int value =  (int)tb.Value;

            return value;
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