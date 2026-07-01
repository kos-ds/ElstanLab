using System.Windows.Forms;
using ElstanLab.Services;

namespace ElstanLab.UI
{
    public static class FieldBinder
    {
        public static void BindCalculationEvents()
        {
            BindNumeric("numHVVoltage");

            BindNumeric("numLVVoltage");

            BindNumeric("numPower");
        }

        static void BindNumeric(string name)
        {
            NumericUpDown num =
                ControlRegistry
                .Get<NumericUpDown>(name);

            if (num != null)
            {
                num.ValueChanged += (s, e) =>
                {
                    TransformerCalculator.Calculate();
                };
            }
        }
    }
}