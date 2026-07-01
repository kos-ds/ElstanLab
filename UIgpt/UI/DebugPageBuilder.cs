using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ElstanLab.Models;
using ElstanLab.Services;

namespace ElstanLab.UI
{
    public class DebugPageBuilder
    {
        private readonly TabPage page;

        private readonly Dictionary<string, Label> labels
            = new Dictionary<string, Label>();

        public DebugPageBuilder(TabPage tabPage)
        {
            page = tabPage;

            Build();

            DataModelService.DataUpdated += OnDataUpdated;
        }

        private void Build()
        {
            TableLayoutPanel table = new TableLayoutPanel();

            table.Dock = DockStyle.Fill;
            table.AutoScroll = true;
            table.ColumnCount = 2;
            table.RowCount = 0;

            table.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 180));

            table.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            page.Controls.Add(table);

            //////////////////////////////////////////////////
            // Состояния
            //////////////////////////////////////////////////

            AddRow(table, "COMM1");
            AddRow(table, "COMM2");

            AddRow(table, "MODE");
            AddRow(table, "SNAPSHOT");

            AddRow(table, "F1");
            AddRow(table, "F2");

            //////////////////////////////////////////////////
            // Прибор 1
            //////////////////////////////////////////////////

            AddRow(table, "U1_A");
            AddRow(table, "U1_B");
            AddRow(table, "U1_C");

            AddRow(table, "UL1_AB");
            AddRow(table, "UL1_BC");
            AddRow(table, "UL1_CA");

            AddRow(table, "I1_A");
            AddRow(table, "I1_B");
            AddRow(table, "I1_C");

            AddRow(table, "P1_A");
            AddRow(table, "P1_B");
            AddRow(table, "P1_C");

            AddRow(table, "PTOTAL");

            //////////////////////////////////////////////////
            // Прибор 2
            //////////////////////////////////////////////////

            AddRow(table, "U2_A");
            AddRow(table, "U2_B");
            AddRow(table, "U2_C");

            AddRow(table, "UL2_AB");
            AddRow(table, "UL2_BC");
            AddRow(table, "UL2_CA");
        }

        private void AddRow(TableLayoutPanel table, string key)
        {
            int row = table.RowCount++;

            table.RowStyles.Add(
                new RowStyle(SizeType.AutoSize));

            Label lblName = new Label();

            lblName.Text = key;
            lblName.AutoSize = true;
            lblName.Padding = new Padding(5);

            Label lblValue = new Label();

            lblValue.Text = "-";
            lblValue.AutoSize = true;
            lblValue.Padding = new Padding(5);
            lblValue.Font = new Font(
                "Consolas",
                10,
                FontStyle.Bold);

            table.Controls.Add(lblName, 0, row);
            table.Controls.Add(lblValue, 1, row);

            labels[key] = lblValue;
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

            //////////////////////////////////////////////////
            // Состояния
            //////////////////////////////////////////////////

            SetValue("COMM1", p.COMM1);
            SetValue("COMM2", p.COMM2);

            SetValue("MODE", p.MODE);
            SetValue("SNAPSHOT", p.SNAPSHOT);

            SetValue("F1", p.F1.ToString("F2"));
            SetValue("F2", p.F2.ToString("F2"));

            //////////////////////////////////////////////////
            // Прибор 1
            //////////////////////////////////////////////////

            SetValue("U1_A", p.U1_A.ToString("F2"));
            SetValue("U1_B", p.U1_B.ToString("F2"));
            SetValue("U1_C", p.U1_C.ToString("F2"));

            SetValue("UL1_AB", p.UL1_AB.ToString("F2"));
            SetValue("UL1_BC", p.UL1_BC.ToString("F2"));
            SetValue("UL1_CA", p.UL1_CA.ToString("F2"));

            SetValue("I1_A", p.I1_A.ToString("F2"));
            SetValue("I1_B", p.I1_B.ToString("F2"));
            SetValue("I1_C", p.I1_C.ToString("F2"));

            SetValue("P1_A", p.P1_A.ToString("F2"));
            SetValue("P1_B", p.P1_B.ToString("F2"));
            SetValue("P1_C", p.P1_C.ToString("F2"));

            SetValue("PTOTAL", p.PTOTAL.ToString("F2"));

            //////////////////////////////////////////////////
            // Прибор 2
            //////////////////////////////////////////////////

            SetValue("U2_A", p.U2_A.ToString("F2"));
            SetValue("U2_B", p.U2_B.ToString("F2"));
            SetValue("U2_C", p.U2_C.ToString("F2"));

            SetValue("UL2_AB", p.UL2_AB.ToString("F2"));
            SetValue("UL2_BC", p.UL2_BC.ToString("F2"));
            SetValue("UL2_CA", p.UL2_CA.ToString("F2"));
        }

        private void SetValue(string key, object value)
        {
            if (!labels.ContainsKey(key))
                return;

            labels[key].Text = value.ToString();
        }
    }
}