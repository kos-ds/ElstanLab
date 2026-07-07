
using System;
using System.Text;
using System.Windows.Forms;

using ElstanLab.Models;

namespace ElstanLab.Pages.ReportPage
{
    public class ReportPageBuilder
    {
        //////////////////////////////////////////////////
        // UI
        //////////////////////////////////////////////////

        private TabPage page;

        private Panel topPanel;

        private ComboBox cbType;

        private Button btnGenerate;

        private Button btnPrint;

        private WebBrowser browser;

        //////////////////////////////////////////////////
        // INIT
        //////////////////////////////////////////////////

        public ReportPageBuilder(TabPage tab)
        {
            page = tab;

            Build();
        }

        //////////////////////////////////////////////////
        // BUILD
        //////////////////////////////////////////////////

        private void Build()
        {
            page.Controls.Clear();

            BuildTopPanel();

            BuildBrowser();

            RegisterEvents();
        }

        //////////////////////////////////////////////////
        // TOP PANEL
        //////////////////////////////////////////////////

        private void BuildTopPanel()
        {
            topPanel = new Panel();

            topPanel.Dock = DockStyle.Top;

            topPanel.Height = 50;

            topPanel.Padding = new Padding(5);

            //------------------------------------------------
            // REPORT TYPE
            //------------------------------------------------

            cbType = new ComboBox();

            cbType.Left = 10;

            cbType.Top = 10;

            cbType.Width = 260;

            cbType.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cbType.Items.Add(
                "Потери и ток ХХ");

            cbType.Items.Add(
                "Сопротивление и потери КЗ");

            cbType.Items.Add(
                "Коэффициент трансформации");

            cbType.SelectedIndex = 0;

            //------------------------------------------------
            // GENERATE
            //------------------------------------------------

            btnGenerate = new Button();

            btnGenerate.Text =
                "Сформировать";

            btnGenerate.Left = 290;

            btnGenerate.Top = 8;

            btnGenerate.Width = 140;

            btnGenerate.Height = 32;

            //------------------------------------------------
            // PRINT
            //------------------------------------------------

            btnPrint = new Button();

            btnPrint.Text =
                "Печать";

            btnPrint.Left = 440;

            btnPrint.Top = 8;

            btnPrint.Width = 100;

            btnPrint.Height = 32;

            //------------------------------------------------
            // ADD
            //------------------------------------------------

            topPanel.Controls.Add(cbType);

            topPanel.Controls.Add(btnGenerate);

            topPanel.Controls.Add(btnPrint);

            page.Controls.Add(topPanel);
        }

        //////////////////////////////////////////////////
        // BROWSER
        //////////////////////////////////////////////////

        private void BuildBrowser()
        {
            browser = new WebBrowser();

            browser.Dock = DockStyle.Fill;

            page.Controls.Add(browser);
        }

        //////////////////////////////////////////////////
        // EVENTS
        //////////////////////////////////////////////////

        private void RegisterEvents()
        {
            btnGenerate.Click += BtnGenerate_Click;

            btnPrint.Click += BtnPrint_Click;
        }

        //////////////////////////////////////////////////
        // GENERATE
        //////////////////////////////////////////////////

        private void BtnGenerate_Click(
            object sender,
            EventArgs e)
        {
            string type = cbType.SelectedItem.ToString();

            if (type == "Потери и ток ХХ")
            {
                GenerateNoLoadReport();
            }

            if (type == "Сопротивление и потери КЗ")
            {
                GenerateShortCircuitReport();
            }

            if (type == "Коэффициент трансформации")
            {
                GenerateRatioReport();
            }
        }

        //////////////////////////////////////////////////
        // PRINT
        //////////////////////////////////////////////////

        private void BtnPrint_Click(object sender,  EventArgs e)
        {
            if (browser.Document != null)
            {
                browser.ShowPrintDialog();
            }
        }

        //////////////////////////////////////////////////
        // ХХ REPORT
        //////////////////////////////////////////////////

