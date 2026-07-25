using System;
using System.Text;

using ElstanLab.Models;

namespace ElstanLab.Pages.ReportPage.Reports
{
    public abstract class BaseReportBuilder
    {
//////////////////////////////////////////////////
// BUILD
//////////////////////////////////////////////////


    public abstract string Build();

        //////////////////////////////////////////////////
        // HTML
        //////////////////////////////////////////////////

        protected void BeginHtml(StringBuilder sb)
        {
            sb.Append(ReportStyles.CSS);

            sb.Append("<body>");

            sb.Append("<div class='page'>");
        }

        protected void EndHtml(StringBuilder sb)
        {
            sb.Append("</div>");

            sb.Append("</body>");

            sb.Append("</html>");
        }

        //////////////////////////////////////////////////
        // HEADER
        //////////////////////////////////////////////////

        protected void AddProtocolHeader(
            StringBuilder sb,
            PassportModel p,
            string title)
        {
            sb.Append(@"
```

<div class='protocol-header'>

<div class='company'>
ELSTANLAB
</div>

<div class='protocol-title'>
ПРОТОКОЛ ИСПЫТАНИЯ
</div>

<div class='protocol-subtitle'>
" + title + @"
</div>

<table class='info-table'>

<tr>

<td><b>Организация</b></td>
<td>ELSTANLAB</td>

<td><b>Протокол №</b></td>
<td>" + DateTime.Now.ToString("yyyyMMddHHmm") + @"</td>

</tr>

<tr>

<td><b>Дата</b></td>
<td>" + p.TestDate.ToString("dd.MM.yyyy") + @"</td>

<td><b>Оператор</b></td>
<td>" + p.Engineer + @"</td>

</tr>

<tr>

<td><b>Заказчик</b></td>
<td>" + p.Customer + @"</td>

<td><b>Серийный №</b></td>
<td>" + p.Serial + @"</td>

</tr>

</table>

</div>

");
        }

    //////////////////////////////////////////////////
    // SECTION
    //////////////////////////////////////////////////

    protected void AddSectionTitle(
        StringBuilder sb,
        string text)
        {
            sb.Append(
                "<div class='section-title'>" +
                text +
                "</div>");
        }

        //////////////////////////////////////////////////
        // PASSPORT
        //////////////////////////////////////////////////

        protected void AddPassport(
            StringBuilder sb,
            PassportModel p)
        {
            AddSectionTitle(sb, "Паспорт");

            sb.Append("<table>");

            AddRow(sb, "Тип", p.Type);

            AddRow(sb, "Завод", p.Factory);

            AddRow(sb, "Серийный номер", p.Serial);

            AddRow(sb, "Год", p.Year.ToString());

            AddRow(sb, "Мощность", p.PowerKva + " кВА");

            AddRow(sb, "Частота", p.Frequency + " Гц");

            AddRow(sb, "Группа", p.VectorGroup);

            AddRow(sb, "Охлаждение", p.Cooling);

            AddRow(sb, "ВН", p.HVVoltage + " кВ");

            AddRow(sb, "НН", p.LVVoltage + " кВ");

            AddRow(sb, "Uk", p.UkPercent + " %");

            AddRow(sb, "Pk", p.PkLoss + " Вт");

            AddRow(sb, "P0", p.P0Loss + " Вт");

            AddRow(sb, "I0", p.I0Percent + " %");

            sb.Append("</table>");
        }

        //////////////////////////////////////////////////
        // TABLE
        //////////////////////////////////////////////////

        protected void AddRow(
            StringBuilder sb,
            string name,
            string value)
        {
            sb.Append("<tr>");

            sb.Append("<td>");
            sb.Append(name);
            sb.Append("</td>");

            sb.Append("<td>");
            sb.Append(value);
            sb.Append("</td>");

            sb.Append("</tr>");
        }
        
        protected void AddRow4(StringBuilder sb, string name, string value, string name2, string value2)
        {
            sb.Append("<tr>");

            sb.Append("<td>");
            sb.Append(name);
            sb.Append("</td>");

            sb.Append("<td>");
            sb.Append(value);
            sb.Append("</td>");

            sb.Append("<td>");
            sb.Append(name2);
            sb.Append("</td>");

            sb.Append("<td>");
            sb.Append(value2);
            sb.Append("</td>");

            sb.Append("</tr>");
        }

        protected void AddCell(
            StringBuilder sb,
            string value)
        {
            sb.Append("<td>");

            sb.Append(value);

            sb.Append("</td>");
        }

        //////////////////////////////////////////////////
        // STATUS
        //////////////////////////////////////////////////

        protected string GetStatusClass(
            bool pass)
        {
            return pass ? "pass" : "fail";
        }

        //////////////////////////////////////////////////
        // FOOTER
        //////////////////////////////////////////////////

        protected void AddFooter(
            StringBuilder sb)
        {
            sb.Append(@"
```

<div class='footer'>

<table class='sign-table'>

<tr>

<td>
Испытание выполнил:
________________
</td>

<td>
Ответственный:
________________
</td>

</tr>

</table>

</div>

");
        }
    }
}
