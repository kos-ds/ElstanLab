using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ElstanLab.UI;
using ElstanLab.Models;
using ElstanLab.Services;
using System.IO.Ports;
using ElstanLab.Pages.RatioPage;
using ElstanLab.Pages.ShortCircuitPage;
using ElstanLab.Pages.NoLoadPage;
using ElstanLab.Pages.ReportPage;
using ElstanLab.Pages.IVWPage;
using ElstanLab.Pages.AVPage;

namespace UIgpt
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        CheckBox chkAuto;
        ToolStripComboBox cmbPorts;
        public static MainForm Instance { get; private set; }
        private NoLoadPageBuilder noLoadPage;
        private IVWPageBuilder IVWPage;
        private AVPageBuilder AVPage;
        private ShortCircuitPageBuilder shortCircuitPage;
        private RatioPageBuilder ratioPage;

        public MainForm()
        {
            InitializeComponent();
            Instance = this;
            SettingsManager.Load();
         //   tabMain.TabPages.Remove(tabOther);
         //   tabMain.TabPages.Remove(tabIVW);
            ////////////////////Page 1
            PassportPageBuilder.Build(tabPassport);
            FieldBinder.BindCalculationEvents();
            TransformerCalculator.Calculate();

            /////////////////////////
            new DebugPageBuilder(tabOther);
            ratioPage = new RatioPageBuilder(tabRatio);
            shortCircuitPage = new ShortCircuitPageBuilder(tabShortCircuit);
            noLoadPage = new NoLoadPageBuilder(tabNoLoad);
            new ReportPageBuilder(tabReport);
            IVWPage = new IVWPageBuilder(tabIVW);
            AVPage = new AVPageBuilder(tabAV);
            // reportPage.PassportModel = PassportModel;

            //  reportPage.NoLoadSnapshots = NoLoadSnapshots;
            //////////////status strip and  com port
            CreateStatusCheckbox();
            LoadPorts();
            SerialService.ConnectionChanged += SerialService_ConnectionChanged;
            SerialService.ModeChanged += SerialService_ModeChanged;
            SerialService.Start();
            /////////////////
           

            DataModelService.DataUpdated += DataModelService_DataUpdated;
            ///////////////Ktr
            PacketParser.SnapshotRequested += PacketParser_SnapshotRequested;


            ///////////////////
        }

        private void PacketParser_SnapshotRequested()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(PacketParser_SnapshotRequested));
                return;
            }

            ExternalSnapshot();
        }

        public void ExternalSnapshot()
        {
            if (tabMain.SelectedTab == tabNoLoad)
            {
                noLoadPage.MakeSnapshot();
            }
            else if (tabMain.SelectedTab == tabShortCircuit)
            {
                shortCircuitPage.MakeSnapshot();
            }
            else if (tabMain.SelectedTab == tabRatio)
            {
                ratioPage.MakeSnapshot();
            }
        }

        void SerialService_ConnectionChanged(bool state)
        {
            BeginInvoke((Action)(() =>
            {
                if (state)
                {
                    lblConnection.Text = "● Подключено";
                    lblConnection.ForeColor = Color.Green;
                    LabStorage.labsett.connect = true;
                } else
                {
                    lblConnection.Text =  "○ Нет подключения";
                    lblConnection.ForeColor =  Color.Red;
                    LabStorage.labsett.connect = false;
                }

            }));
        }

        

        public void SetStatus(string text, Color xx )
        {
            dopdata.Text = text;
            dopdata.ForeColor = xx;
        }

        void DataModelService_DataUpdated(MeterPacket p)
        {
            if (InvokeRequired)
            {
                BeginInvoke((Action)(() =>
                {
                    DataModelService_DataUpdated(p);
                }));

         
                return;
            }
            
        }

        void SerialService_ModeChanged(LabMode mode)
        {
            BeginInvoke((Action)(() =>
            {
                lblMode.Text = "Режим: " + ModeNames.Get(mode);

                if (chkAuto.Checked)
                {
                    switch (mode)
                    {
                        case LabMode.Passport:
                            tabMain.SelectedTab =
                                tabPassport;
                            break;

                        case LabMode.KTR:
                            tabMain.SelectedTab =
                                tabRatio;
                            break;

                        case LabMode.ShortCircuit:
                            tabMain.SelectedTab =
                                tabShortCircuit;
                            break;

                        case LabMode.NoLoad:
                            tabMain.SelectedTab =
                                tabNoLoad;
                            break;

                        case LabMode.IVW:
                            tabMain.SelectedTab =
                                tabIVW;
                            break;

                        case LabMode.Other:
                            tabMain.SelectedTab =
                                tabOther;
                            break;

                        case LabMode.Report:
                            tabMain.SelectedTab =
                                tabReport;
                            break;
                    }
                
                }
                
            }));
        }

        void CreateStatusCheckbox()
        {
            chkAuto = new CheckBox();

            chkAuto.Text = "Автопереключение";

            chkAuto.Checked = true;

            chkAuto.AutoSize = true;

            chkAuto.BackColor = Color.Transparent;

            ToolStripControlHost host = new ToolStripControlHost(chkAuto);

            Button btnSettings = new Button();
            btnSettings.Text = "⚙";
            btnSettings.Height = 30;
            btnSettings.Width = 30;

            btnSettings.Click += BtnSettings_Click;
            ToolStripControlHost setting = new ToolStripControlHost(btnSettings);

            cmbPorts = new ToolStripComboBox();

            cmbPorts.Width = 80;

            cmbPorts.DropDownStyle = ComboBoxStyle.DropDownList;

            cmbPorts.SelectedIndexChanged += CmbPorts_SelectedIndexChanged;
            statusStrip1.Items.Add(new ToolStripStatusLabel("   "));
            statusStrip1.Items.Add(new ToolStripStatusLabel("COM:"));
            statusStrip1.Items.Add(new ToolStripStatusLabel("   "));

            statusStrip1.Items.Add(cmbPorts);
            statusStrip1.Items.Add(new ToolStripStatusLabel("   "));
            statusStrip1.Items.Add(host);

            ToolStripStatusLabel spacer = new ToolStripStatusLabel();
            spacer.Spring = true;   // занимает всё свободное пространство

            statusStrip1.Items.Add(new ToolStripStatusLabel("   "));

            statusStrip1.Items.Add(lblMode);

            statusStrip1.Items.Add(new ToolStripStatusLabel("   "));

            statusStrip1.Items.Add(lblConnection);
           
            statusStrip1.Items.Add(dopdata);

            //statusStrip1.Items.Add(new ToolStripStatusLabel("   "));
            statusStrip1.Items.Add(spacer);

            statusStrip1.Items.Add(setting);
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            using (SettingsForm f = new SettingsForm())
            {
                f.ShowDialog();
            }
        }

        void LoadPorts()
        {
            cmbPorts.Items.Clear();

            string[] ports = SerialPort.GetPortNames();

            Array.Sort(ports);

            cmbPorts.Items.AddRange(ports);

            string saved = Properties.Settings.Default.ComPort;

            if (!string.IsNullOrWhiteSpace(saved))
            {
                if (cmbPorts.Items.Contains(saved))
                {
                    cmbPorts.SelectedItem = saved;
                }
            }

            if (cmbPorts.SelectedIndex < 0 &&
                cmbPorts.Items.Count > 0)
            {
                cmbPorts.SelectedIndex = 0;
            }
        }

        void CmbPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPorts.SelectedItem == null)
                return;

            string port =
                cmbPorts.SelectedItem.ToString();

            Properties.Settings.Default.ComPort =
                port;

            Properties.Settings.Default.Save();

            SerialService.SetPort(port);
        }

        private void tabPassport_Click(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel3_Click(object sender, EventArgs e)
        {

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void tabMain_SelectedIndexChanged(object sender, EventArgs e)
        {
          //  if (tabMain.SelectedTab == tabRatio)
          //  {
          //      new RatioPageBuilder(tabRatio);
          //  }
        }

        private void lblConnection_Click(object sender, EventArgs e)
        {

        }

        private void tabAV_Click(object sender, EventArgs e)
        {

        }
    }
}