        private void GenerateNoLoadReport()
        {
            if (LabStorage.NoLoadSnapshots.Count == 0)
            {
                MessageBox.Show("Нет данных ХХ");

                return;
            }

            PassportModel p = LabStorage.Passport;
            
            NoLoadSnapshot last = LabStorage.NoLoadSnapshots[LabStorage.CurrentNoLoad.rowcheckid];

            StringBuilder sb = new StringBuilder();

            BeginHtml(sb);

            //------------------------------------------------
            // TITLE
            //------------------------------------------------

            AddMainTitle(sb, "ПРОТОКОЛ ИСПЫТАНИЯ");

            AddSubTitle(sb, "ПОТЕРИ И ТОК ХОЛОСТОГО ХОДА");

            //------------------------------------------------
            // PASSPORT
            //------------------------------------------------

            AddPassportTable(sb, p);

            //------------------------------------------------
            // RESULTS
            //------------------------------------------------

            AddSectionTitle(sb, "Результаты измерений");

            sb.Append("<table>");

            sb.Append(@"
            <tr>
            <th>Uab</th>
            <th>Ubc</th>
            <th>Uca</th>
            
            <th>Ia</th>
            <th>Ib</th>
            <th>Ic</th>
            
            <th>PΣ</th>
            <th>Cosφ</th>
            </tr>
            ");
      
                var s = LabStorage.NoLoadSnapshots[LabStorage.CurrentNoLoad.rowcheckid];

                sb.Append("<tr>");

                AddCell(sb, s.Uab.ToString("F1"));
                AddCell(sb, s.Ubc.ToString("F1"));
                AddCell(sb, s.Uca.ToString("F1"));

                AddCell(sb, s.Ia.ToString("F2"));
                AddCell(sb, s.Ib.ToString("F2"));
                AddCell(sb, s.Ic.ToString("F2"));

                AddCell(sb, s.Ptotal.ToString("F1"));

                AddCell(sb, s.CosPhi.ToString("F3"));

                sb.Append("</tr>");
            //}

            sb.Append("</table>");

            //------------------------------------------------
            // CALC
            //------------------------------------------------

            AddSectionTitle(sb, "Расчетные параметры");

            sb.Append("<table>");

            AddRow(sb, "Измеренный I0 %", last.I0.ToString("F2"));

            AddRow(
                sb,
                "Паспортный I0 %",
                last.I0Passp.ToString("F2"));

            AddRow(
                sb,
                "Отклонение I0 %",
                last.I0Otklon.ToString("F2"));

            AddRow(
                sb,
                "Измеренные P0",
                last.P0.ToString("F1"));

            AddRow(
                sb,
                "Паспортные P0",
                last.P0Passp.ToString("F1"));

            AddRow(
                sb,
                "Отклонение P0 %",
                last.P0Otklon.ToString("F2"));

            sb.Append("</table>");

            //------------------------------------------------
            // RESULT
            //------------------------------------------------

            AddConclusion(
                sb,
                last.Passed);

            EndHtml(sb);

            browser.DocumentText =
                sb.ToString();
        }

        //////////////////////////////////////////////////
        // КЗ REPORT
        //////////////////////////////////////////////////

        private void GenerateShortCircuitReport()
        {
            if (LabStorage.KzSnapshots.Count == 0)
            {
                MessageBox.Show("Нет данных КЗ");

                return;
            }

            PassportModel p = LabStorage.Passport;
            
            ShortCircuitSnapshot last = LabStorage.KzSnapshots[LabStorage.CurrentKz.rowcheckid];

            StringBuilder sb = new StringBuilder();

            BeginHtml(sb);

            //------------------------------------------------
            // TITLE
            //------------------------------------------------

            AddMainTitle(sb, "ПРОТОКОЛ ИСПЫТАНИЯ");

            AddSubTitle(sb, "СОПРОТИВЛЕНИЕ И ПОТЕРИ КОРОТКОГО ЗАМЫКАНИЯ");

            //------------------------------------------------
            // PASSPORT
            //------------------------------------------------

            AddPassportTable(sb, p);

            //------------------------------------------------
            // RESULTS
            //------------------------------------------------

            AddSectionTitle(sb, "Результаты измерений");

            sb.Append("<table>");

            sb.Append(@"
                <tr>
                
                <th>Uab</th>
                <th>Ubc</th>
                <th>Uca</th>
                
                <th>Ia</th>
                <th>Ib</th>
                <th>Ic</th>
                
                <th>PΣ</th>
                
                <th>Uk%</th>
                
                </tr>
                ");
            
                var s = LabStorage.KzSnapshots[LabStorage.CurrentKz.rowcheckid];

                sb.Append("<tr>");

                AddCell(sb, s.Uab.ToString("F1"));
                AddCell(sb, s.Ubc.ToString("F1"));
                AddCell(sb, s.Uca.ToString("F1"));

                AddCell(sb, s.Ia.ToString("F2"));
                AddCell(sb, s.Ib.ToString("F2"));
                AddCell(sb, s.Ic.ToString("F2"));

                AddCell(sb, s.Ptotal.ToString("F1"));

                AddCell(
                    sb,
                    s.UkPercent.ToString("F2"));

                sb.Append("</tr>");
            //}

            sb.Append("</table>");

            //------------------------------------------------
            // CALC
            //------------------------------------------------

            AddSectionTitle(
                sb,
                "Расчетные параметры");

            sb.Append("<table>");

            AddRow(
                sb,
                "Uk измеренное %",
                last.UkPercent.ToString("F2"));

            AddRow(
                sb,
                "Uk паспортное %",
                last.UkPassp.ToString("F2"));

            AddRow(
                sb,
                "Отклонение Uk %",
                last.UkOtklon.ToString("F2"));

            AddRow(
                sb,
                "Pk измеренное",
                last.Ptotal.ToString("F1"));

            AddRow(
                sb,
                "Pk паспортное",
                last.PkPassp.ToString("F1"));

            AddRow(
                sb,
                "Отклонение Pk %",
                last.PkOtklon.ToString("F2"));

            AddRow(
                sb,
                "Rk",
                last.Rk.ToString("F4"));

            AddRow(
                sb,
                "Xk",
                last.Xk.ToString("F4"));

            AddRow(
                sb,
                "Zk",
                last.Zk.ToString("F4"));

            sb.Append("</table>");

            //------------------------------------------------
            // RESULT
            //------------------------------------------------

            AddConclusion(
                sb,
                last.Passed);

            EndHtml(sb);

            browser.DocumentText =
                sb.ToString();
        }

        //////////////////////////////////////////////////
        // KTR REPORT
        //////////////////////////////////////////////////

        private void GenerateRatioReport()
        {
            if (LabStorage.KtrSnapshots == null
                ||
                LabStorage.KtrSnapshots.Count == 0)
            {
                MessageBox.Show(
                    "Нет данных КТР");

                return;
            }

            PassportModel p =
                LabStorage.Passport;

            RatioRealtimeData last = LabStorage.KtrSnapshots[ LabStorage.KtrSnapshots.Count - 1];

            StringBuilder sb =
                new StringBuilder();

            BeginHtml(sb);

            //------------------------------------------------
            // TITLE
            //------------------------------------------------

            AddMainTitle(
                sb,
                "ПРОТОКОЛ ИСПЫТАНИЯ");

            AddSubTitle(
                sb,
                "КОЭФФИЦИЕНТ ТРАНСФОРМАЦИИ");

            //------------------------------------------------
            // PASSPORT
            //------------------------------------------------

            AddPassportTable(
                sb,
                p);

            //------------------------------------------------
            // RESULTS
            //------------------------------------------------

            AddSectionTitle(
                sb,
                "Результаты измерений");

            sb.Append("<table>");

            sb.Append(@"

<tr>

<th>№</th>

<th>ВН AB</th>
<th>ВН BC</th>
<th>ВН CA</th>
<th>ВН AVG</th>

<th>НН ab</th>
<th>НН bc</th>
<th>НН ca</th>
<th>НН avg</th>

<th>Kab</th>
<th>Kbc</th>
<th>Kca</th>

<th>Kavg</th>

<th>Δ%</th>

<th>IEC%</th>

</tr>

");

            for (int i = 0;
                i < LabStorage.KtrSnapshots.Count;
                i++)
            {
                var s =
                    LabStorage.KtrSnapshots[i];

                sb.Append("<tr>");

                //------------------------------------------------
                // NUMBER
                //------------------------------------------------

                AddCell(
                    sb,
                    (i + 1).ToString());

                //------------------------------------------------
                // HV
                //------------------------------------------------

                AddCell(
                    sb,
                    s.HvAB.ToString("F1"));

                AddCell(
                    sb,
                    s.HvBC.ToString("F1"));

                AddCell(
                    sb,
                    s.HvCA.ToString("F1"));

                AddCell(
                    sb,
                    s.HvAVG.ToString("F1"));

                //------------------------------------------------
                // LV
                //------------------------------------------------

                AddCell(
                    sb,
                    s.LvAB.ToString("F3"));

                AddCell(
                    sb,
                    s.LvBC.ToString("F3"));

                AddCell(
                    sb,
                    s.LvCA.ToString("F3"));

                AddCell(
                    sb,
                    s.LvAVG.ToString("F3"));

                //------------------------------------------------
                // K
                //------------------------------------------------

                AddCell(
                    sb,
                    s.KAB.ToString("F3"));

                AddCell(
                    sb,
                    s.KBC.ToString("F3"));

                AddCell(
                    sb,
                    s.KCA.ToString("F3"));

                AddCell(
                    sb,
                    s.KAVG.ToString("F3"));

                //------------------------------------------------
                // DEV
                //------------------------------------------------

                AddCell(
                    sb,
                    s.Dev.ToString("F2"));

                //------------------------------------------------
                // IEC
                //------------------------------------------------

                AddCell(
                    sb,
                    s.Err.ToString("F2"));

                sb.Append("</tr>");
            }

            sb.Append("</table>");

            //------------------------------------------------
            // CALC
            //------------------------------------------------

            AddSectionTitle(
                sb,
                "Расчетные параметры");

            sb.Append("<table>");

            AddRow(
                sb,
                "Kab",
                last.KAB.ToString("F3"));

            AddRow(
                sb,
                "Kbc",
                last.KBC.ToString("F3"));

            AddRow(
                sb,
                "Kca",
                last.KCA.ToString("F3"));

            AddRow(
                sb,
                "Kavg",
                last.KAVG.ToString("F3"));

            AddRow(
                sb,
                "Максимальное отклонение %",
                last.Dev.ToString("F2"));

            AddRow(
                sb,
                "IEC ошибка %",
                last.Err.ToString("F2"));

            AddRow(
                sb,
                "Ответвление ВН %",
                last.HvPercent.ToString("F2"));

            AddRow(
                sb,
                "Ответвление НН %",
                last.LvPercent.ToString("F2"));

            sb.Append("</table>");

            //------------------------------------------------
            // IEC
            //------------------------------------------------

            AddSectionTitle(
                sb,
                "Норматив");

            sb.Append("<table>");

            AddRow(
                sb,
                "Допустимое отклонение IEC",
                "±0.5 %");

            AddRow(
                sb,
                "Измеренное отклонение",
                last.Err.ToString("F2") + " %");

            sb.Append("</table>");

            //------------------------------------------------
            // RESULT
            //------------------------------------------------

            AddConclusion(
                sb,
                last.Passed);

            //------------------------------------------------
            // NOTE
            //------------------------------------------------

            AddSectionTitle(
                sb,
                "Примечание");

            sb.Append(@"

<div style='margin-top:15px;
font-size:15px;
line-height:24px;'>

Испытание коэффициента трансформации
выполнено в соответствии
с IEC 60076.

</div>

");

            EndHtml(sb);

            browser.DocumentText =
                sb.ToString();
        }


      

        //////////////////////////////////////////////////
        // HTML START
        //////////////////////////////////////////////////

        private void BeginHtml(
            StringBuilder sb)
        {
            sb.Append(@"

<html>

<head>

<meta charset='utf-8'>

<style>

body
{
    font-family: Arial;
    margin: 30px;
    color:#222;
}

h1,h2,h3
{
    text-align:center;
}

table
{
    width:100%;
    border-collapse:collapse;
    margin-top:20px;
}

th
{
    background:#EAEAEA;
}

th,td
{
    border:1px solid #555;
    padding:8px;
    text-align:center;
}

.title
{
    font-size:26px;
    font-weight:bold;
    margin-top:10px;
}

.subtitle
{
    font-size:20px;
    margin-top:10px;
    margin-bottom:20px;
}

.section
{
    font-size:18px;
    font-weight:bold;
    margin-top:25px;
}

.ok
{
    color:green;
    font-size:24px;
    font-weight:bold;
    text-align:center;
    margin-top:30px;
}

.fail
{
    color:red;
    font-size:24px;
    font-weight:bold;
    text-align:center;
    margin-top:30px;
}

</style>

</head>

<body>
");
        }

        //////////////////////////////////////////////////
        // HTML END
        //////////////////////////////////////////////////

        private void EndHtml(
            StringBuilder sb)
        {
            sb.Append("</body>");

            sb.Append("</html>");
        }

        //////////////////////////////////////////////////
        // TITLES
        //////////////////////////////////////////////////

        private void AddMainTitle(
            StringBuilder sb,
            string text)
        {
            sb.Append(
                "<div class='title'>" +
                text +
                "</div>");
        }

        private void AddSubTitle(
            StringBuilder sb,
            string text)
        {
            sb.Append(
                "<div class='subtitle'>" +
                text +
                "</div>");
        }

        private void AddSectionTitle(
            StringBuilder sb,
            string text)
        {
            sb.Append(
                "<div class='section'>" +
                text +
                "</div>");
        }

        //////////////////////////////////////////////////
        // PASSPORT
        //////////////////////////////////////////////////

        private void AddPassportTable(
            StringBuilder sb,
            PassportModel p)
        {
            sb.Append("<table>");

            AddRow(
                sb,
                "Заказчик",
                p.Customer);

            AddRow(
                sb,
                "Объект",
                p.ObjectName);

            AddRow(
                sb,
                "Тип",
                p.Type);

            AddRow(
                sb,
                "Завод",
                p.Factory);

            AddRow(
                sb,
                "Серийный номер",
                p.Serial);

            AddRow(
                sb,
                "Год",
                p.Year.ToString());

            AddRow(
                sb,
                "Мощность",
                p.PowerKva + " кВА");

            AddRow(
                sb,
                "Частота",
                p.Frequency + " Гц");

            AddRow(
                sb,
                "ВН",
                p.HVVoltage + " кВ");

            AddRow(
                sb,
                "НН",
                p.LVVoltage + " кВ");

            AddRow(
                sb,
                "Группа",
                p.VectorGroup);

            AddRow(
                sb,
                "Охлаждение",
                p.Cooling);

            AddRow(
                sb,
                "Инженер",
                p.Engineer);

            AddRow(
                sb,
                "Дата",
                p.TestDate.ToString(
                    "dd.MM.yyyy"));

            sb.Append("</table>");
        }

        //////////////////////////////////////////////////
        // RESULT
        //////////////////////////////////////////////////

        private void AddConclusion(
            StringBuilder sb,
            bool passed)
        {
            AddSectionTitle(
                sb,
                "Заключение");

            if (passed)
            {
                sb.Append(
                    "<div class='ok'>" +
                    "СООТВЕТСТВУЕТ" +
                    "</div>");
            }
            else
            {
                sb.Append(
                    "<div class='fail'>" +
                    "НЕ СООТВЕТСТВУЕТ" +
                    "</div>");
            }
        }

        //////////////////////////////////////////////////
        // TABLE
        //////////////////////////////////////////////////

        private void AddRow(
            StringBuilder sb,
            string name,
            string value)
        {
            sb.Append("<tr>");

            sb.Append(
                "<td>" +
                name +
                "</td>");

            sb.Append(
                "<td>" +
                value +
                "</td>");

            sb.Append("</tr>");
        }

        private void AddCell(
            StringBuilder sb,
            string value)
        {
            sb.Append(
                "<td>" +
                value +
                "</td>");
        }
    }
}
