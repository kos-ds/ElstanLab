using System;
using System.Drawing;
using System.Windows.Forms;
using ElstanLab.Models;
using ElstanLab.Services;

namespace ElstanLab.UI
{
    public class SettingsForm : Form
    {
        NumericUpDown numDeltaU;
        NumericUpDown numDeltaI;
        NumericUpDown numP0;
        NumericUpDown numI0;

        NumericUpDown numUk;
        NumericUpDown numPk;
        NumericUpDown numKzUdelta;
        NumericUpDown numKzIdelta;

        NumericUpDown numRatio;
        NumericUpDown kDeviation;
        CheckBox chkAuto;

        public SettingsForm()
        {
            Text = "Настройки";
            Width = 420;
            Height = 500;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AutoSize = true;
            MaximizeBox = false;
            MinimizeBox = false;

            BuildUI();
            LoadValues();
        }

        void BuildUI()
        {
            TableLayoutPanel main = new TableLayoutPanel();

            main.Dock = DockStyle.Fill;
            main.Padding = new Padding(10);
            main.RowCount = 4;

            Controls.Add(main);

            //////////////////////////////////////////////////////
            // ХХ
            //////////////////////////////////////////////////////

            GroupBox gb1 = new GroupBox();
            gb1.Text = "Испытание холостого хода";
            gb1.Dock = DockStyle.Top;
            gb1.Height = 140;

            TableLayoutPanel t1 = CreateTable();
            numDeltaU = AddRow(t1, 0, "ΔU, %");
            numDeltaI = AddRow(t1, 1, "ΔI, %");
            numP0 = AddRow(t1, 2, "Pxx отклонение, %");
            numI0 = AddRow(t1, 3, "Ixx отклонение, %");

            gb1.Controls.Add(t1);

            //////////////////////////////////////////////////////
            // КЗ
            //////////////////////////////////////////////////////

            GroupBox gb2 = new GroupBox();
            gb2.Text = "Испытание КЗ";
            gb2.Height = 140;

            TableLayoutPanel t2 = CreateTable();

            numUk = AddRow(t2, 0, "Uk отклонение, %");
            numPk = AddRow(t2, 1, "Pk отклонение, %");
            numKzUdelta = AddRow(t2, 2, "ΔU между фазами, %");
            numKzIdelta = AddRow(t2, 3, "ΔI между фазами, %");               
                                                                        

            gb2.Controls.Add(t2);

            //////////////////////////////////////////////////////
            // КТР
            //////////////////////////////////////////////////////

            GroupBox gb3 = new GroupBox();
            gb3.Text = "Коэффициент трансформации";
            gb3.Height = 80;

            TableLayoutPanel t3 = CreateTable();

            numRatio = AddRow(t3, 0, "Ошибка Kтр, %");
            kDeviation = AddRow(t3, 1, "Несимметрия между фазами");

            gb3.Controls.Add(t3);

            //////////////////////////////////////////////////////
            // Общие
            //////////////////////////////////////////////////////

            chkAuto = new CheckBox();
            chkAuto.Text = "Автоматически выбирать последний Snapshot";
            chkAuto.Dock = DockStyle.Top;

            //////////////////////////////////////////////////////
            // Кнопки
            //////////////////////////////////////////////////////

            FlowLayoutPanel buttons = new FlowLayoutPanel();

            buttons.FlowDirection = FlowDirection.RightToLeft;
            buttons.Dock = DockStyle.Fill;

            Button btnOk = new Button();
            btnOk.Text = "Сохранить";
            btnOk.Width = 100;
            btnOk.Click += BtnOk_Click;

            Button btnCancel = new Button();
            btnCancel.Text = "Отмена";
            btnCancel.Width = 100;
            btnCancel.Click += (s, e) => Close();

            buttons.Controls.Add(btnOk);
            buttons.Controls.Add(btnCancel);

            //////////////////////////////////////////////////////

            main.Controls.Add(gb1);
            main.Controls.Add(gb2);
            main.Controls.Add(gb3);
            main.Controls.Add(chkAuto);
            main.Controls.Add(buttons);
        }

        TableLayoutPanel CreateTable()
        {
            TableLayoutPanel t = new TableLayoutPanel();

            t.Dock = DockStyle.Fill;
            t.ColumnCount = 2;
            t.RowCount = 6;

            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

            return t;
        }

        NumericUpDown AddRow(TableLayoutPanel t, int row, string text)
        {
            Label l = new Label();

            l.Text = text;
            l.Dock = DockStyle.Fill;
            l.TextAlign = ContentAlignment.MiddleLeft;

            NumericUpDown n = new NumericUpDown();

            n.DecimalPlaces = 1;
            n.Maximum = 10000;
            n.Minimum = 0;
            n.Increment = 0.1M;
            n.Dock = DockStyle.Fill;

            t.Controls.Add(l, 0, row);
            t.Controls.Add(n, 1, row);

            return n;
        }

        void LoadValues()
        {
            numDeltaU.Value = (decimal)LabStorage.labsett.NoLoadDeltaU;
            numDeltaI.Value = (decimal)LabStorage.labsett.NoLoadDeltaI;

            numP0.Value = (decimal)LabStorage.labsett.NoLoadP0Deviation;
            numI0.Value = (decimal)LabStorage.labsett.NoLoadI0Deviation;

            numUk.Value = (decimal)LabStorage.labsett.ShortCircuitUkDeviation;
            numPk.Value = (decimal)LabStorage.labsett.ShortCircuitPkDeviation;
            numKzUdelta.Value = (decimal)LabStorage.labsett.ShortCircuitVoltageDelta;
            numKzIdelta.Value = (decimal)LabStorage.labsett.ShortCircuitCurrentDelta;

            numRatio.Value = (decimal)LabStorage.labsett.RatioDeviation;

            kDeviation.Value = (decimal)LabStorage.labsett.RatioKdeviation;

            chkAuto.Checked = LabStorage.labsett.AutoSelectSnapshot;
        }

        void BtnOk_Click(object sender, EventArgs e)
        {
            LabStorage.labsett.NoLoadDeltaU = (double)numDeltaU.Value;
            LabStorage.labsett.NoLoadDeltaI = (double)numDeltaI.Value;

            LabStorage.labsett.NoLoadP0Deviation = (double)numP0.Value;
            LabStorage.labsett.NoLoadI0Deviation = (double)numI0.Value;

            LabStorage.labsett.ShortCircuitUkDeviation = (double)numUk.Value;
            LabStorage.labsett.ShortCircuitPkDeviation = (double)numPk.Value;
            LabStorage.labsett.ShortCircuitVoltageDelta = (double)numKzUdelta.Value;
            LabStorage.labsett.ShortCircuitCurrentDelta = (double)numKzIdelta.Value;

            LabStorage.labsett.RatioDeviation = (double)numRatio.Value;

            LabStorage.labsett.RatioKdeviation = (double)kDeviation.Value;

            LabStorage.labsett.AutoSelectSnapshot = chkAuto.Checked;

            SettingsManager.Save();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SettingsForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "SettingsForm";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.ResumeLayout(false);

        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {

        }
    }
}