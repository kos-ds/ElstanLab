using System.Drawing;
using System.Windows.Forms;
using ElstanLab.Themes;
using ElstanLab.Models;
using ElstanLab.Data;

namespace ElstanLab.UI
{
    public static class PassportPageBuilder
    {
        private static PassportModel passport = LabStorage.Passport;

        public static void Build(TabPage page)
        {
            page.Controls.Clear();

            page.BackColor = Theme.FormBack;

            TableLayoutPanel table = new TableLayoutPanel();

            table.Dock = DockStyle.Fill;

            table.ColumnCount = 2;

            table.RowCount = 3;

            table.Padding = new Padding(8);

            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            table.RowStyles.Add(new RowStyle(SizeType.Percent, 65));

            table.RowStyles.Add(new RowStyle(SizeType.Percent, 35));

            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 170));

            page.Controls.Add(table);

            Label title = new Label();

            title.Text = "ПАСПОРТ ИЗДЕЛИЯ";

            title.Dock = DockStyle.Top;

            title.Height = 40;

            title.TextAlign = ContentAlignment.MiddleCenter;

            title.Font = Theme.HeaderFont;

            title.ForeColor = Theme.HeaderColor;

            page.Controls.Add(title);

            GroupBox gbInfo = ControlFactory.CreateGroup("Основные сведения");
            GroupBoxBuilder.Build(gbInfo,

            new FieldInfo[]
            {
                new FieldInfo("Заказчик","txtCustomer",FieldType.TextBox),
            
                new FieldInfo("Объект","txtObject",FieldType.TextBox),
            
                new FieldInfo("Дата испытания","dtTest",FieldType.Date),
            
                new FieldInfo("Испытатель","txtEngineer",FieldType.TextBox),

                //  new FieldInfo("Примечание","txtNote",FieldType.MultiLine)

                new FieldInfo(
                        "Тип трансформатора",
                        "cmbTransformer",
                        FieldType.ComboBox)
                    {
                        DataSource = TransformerCatalog.Types
                    },

                new FieldInfo(
                    "Материал обмотки",
                    "cmbMaterial",
                    FieldType.ComboBox)
                {
                    Items = new[] { "Cu (медь)", "Al (алюминий)" },
                    DefaultIndex = 1
                }
            });

            TextBox txtCustomer = ControlRegistry.Get<TextBox>("txtCustomer");

            txtCustomer.Text = passport.Customer;

            txtCustomer.TextChanged += (s, e) =>
            {
                passport.Customer = txtCustomer.Text;
            };


            TextBox txtObject =
                ControlRegistry.Get<TextBox>("txtObject");

            txtObject.Text = passport.ObjectName;

            txtObject.TextChanged += (s, e) =>
            {
                passport.ObjectName = txtObject.Text;
            };


            DateTimePicker dtTest = ControlRegistry.Get<DateTimePicker>("dtTest");

            dtTest.Value = passport.TestDate;

            dtTest.ValueChanged += (s, e) =>
            {
                passport.TestDate = dtTest.Value;
            };


            TextBox txtEngineer = ControlRegistry.Get<TextBox>("txtEngineer");

            txtEngineer.Text = passport.Engineer;

            txtEngineer.TextChanged += (s, e) =>
            {
                passport.Engineer = txtEngineer.Text;
            };

            ComboBox cmbMaterial = ControlRegistry.Get<ComboBox>("cmbMaterial");

            cmbMaterial.SelectedIndex = passport.wendtype ;

            cmbMaterial.SelectedIndexChanged += (s, e) =>
            {
                passport.wendtype = cmbMaterial.SelectedIndex;// Text;
            };

            /*
            TextBox txtNote =
                ControlRegistry.Get<TextBox>("txtNote");

            txtNote.Text = passport.Note;

            txtNote.TextChanged += (s, e) =>
            {
                passport.Note = txtNote.Text;
            };
            */




