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

namespace UIgpt
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        CheckBox chkAuto;
        ToolStripComboBox cmbPorts;
        

        public MainForm()
        {
            InitializeComponent();
            ////////////////////Page 1
            PassportPageBuilder.Build(tabPassport);
            FieldBinder.BindCalculationEvents();
            TransformerCalculator.Calculate();
            //new RatioPageBuilder(tabRatio);
            /////////////////////////
            new DebugPageBuilder(tabOther);
            new RatioPageBuilder(tabRatio);
            //////////////status strip and  com port
            CreateStatusCheckbox();
            LoadPorts();
            SerialService.ConnectionChanged += SerialService_ConnectionChanged;
            SerialService.ModeChanged += SerialService_ModeChanged;
            SerialService.Start();
            /////////////////
           

            DataModelService.DataUpdated += DataModelService_DataUpdated;
            ///////////////Ktr



            ///////////////////
        }

        void SerialService_ConnectionChanged(bool state)
        {
            BeginInvoke((Action)(() =>
            {
                lblConnection.Text = state ? "● Подключено" : "○ Нет подключения";
                lblConnection.ForeColor = state ? Color.Green : Color.Red;
            }));
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
            
            

            statusStrip1.Items.Add(new ToolStripStatusLabel("   "));

            statusStrip1.Items.Add(lblMode);

            statusStrip1.Items.Add(new ToolStripStatusLabel("   "));

            statusStrip1.Items.Add(lblConnection);
        }


        void LoadPorts()
        {
            cmbPorts.Items.Clear();

            string[] ports =
                SerialPort.GetPortNames();

            Array.Sort(ports);

            cmbPorts.Items.AddRange(ports);

            string saved =
                Properties.Settings.Default.ComPort;

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
    }
}
