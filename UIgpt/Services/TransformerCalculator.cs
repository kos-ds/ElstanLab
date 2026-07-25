using System;
using System.Windows.Forms;
using ElstanLab.UI;
using ElstanLab.Models;

namespace ElstanLab.Services
{
    public static class TransformerCalculator
    {
        private static PassportModel passport = LabStorage.Passport;

        public static void Calculate()
        {
            try
            {
                decimal hv =
                    ControlRegistry
                    .Get<NumericUpDown>("numHVVoltage")
                    .Value;
                
                decimal lv =
                    ControlRegistry
                    .Get<NumericUpDown>("numLVVoltage")
                    .Value;

                decimal power =
                    ControlRegistry
                    .Get<NumericUpDown>("numPower")
                    .Value;

                //-------------------------------------------------
                // Коэффициент трансформации
                //-------------------------------------------------

                if (lv > 0)
                {
                    decimal ratio = hv / lv;

                    ControlRegistry
                        .Get<TextBox>("txtRatio")
                        .Text =
                        ratio.ToString("F3");
                    passport.Ratio = (double)ratio;
                }

                //-------------------------------------------------
                // Ток ВН
                //-------------------------------------------------

                if (hv > 0)
                {
                    decimal ihv =
                        (power * 1000M) /
                        ((decimal)Math.Sqrt(3) * hv * 1000M);

                    ControlRegistry
                        .Get<TextBox>("txtIHV")
                        .Text =
                        ihv.ToString("F2");
                    passport.IHV = (double)ihv;
                }

                //-------------------------------------------------
                // Ток НН
                //-------------------------------------------------

                if (lv > 0)
                {
                    decimal ilv =
                        (power * 1000M) /
                        ((decimal)Math.Sqrt(3) * lv * 1000M);

                    ControlRegistry
                        .Get<TextBox>("txtILV")
                        .Text =
                        ilv.ToString("F2");
                    passport.ILV = (double)ilv;

                }
            }
            catch
            {
            }
        }
    }
}