            GroupBox gbPassport = ControlFactory.CreateGroup("Паспорт трансформатора");
            GroupBoxBuilder.Build(gbPassport,

            new FieldInfo[]
            {
                new FieldInfo(
                    "Завод изготовитель",
                    "txtFactory",
                    FieldType.TextBox)
                {
                    DefaultText = "Elstan"
                },

                new FieldInfo(
                    "Год выпуска",
                    "numYear",
                    FieldType.Numeric)
                {
                    DecimalPlaces = 0,
                    Minimum = 1950,
                    Maximum = 2100,
                    Increment = 1,
                    DefaultValue = 2026
                },

                new FieldInfo(
                    "Тип",
                    "txtType",
                    FieldType.TextBox),
                    

                 /*   new FieldInfo(
                    "Тип",
                    "cmbType",
                    FieldType.ComboBox),
                    */
                new FieldInfo(
                    "Серийный №",
                    "txtSerial",
                    FieldType.TextBox),

                new FieldInfo(
                    "Мощность, кВА",
                    "numPower",
                    FieldType.Numeric)
                {
                    DecimalPlaces = 1,
                    Maximum = 100000,
                    Increment = 10,
                    DefaultValue = 160
                },

                new FieldInfo(
                    "Частота, Гц",
                    "numFreq",
                    FieldType.Numeric)
                {
                    DecimalPlaces = 1,
                    Maximum = 1000,
                    Increment = 1,
                    DefaultValue = 50
                },



                new FieldInfo(
                    "Схема соединения",
                    "cmbVector",
                    FieldType.ComboBox)
                {
                    Items = new[]
                    {
                        "Y/Y-0",
                        "Y/Δ-11",
                        "Δ/Y-11",
                        "Dyn11",
                        "Yzn11"
                    },
                    DefaultIndex =0
                },

                new FieldInfo(
                    "Охлаждение",
                    "cmbCooling",
                    FieldType.ComboBox)
                {
                    Items = new[]
                    {
                        "ONAN",
                        "ONAF",
                        "OFAF"
                    },
                    DefaultIndex =0
                }
            });

            TextBox txtFactory =
                ControlRegistry.Get<TextBox>("txtFactory");

            txtFactory.Text = passport.Factory;

            txtFactory.TextChanged += (s, e) =>
            {
                passport.Factory = txtFactory.Text;
            };


            NumericUpDown numYear =
                ControlRegistry.Get<NumericUpDown>("numYear");

            numYear.Value = (decimal)passport.Year;

            numYear.ValueChanged += (s, e) =>
            {
                passport.Year = (int)numYear.Value;
            };


            TextBox txtType =
                ControlRegistry.Get<TextBox>("txtType");

            txtType.Text = passport.Type;

            txtType.TextChanged += (s, e) =>
            {
                passport.Type = txtType.Text;
            };

    
            TextBox txtSerial =
                ControlRegistry.Get<TextBox>("txtSerial");

            txtSerial.Text = passport.Serial;

            txtSerial.TextChanged += (s, e) =>
            {
                passport.Serial = txtSerial.Text;
            };


            NumericUpDown numPower =
                ControlRegistry.Get<NumericUpDown>("numPower");

            numPower.Value = (decimal)passport.PowerKva;

            numPower.ValueChanged += (s, e) =>
            {
                passport.PowerKva = (double)numPower.Value;
            };


            NumericUpDown numFreq =
                ControlRegistry.Get<NumericUpDown>("numFreq");

            numFreq.Value = (decimal)passport.Frequency;

            numFreq.ValueChanged += (s, e) =>
            {
                passport.Frequency = (double)numFreq.Value;
            };


            ComboBox cmbVector = ControlRegistry.Get<ComboBox>("cmbVector");

            cmbVector.Text = passport.VectorGroup;

            cmbVector.SelectedIndexChanged += (s, e) =>
            {
                passport.VectorGroup = cmbVector.Text;
            };


            ComboBox cmbCooling = ControlRegistry.Get<ComboBox>("cmbCooling");
            

            cmbCooling.Text = passport.Cooling;
            

            cmbCooling.SelectedIndexChanged += (s, e) =>
            {
                passport.Cooling = cmbCooling.Text;
            };



            GroupBox gbHV = ControlFactory.CreateGroup("ВН");

