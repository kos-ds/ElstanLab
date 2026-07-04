using System.Drawing;
using System.Windows.Forms;
using ElstanLab.Themes;
using ElstanLab.Models;

namespace ElstanLab.UI
{
    public static class PassportPageBuilder
    {
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

            title.Height = 45;

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

                new FieldInfo("Примечание","txtNote",FieldType.MultiLine)
            });

            GroupBox gbPassport = ControlFactory.CreateGroup("Паспорт трансформатора");
            GroupBoxBuilder.Build(gbPassport,

            new FieldInfo[]
            {
            
                new FieldInfo(
                    "Тип",
                    "txtType",
                    FieldType.TextBox),

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
                    "паспортное Uk, %",
                    "UkPassport",
                    FieldType.Numeric)
                {
                    DecimalPlaces = 1,
                    Maximum = 1000,
                    Increment = 1,
                    DefaultValue = 4.5M
                },

                new FieldInfo(
                    "паспортное Pk, Вт",
                    "PkPassport",
                    FieldType.Numeric)
                {
                    DecimalPlaces = 1,
                    //Maximum = 1000,
                    Increment = 1,
                    DefaultValue = 5000
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
                    }
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
                    }
                }
            });

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
                           
            
               /* new FieldInfo(
                    "Положение ПБВ/РПН",
                    "cmbHVTapPosition",
                    FieldType.ComboBox)
                {
                    Items = new[]
                    {
                        "-5%",
                        "-2.5%",
                        "0%",
                        "+2.5%",
                        "+5%"
                    },
                    DefaultValue = 0
                },
            */
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
            
                
              /*  new FieldInfo(
                    "Положение ПБВ/РПН",
                    "cmbLVTapPosition",
                    FieldType.ComboBox)
                {
                    Items = new[]
                    {
                        "-5%",
                        "-2.5%",
                        "0%",
                        "+2.5%",
                        "+5%"
                    }
                },
            */
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

            GroupBox gbCalc = ControlFactory.CreateGroup("Расчетные данные");

            GroupBoxBuilder.Build(gbCalc,

            new FieldInfo[]
            {
                new FieldInfo("Коэффициент трансформации","txtRatio",FieldType.ReadOnly),
            
                new FieldInfo("Расчетный ток ВН","txtIHV",FieldType.ReadOnly),
            
                new FieldInfo("Расчетный ток НН","txtILV",FieldType.ReadOnly)
            
                
            });

            table.Controls.Add(gbInfo, 0, 0);

            table.Controls.Add(gbPassport, 1, 0);

            table.Controls.Add(gbHV, 0, 1);

            table.Controls.Add(gbLV, 1, 1);

            table.Controls.Add(gbCalc, 0, 2);

            table.SetColumnSpan(gbCalc, 2);
        }
    }
}