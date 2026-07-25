using System;
using System.Linq;
using System.Text;

using ElstanLab.Models;

namespace ElstanLab.Pages.ReportPage.Reports
{
    public class RatioReportBuilder
        : BaseReportBuilder
    {

        ///////////////////////////////////////////////////////////////
        // BUILD
        ///////////////////////////////////////////////////////////////

        public override string Build()
        {
            PassportModel p = LabStorage.Passport;

            RatioRealtimeData s = LabStorage.KtrSnapshots[LabStorage.CurrentKtr.rowcheckid];

            LabSettings l = LabStorage.labsett;
            
            StringBuilder sb = new StringBuilder();

            BeginHtml(sb);

            BuildStyle(sb);

            sb.AppendLine(@"
<body>

<div class='protocolNumber'>
ПРОТОКОЛ № _______
</div>

<div class='protocolTitle'>
<h1>
КОЭФФИЦИЕНТ ТРАНСФОРМАЦИИ
</h1>
</div>
");

            
            BuildTopTables(sb, p);

            BuildSnapshotsTable(sb);

            BuildConclusion(sb,p);

            sb.AppendLine(@"
</body>
</html>
");

            return sb.ToString();
        }

        ///////////////////////////////////////////////////////////////
        // STYLE
        ///////////////////////////////////////////////////////////////


        private void BuildStyle(StringBuilder sb)
        {
            sb.AppendLine(@"

<style>

@page
{
    size: A4;
    margin: 15mm;
}

body
{
    font-family: Arial;
    font-size: 12px;
    color: #222;
    margin: 0;
    padding: 0;
}

h1
{
    text-align: center;
    font-size: 20px;
    margin: 0;
    padding: 0;
}

table
{
    width: 100%;
    border-collapse: collapse;
    margin-top: 8px;
    margin-bottom: 18px;
}

th
{
    background: #e8edf5;
    border: 1px solid #666;
    padding: 6px;
    text-align: center;
    font-weight: bold;
}

td
{
    border: 1px solid #888;
    padding: 6px;
    vertical-align: middle;
}

caption
{
    caption-side: top;
    text-align: left;
    font-size: 16px;
    font-weight: bold;

    margin-bottom: 8px;

    border-bottom: 2px solid #444;
    padding-bottom: 4px;
}

.divcaption
{
    caption-side: top;
    text-align: left;
    font-size: 20px;
    font-weight: bold;

    margin-bottom: 8px;

    border-bottom: 2px solid #444;
    padding-bottom: 4px;
}

.center
{
    text-align: center;
}

.right
{
    text-align: right;
}

.pass
{
    color: #0A8A0A;
    font-weight: bold;
}

.fail
{
    color: #D00000;
    font-weight: bold;
}

.resultBox
{
    border: 2px solid #4CAF50;
    background: #F1FFF1;
    padding: 18px;
    margin-top: 15px;

    font-size: 16px;
    font-weight: bold;

    color: #1D6F1D;

    text-align: center;
}

.resultBoxFail
{
    border: 2px solid #D00000;
    background: #FFF2F2;
    color: #B00000;
}

.footer
{
    margin-top: 45px;
}

.flexRow
{
    display: flex;
    gap: 20px;
    align-items: flex-start;
}

.flexCol
{
    flex: 1;    
}

.protocolTitle
{
    margin-top: 10px;
    margin-bottom: 20px;
}

.protocolNumber
{
    font-size: 18px;
    font-weight: bold;
    text-align: center;
    margin-bottom: 10px;
}

.signTable td
{
    border: 0;
    padding-top: 30px;
}

.compact td
{
    padding: 5px;
}

.compact th
{
    padding: 5px;
}

.compact caption {
    font-size: 20px;
    font-weight: bold;
    text-align: left;
    padding-bottom: 5px;
}

</style>

");
        }


        private void BuildTopTables(StringBuilder sb, PassportModel p)
        {
            ///////////////////////////////////////////////////////////////
            // Passport
            ///////////////////////////////////////////////////////////////
            sb.AppendLine(@"
               <div class='flexRow'>
               <div class='flexCol'>
               <table class='compact'>
               <caption >1. Паспортные данные</caption>
               ");
            AddRow4(sb, "Дата испытания", DateTime.Now.ToString("dd.MM.yyyy"), "Напряжение ВН, кВ", p.HVVoltage.ToString("F3"));
            AddRow4(sb, "Производитель тр-ра", p.Factory, "Ток ВН, А", p.IHV.ToString("F1"));
            AddRow4(sb, "Тип трансформатора", p.Type, "Напряжение НН, кВ", p.LVVoltage.ToString("F3"));
            AddRow4(sb, "Заводской номер", p.Serial, "Ток НН, А", p.ILV.ToString("F1"));
            AddRow4(sb, "Номинальная мощность, кВА", p.PowerKva.ToString("F0"), "Охлаждение", p.Cooling);
            AddRow4(sb, "Частота, Гц", "50", "Напряжение Uкз, %", p.UkPercent.ToString("F1"));
            AddRow4(sb, "Группа соединения", p.VectorGroup, "Ток ХХ паспорт, %", p.I0Percent.ToString("F2"));

            sb.AppendLine(@"
            </table>
            </div>
            </div>
            ");
        }
        
        ///////////////////////////////////////////////////////////////
        // SNAPSHOTS TABLE
        ///////////////////////////////////////////////////////////////

        private void BuildSnapshotsTable(StringBuilder sb)
        {

            sb.AppendLine(@"

               <div class='flexRow'>
               <div class='flexCol'>
               <table class='compact'>
               <caption> 2. Измеренные данные </caption>

<thead>

<tr>

<th rowspan='3'>№</th>
<th rowspan='3'>ВН %</th>
<th rowspan='3'>НН %</th>

<th colspan='3'>AB – ab</th>
<th colspan='3'>BC – bc</th>
<th colspan='3'>CA – ca</th>

<th rowspan='3'>ΔKabc</th>
<th rowspan='3'>Ктр</th>
<th rowspan='3'>Результат</th>

</tr>

<tr>

<th colspan='2'>Напряжение</th>
<th rowspan='2'>Ктр</th>

<th colspan='2'>Напряжение</th>
<th rowspan='2'>Ктр</th>

<th colspan='2'>Напряжение</th>
<th rowspan='2'>Ктр</th>

</tr>

<tr>

<th>AB</th>
<th>ab</th>

<th>BC</th>
<th>bc</th>

<th>CA</th>
<th>ca</th>

</tr>

</thead>

<tbody>

");

            int i = 1;

            foreach (RatioRealtimeData s in LabStorage.KtrSnapshots)
            {
                string result = s.Passed ? "PASS" : "FAIL";
                string cls = s.Passed ? "pass" : "fail";

                sb.AppendLine($@"

<tr class='center'>

<td>{i}</td>

<td>{s.HvPercent:F1}</td>
<td>{s.LvPercent:F1}</td>

<td>{s.HvAB:F1}</td>
<td>{s.LvAB:F2}</td>
<td>{s.KAB:F3}</td>

<td>{s.HvBC:F1}</td>
<td>{s.LvBC:F2}</td>
<td>{s.KBC:F3}</td>

<td>{s.HvCA:F1}</td>
<td>{s.LvCA:F2}</td>
<td>{s.KCA:F3}</td>

<td>{s.Dev:F2}</td>

<td>{s.Err:F3}</td>

<td class='{cls}'>{result}</td>

</tr>

");

                i++;
            }

            sb.AppendLine(@"

</tbody>

</table>
</div>
</div>

");
        }



        ///////////////////////////////////////////////////////////////
        // CONCLUSION
        ///////////////////////////////////////////////////////////////
        private void BuildConclusion(StringBuilder sb, PassportModel p)
        {
            bool hasTaps =
                LabStorage.Passport.HVTapCount > 1 ||
                LabStorage.Passport.LVTapCount > 1;

            if (hasTaps)
                BuildConclusionWithTaps(sb,  p);
            else
                BuildConclusionSingle(sb,  p);
        }

        private void BuildConclusionSingle(StringBuilder sb, PassportModel p)
        {
            RatioRealtimeData s = LabStorage.KtrSnapshots[0];

            bool pass = s.Passed;

            string cls = pass
                ? "resultBox"
                : "resultBox resultBoxFail";

            StringBuilder text = new StringBuilder();

            if (pass)
            {
                text.Append(
                    $"Проверка коэффициента трансформации выполнена на номинальном положении трансформатора.");

                text.Append("<br><br>");

                text.Append(
                    $"Измеренный коэффициент трансформации составил <b>{s.KAVG:F3}</b>.");

                text.Append("<br><br>");

                text.Append(
                    $"Отклонение составило <b>{s.Err:F3}%</b>, что не превышает допустимое значение <b>±{LabStorage.labsett.RatioDeviation:F3}%</b>.");

                text.Append("<br><br>");

                text.Append(
                    "Измеренные значения соответствуют паспортным данным.");

                text.Append("<br><br>");

                text.Append("<b>Трансформатор испытание выдержал.</b>");
            }
            else
            {
                text.Append(
                    $"Проверка коэффициента трансформации выполнена на номинальном положении трансформатора.");

                text.Append("<br><br>");

                text.Append(
                    $"Измеренный коэффициент трансформации составил <b>{s.KAVG:F3}</b>.");

                text.Append("<br><br>");

                text.Append(
                    $"Отклонение составило <b>{s.Err:F3}%</b>, что превышает допустимое значение <b>±{LabStorage.labsett.RatioDeviation:F3}%</b>.");

                text.Append("<br><br>");

                text.Append(
                    "Измеренные значения не соответствуют паспортным данным.");

                text.Append("<br><br>");

                text.Append("<b>Трансформатор испытание не выдержал.</b>");
            }

            sb.AppendLine("<div class='divcaption'> 4.Заключение </ div > ");

            sb.AppendLine($@"
<div class='{cls}'>
{text}
</div>
");

            BuildFooter(sb,p);
        }

        private void BuildConclusionWithTaps(StringBuilder sb, PassportModel p)
        {
            var list = LabStorage.KtrSnapshots;

            int total = list.Count;

            int passed = list.Count(x => x.Passed);

            int failed = total - passed;

            double maxError =
                list.Max(x => Math.Abs(x.Err));

            double minTap =
                list.Min(x => x.HvPercent);

            double maxTap =
                list.Max(x => x.HvPercent);

            StringBuilder text = new StringBuilder();

            if (failed == 0)
            {
                text.Append(
                    $"Проверка коэффициента трансформации выполнена на <b>{total}</b> положениях переключателя ответвлений.");

                text.Append("<br><br>");

                text.Append(
                    $"Диапазон проверки составил от <b>{minTap:+0.0;-0.0;0}%</b> до <b>{maxTap:+0.0;-0.0;0}%</b>.");

                text.Append("<br><br>");

                text.Append(
                    $"Максимальное отклонение коэффициента трансформации составило <b>{maxError:F3}%</b>, что не превышает допустимое значение <b>±{LabStorage.labsett.RatioDeviation:F3}%</b>.");

                text.Append("<br><br>");

                text.Append(
                    "Все положения переключателя соответствуют паспортным данным.");

                text.Append("<br><br>");

                text.Append("<b>Трансформатор испытание выдержал.</b>");
            }
            else
            {
                text.Append(
                    $"Проверка коэффициента трансформации выполнена на <b>{total}</b> положениях переключателя ответвлений.");

                text.Append("<br><br>");

                text.Append(
                    $"Соответствуют требованиям: <b>{passed}</b>.");

                text.Append("<br>");

                text.Append(
                    $"Не соответствуют требованиям: <b>{failed}</b>.");

                text.Append("<br><br>");

                text.Append(
                    $"Максимальное отклонение составило <b>{maxError:F3}%</b> при допустимом значении <b>±{LabStorage.labsett.RatioDeviation:F3}%</b>.");

                text.Append("<br><br>");

                text.Append(
                    "Несоответствие выявлено на следующих положениях переключателя:");

                text.Append("<br><br>");

                foreach (var s in list.Where(x => !x.Passed))
                {
                    text.Append(
                        $"• ВН {s.HvPercent:+0.0;-0.0;0}%");

                    if (LabStorage.Passport.LVTapCount > 1)
                        text.Append($" / НН {s.LvPercent:+0.0;-0.0;0}%");

                    text.Append(
                        $" — отклонение {s.Err:F3}%");

                    text.Append("<br>");
                }

                text.Append("<br>");

                text.Append(
                    "<b>Трансформатор паспортным данным не соответствует.</b>");
            }

            string cls =
                failed == 0
                ? "resultBox"
                : "resultBox resultBoxFail";

            sb.AppendLine("<div class='divcaption'> 4.Заключение </ div > ");

            sb.AppendLine($@"
<div class='{cls}'>
{text}
</div>
");

            BuildFooter(sb,p);
        }

        private void BuildFooter(StringBuilder sb, PassportModel p)
        {
            sb.AppendLine($@"

<div class='footer'>

<table class='signTable'>

<tr>

<td>
Испытатель: ______________{p.Engineer}
</td>

<td>
Проверил ________________________
</td>

</tr>

</table>

</div>

");
        }


        private void BuildConclusion2(StringBuilder sb)
        {
            var list = LabStorage.KtrSnapshots;

            if (list.Count == 0)
                return;


            int total = list.Count;

            int passed = list.Count(x => x.Passed);

            int failed = total - passed;

            double maxDeviation =
                list.Max(x => Math.Abs(x.Err));

            double minTap =
                list.Min(x => x.HvPercent);

            double maxTap =
                list.Max(x => x.HvPercent);

            StringBuilder text = new StringBuilder();

            //////////////////////////////////////////////////
            // GOOD
            //////////////////////////////////////////////////

            if (failed == 0)
            {
                text.Append(
                    $"Проверка коэффициента трансформации выполнена на {total} положениях переключателя ответвлений ");

                text.Append(
                    $"в диапазоне от {minTap:F1}% до {maxTap:F1}%.");

                text.Append("<br><br>");

                text.Append(
                    $"Максимальное отклонение коэффициента трансформации составило {maxDeviation:F3}% ");

                text.Append(
                    $"при допустимом значении ±{LabStorage.labsett.RatioDeviation:F3}%.");

                text.Append("<br><br>");

                text.Append(
                    "Все измеренные значения соответствуют паспортным данным и требованиям IEC 60076-1.");

                text.Append("<br><br>");

                text.Append(
                    "<b>Трансформатор испытание выдержал.</b>");
            }

            //////////////////////////////////////////////////
            // FAIL
            //////////////////////////////////////////////////

            else
            {
                text.Append(
                    $"Проверка коэффициента трансформации выполнена на {total} положениях переключателя ответвлений.");

                text.Append("<br><br>");

                text.Append(
                    $"Соответствуют требованиям: {passed}.<br>");

                text.Append(
                    $"Не соответствуют требованиям: {failed}.");

                text.Append("<br><br>");

                text.Append(
                    $"Максимальное отклонение составило {maxDeviation:F3}% ");

                text.Append(
                    $"при допустимом значении ±{LabStorage.labsett.RatioDeviation:F3}%.");

                text.Append("<br><br>");

                text.Append("Несоответствие обнаружено на следующих ответвлениях:");

                text.Append("<br><br>");

                foreach (var s in list.Where(x => !x.Passed))
                {
                    text.Append(
                        $"• ВН {s.HvPercent:+0.0;-0.0;0}%");

                    text.Append(
                        $" / НН {s.LvPercent:+0.0;-0.0;0}%");

                    text.Append(
                        $" — отклонение {s.Err:F3}%<br>");
                }

                text.Append("<br>");

                text.Append(
                    "<b>Трансформатор не соответствует требованиям IEC 60076-1.</b>");
            }

            string cls =
                failed == 0
                ? "resultBox"
                : "resultBox resultBoxFail";

            sb.AppendLine("<h2>3. Заключение</h2>");

            sb.AppendLine($@"

<div class='{cls}'>

{text}

</div>

");

            //////////////////////////////////////////////////
            // SIGNATURE
            //////////////////////////////////////////////////

            sb.AppendLine(@"

<div class='footer'>

<table class='signTable'>

<tr>

<td>
Испытатель ________________________
</td>

<td>
Проверил ________________________
</td>

</tr>

</table>

</div>

");
        }

        private void BuildConclusion1(
            StringBuilder sb,
            RatioRealtimeData s)
        {
            sb.AppendLine("<h2>8. Заключение</h2>");

            string text =
                s.Passed
                ? @"В результате проверки коэффициента трансформации установлено,
что измеренные значения соответствуют паспортным данным
и требованиям IEC 60076-1.

Трансформатор признан годным к эксплуатации."
                : @"Выявлено отклонение коэффициента трансформации,
превышающее допустимые нормы IEC 60076-1.

Трансформатор требует дополнительной диагностики.";

            string cls =
                s.Passed
                ? "resultBox"
                : "resultBox fail";

            sb.AppendLine($@"

<div class='{cls}'>

{text}

</div>

");

            sb.AppendLine(@"

<div class='footer'>

<table style='border:0;'>

<tr style='border:0;'>

<td style='border:0;'>
Испытатель: ___________________
</td>

<td style='border:0;'>
Проверил: ___________________
</td>

</tr>

</table>

<div class='small'>
Протокол сформирован автоматически системой ElstanLab
</div>

</div>

");
        }

   
     
    }
}