            GroupBoxBuilder.Build(gbHV,

             new FieldInfo[]
             {
                new FieldInfo(
                    "Номинальное напряжение, кВ",
                    "numHVVoltage",
                    FieldType.Numeric)
                {
                    DecimalPlaces = 2,
                    Maximum = 1000,
                    Increment = 0.1M,
                    DefaultValue = 10
                },     
              
                new FieldInfo(
                    "Количество положений ВН",
                    "numHVTapCount",
                    FieldType.Numeric)
                {
                    DecimalPlaces = 0,
                    Maximum = 99,
                    Increment = 1,
                    DefaultValue = 1
                },

                 new FieldInfo(
                    "Шаг переключения, %",
                    "percHV",
                    FieldType.Numeric)
                {
                    DecimalPlaces = 1,
                    Maximum = 100,
                    Increment = 0.1M
                }
             });

            NumericUpDown numHVVoltage =
                ControlRegistry.Get<NumericUpDown>("numHVVoltage");

            numHVVoltage.Value = (decimal)passport.HVVoltage;

            numHVVoltage.ValueChanged += (s, e) =>
            {
                passport.HVVoltage = (double)numHVVoltage.Value;
            };


            NumericUpDown numHVTapCount =
                ControlRegistry.Get<NumericUpDown>("numHVTapCount");

            numHVTapCount.Value = passport.HVTapCount;

            numHVTapCount.ValueChanged += (s, e) =>
            {
                passport.HVTapCount = (int)numHVTapCount.Value;
            };


            NumericUpDown percHV =
                ControlRegistry.Get<NumericUpDown>("percHV");

            percHV.Value = (decimal)passport.HVPercent;

            percHV.ValueChanged += (s, e) =>
            {
                passport.HVPercent = (double)percHV.Value;
            };


            GroupBox gbLV = ControlFactory.CreateGroup("НН");

            GroupBoxBuilder.Build(gbLV,

            new FieldInfo[]
            {
                new FieldInfo(
                    "Номинальное напряжение, кВ",
                    "numLVVoltage",
                    FieldType.Numeric)
                {
                    DecimalPlaces = 3,
                    Maximum = 1000,
                    Increment = 0.1M,
                    DefaultValue = 0.4M
                },
            
                new FieldInfo(
                    "Количество положений НН",
                    "numLVTapCount",
                    FieldType.Numeric)
                {
                    DecimalPlaces = 0,
                    Maximum = 99,
                    Increment = 1,
                    DefaultValue = 1
                },

                new FieldInfo(
                    "Шаг переключения, %",
                    "percLV",
                    FieldType.Numeric)
                {
                    DecimalPlaces = 1,
                    Maximum = 100,
                    Increment = 0.1M
                }
            });

            NumericUpDown numLVVoltage =
                ControlRegistry.Get<NumericUpDown>("numLVVoltage");

            numLVVoltage.Value = (decimal)passport.LVVoltage;

            numLVVoltage.ValueChanged += (s, e) =>
            {
                passport.LVVoltage = (double)numLVVoltage.Value;
            };


            NumericUpDown numLVTapCount =
                ControlRegistry.Get<NumericUpDown>("numLVTapCount");

            numLVTapCount.Value = passport.LVTapCount;

            numLVTapCount.ValueChanged += (s, e) =>
            {
                passport.LVTapCount = (int)numLVTapCount.Value;
            };


            NumericUpDown percLV = ControlRegistry.Get<NumericUpDown>("percLV");

            percLV.Value = (decimal)passport.LVPercent;

            percLV.ValueChanged += (s, e) =>
            {
                passport.LVPercent = (double)percLV.Value;
            };

            GroupBox gbCalc = ControlFactory.CreateGroup("Расчетные данные");

            GroupBoxBuilder.Build(gbCalc,

            new FieldInfo[]
            {
                new FieldInfo("Коэффициент трансформации","txtRatio",FieldType.ReadOnly),
            
                new FieldInfo("Расчетный ток ВН","txtIHV",FieldType.ReadOnly),
            
                new FieldInfo("Расчетный ток НН","txtILV",FieldType.ReadOnly)
            
                
            });

            GroupBox gbPasp = ControlFactory.CreateGroup("Поверочные данные");

