using System.Drawing;
using System.Windows.Forms;
using ElstanLab.Models;

namespace ElstanLab.UI
{
    public static class GroupBoxBuilder
    {
        public static void Build(GroupBox group, FieldInfo[] fields)
        {
            TableLayoutPanel table = new TableLayoutPanel();

            // table.Dock = DockStyle.Fill;
            table.Dock = DockStyle.Top;

            table.ColumnCount = 2;

            table.RowCount = fields.Length;

            table.AutoSize = true;

            table.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            //table.Padding = new Padding(5);
            table.Padding = new Padding(8);
            table.Margin = new Padding(0);

            table.AutoScroll = true;

            table.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 40));

            table.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 60));

            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].Type == FieldType.MultiLine)
                {
                    table.RowStyles.Add(
                        new RowStyle(SizeType.Absolute, 70));//80
                }
                else
                {
                    table.RowStyles.Add(
                        new RowStyle(SizeType.Absolute, 30));//34
                }
            }

            for (int row = 0; row < fields.Length; row++)
            {
                FieldInfo field = fields[row];

                Label lbl =
                    ControlFactory.CreateLabel(field.Label);

                Control ctrl = CreateControl(field);
                ctrl.Anchor = AnchorStyles.Left |AnchorStyles.Right;

                ControlRegistry.Register(ctrl);

                table.Controls.Add(lbl, 0, row);

                table.Controls.Add(ctrl, 1, row);
            }

            group.Controls.Add(table);
        }

        static Control CreateControl(FieldInfo field)
        {
            switch (field.Type)
            {
                case FieldType.TextBox:

                    TextBox tb =
                        ControlFactory.CreateTextBox();

                    tb.Name = field.Name;

                    tb.Text = field.DefaultText;

                    return tb;

                case FieldType.Numeric:

                    NumericUpDown num =
                        ControlFactory.CreateNumeric();

                    num.Name = field.Name;

                    num.DecimalPlaces =
                        field.DecimalPlaces;

                    num.Minimum =
                        field.Minimum;

                    num.Maximum =
                        field.Maximum;

                    num.Increment =
                        field.Increment;

                    num.Value =
                        field.DefaultValue;

                    return num;

                case FieldType.ComboBox:

                    ComboBox cb =
                        ControlFactory.CreateCombo();

                    cb.Name = field.Name;

                    if (field.Items != null)
                        cb.Items.AddRange(field.Items);

                    return cb;

                case FieldType.Date:

                    DateTimePicker dt =
                        new DateTimePicker();

                    //dt.Dock = DockStyle.Fill;
                    dt.Anchor = AnchorStyles.Left |AnchorStyles.Right | AnchorStyles.Top;

                    dt.Name = field.Name;

                    dt.Margin = new Padding(3);

                    return dt;

                case FieldType.MultiLine:

                    TextBox mt =
                        ControlFactory.CreateTextBox();

                    mt.Multiline = true;

                    mt.Height = 70;

                    mt.ScrollBars = ScrollBars.Vertical;

                    mt.Name = field.Name;

                    return mt;

                case FieldType.ReadOnly:

                    TextBox ro =
                        ControlFactory.CreateTextBox();

                    ro.ReadOnly = true;

                    ro.BackColor = Color.WhiteSmoke;

                    ro.Name = field.Name;

                    return ro;
            }

            return null;
        }
    }
}