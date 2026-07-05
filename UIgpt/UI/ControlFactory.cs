using System.Drawing;
using System.Windows.Forms;
using ElstanLab.Themes;

namespace ElstanLab.UI
{
    public static class ControlFactory
    {
        public static GroupBox CreateGroup(string text)
        {
            return new GroupBox()
            {
                Text = text,
                Dock = DockStyle.Fill,
                Font = Theme.GroupFont,
                BackColor = Theme.GroupBack,
                //Padding = new Padding(10),
                Padding = new Padding(10, 3, 10, 10),
                //Margin = new Padding(8)
                Margin = new Padding(4)
            };
        }

        public static Label CreateLabel(string text)
        {
            return new Label()
            {
                Text = text,

                AutoSize = true,

                Anchor =
                    AnchorStyles.Left,

                TextAlign =
                    ContentAlignment.MiddleLeft,

                Font =
                    Theme.DefaultFont,

                //Margin = new Padding(3, 6, 3, 3)
                Margin = new Padding(3)
            };
        }

        public static TextBox CreateTextBox()
        {
            return new TextBox()
            {
                //Dock = DockStyle.Fill,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                Font = Theme.DefaultFont,
                Margin = new Padding(3),
                Width = 150
            };
        }

        public static NumericUpDown CreateNumeric()
        {
            return new NumericUpDown()
            {
                //Dock = DockStyle.Fill,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                DecimalPlaces = 2,
                Maximum = 1000000,
                Font = Theme.DefaultFont,
                Margin = new Padding(3),
                Width = 150
            };
        }

        public static ComboBox CreateCombo()
        {
            return new ComboBox()
            {
                //Dock = DockStyle.Fill,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = Theme.DefaultFont,
                Margin = new Padding(3),
                Width = 150
            };
        }
    }
}