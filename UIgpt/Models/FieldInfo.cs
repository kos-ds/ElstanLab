using System.Windows.Forms;

namespace ElstanLab.Models
{
    public enum FieldType
    {
        TextBox,
        Numeric,
        ComboBox,
        Date,
        MultiLine,
        ReadOnly
    }

    public class FieldInfo
    {
        public string Label { get; set; }

        public string Name { get; set; }

        public FieldType Type { get; set; }

        public string[] Items { get; set; }

        public string Unit { get; set; }

        public string DefaultText { get; set; } = "";

        // Numeric settings

        public int DecimalPlaces { get; set; } = 0;

        public decimal Minimum { get; set; } = 0;

        public decimal Maximum { get; set; } = 1000000;

        public decimal Increment { get; set; } = 1;

        public decimal DefaultValue { get; set; } = 0;

        public FieldInfo(
            string label,
            string name,
            FieldType type)
        {
            Label = label;

            Name = name;

            Type = type;
        }
    }
}