            GroupBoxBuilder.Build(gbPasp,

            new FieldInfo[]
            {
                new FieldInfo(
                    "паспортное Ukз, %",
                    "UkPassport",
                    FieldType.Numeric)
                {
                    DecimalPlaces = 1,
                    Maximum = 1000,
                    Increment = 1,
                    DefaultValue = 4.5M
                },

                new FieldInfo(
                    "паспортное Pkз, Вт",
                    "PkPassport",
                    FieldType.Numeric)
                {
                    DecimalPlaces = 1,
                    //Maximum = 1000,
                    Increment = 1,
                    DefaultValue = 5000
                },

                new FieldInfo(
                    "паспортное Pхх, Вт",
                    "P0Passp",
                    FieldType.Numeric)
                {
                    DecimalPlaces = 1,
                    //Maximum = 1000,
                    Increment = 1,
                    DefaultValue = 300
                },

                new FieldInfo(
                    "паспортное Iхх, %",
                    "I0Passp",
                    FieldType.Numeric)
                {
                    DecimalPlaces = 1,
                    Maximum = 100,
                    Increment = 1,
                    DefaultValue = 2
                }
            });

            NumericUpDown UkPassport =
                ControlRegistry.Get<NumericUpDown>("UkPassport");

            UkPassport.Value = (decimal)passport.UkPercent;

            UkPassport.ValueChanged += (s, e) =>
            {
                passport.UkPercent = (double)UkPassport.Value;
            };


            NumericUpDown PkPassport =
                ControlRegistry.Get<NumericUpDown>("PkPassport");

            PkPassport.Value = (decimal)passport.PkLoss;

            PkPassport.ValueChanged += (s, e) =>
            {
                passport.PkLoss = (double)PkPassport.Value;
            };


            NumericUpDown P0Passp =
                ControlRegistry.Get<NumericUpDown>("P0Passp");

            P0Passp.Value = (decimal)passport.P0Loss;

            P0Passp.ValueChanged += (s, e) =>
            {
                passport.P0Loss = (double)P0Passp.Value;
            };


            NumericUpDown I0Passp =
                ControlRegistry.Get<NumericUpDown>("I0Passp");

            I0Passp.Value = (decimal)passport.I0Percent;

            I0Passp.ValueChanged += (s, e) =>
            {
                passport.I0Percent = (double)I0Passp.Value;
            };


            ComboBox cmbTransformer = ControlRegistry.Get<ComboBox>("cmbTransformer");
            


            cmbTransformer.SelectedIndexChanged += (s, e) =>
            {
                TransformerType tr = cmbTransformer.SelectedItem as TransformerType;

                if (tr == null)
                    return;

                // Тип
                txtType.Text = tr.Name;

                // Паспорт
                numPower.Value = (decimal)tr.Power;

                // ВН/НН
                numHVVoltage.Value = (decimal)tr.HVVoltage;
                numLVVoltage.Value = (decimal)tr.LVVoltage;

                // Потери
                P0Passp.Value = (decimal)tr.P0Loss;
                PkPassport.Value = (decimal)tr.PkLoss;

                // Паспортные данные
                UkPassport.Value = (decimal)tr.UkPercent;
                I0Passp.Value = (decimal)tr.I0Percent;

            };
            




            table.Controls.Add(gbInfo, 0, 0);

            table.Controls.Add(gbPassport, 1, 0);

            table.Controls.Add(gbHV, 0, 1);

            table.Controls.Add(gbLV, 1, 1);

            table.Controls.Add(gbCalc, 0, 2);

            table.Controls.Add(gbPasp, 1, 2);

            if (cmbTransformer.SelectedIndex == -1)
            {
                TransformerType tr1 = TransformerCatalog.Types[0];
                txtType.Text = tr1.Name;
                numPower.Value = (decimal)tr1.Power;
                numHVVoltage.Value = (decimal)tr1.HVVoltage;
                numLVVoltage.Value = (decimal)tr1.LVVoltage;
                P0Passp.Value = (decimal)tr1.P0Loss;
                PkPassport.Value = (decimal)tr1.PkLoss;
                UkPassport.Value = (decimal)tr1.UkPercent;
                I0Passp.Value = (decimal)tr1.I0Percent;
            }



        }
    }
}