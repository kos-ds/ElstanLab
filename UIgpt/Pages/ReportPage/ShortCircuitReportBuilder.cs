using System;
using System.Text;

using ElstanLab.Models;

namespace ElstanLab.Pages.ReportPage.Reports
{
    public class ShortCircuitReportBuilder : BaseReportBuilder
    {

        ///////////////////////////////////////////////////////////////
        // BUILD
        ///////////////////////////////////////////////////////////////
        public override string Build()
        {
            PassportModel p = LabStorage.Passport;

            ShortCircuitSnapshot s = LabStorage.KzSnapshots[LabStorage.CurrentKz.rowcheckid];

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
СОПРОТИВЛЕНИЕ И ПОТЕРИ КОРОТКОГО ЗАМЫКАНИЯ
</h1>
</div>
");

            BuildTopTables(sb, p);

            BuildMeasuredData(sb, s);

            BuildMiddleTables(sb, p, s, l);

            //BuildConclusion(sb, s);

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

.noBreak
{
    page-break-inside: avoid;
    break-inside: avoid;
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
        ///////////////////////////////////////////////////////////////
        // HEADER
        ///////////////////////////////////////////////////////////////

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
        // MEASURED
        ///////////////////////////////////////////////////////////////

        private void BuildMeasuredData(StringBuilder sb, ShortCircuitSnapshot s)
        {
            sb.AppendLine(@"
<table class='compact'>

<caption> 2. Измеренные данные </caption>

<tr>
<th>UAB, В</th>
<th>UBC, В</th>
<th>UCA, В</th>
<th>UAvg, В</th>
<th>ΔU, %</th>
</tr>
");

            sb.AppendLine($@"
<tr class='center'>
<td>{s.Uab:F1}</td>
<td>{s.Ubc:F1}</td>
<td>{s.Uca:F1}</td>
<td>{s.Uavg:F1}</td>
<td>{s.deltaU:F1}</td>
</tr>
");

            sb.AppendLine(@"
<tr>
<th>IAB, A</th>
<th>IBC, A</th>
<th>ICA, A</th>
<th>IAvg, A</th>
<th>ΔI, %</th>
</tr>
");

            sb.AppendLine($@"
<tr class='center'>
<td>{s.Ia:F1}</td>
<td>{s.Ib:F1}</td>
<td>{s.Ic:F1}</td>
<td>{s.Iavg:F1}</td>
<td>{s.deltaI:F1}</td>
</tr>
");

            sb.AppendLine(@"
<tr>
<th>PAB, Вт</th>
<th>PBC, Вт</th>
<th>PCA, Вт</th>
<th>PΣ, Вт</th>
</tr>
");

            sb.AppendLine($@"
<tr class='center'>
<td>{s.Pa:F1}</td>
<td>{s.Pb:F1}</td>
<td>{s.Pc:F1}</td>
<td>{s.Ptotal:F1}</td>
</tr>
");

            sb.AppendLine("</table>");


            sb.AppendLine(@"
<table class='compact'>

<caption> 3. Расчетные параметры короткого замыкания </caption>

<tr>
<th>Uкз-ожид, В</th>
<th>Iном, А</th>
<th>Достигнутый ток, %</th>
<th>Uкз, %</th>
<th>Zk, Ом</th>
<th>Rk, Ом</th>
<th>Xk, Ом</th>
</tr>
");

            sb.AppendLine($@"
<tr class='center'>
<td>{s.ExpectedUkVoltage:F1}</td>
<td>{s.NominalCurrent:F1}</td>
<td>{s.CurrentPercent:F1}</td>
<td>{s.UkPercent:F1}</td>
<td>{s.Zk:F3}</td>
<td>{s.Rk:F3}</td>
<td>{s.Xk:F3}</td>
</tr>
");
            sb.AppendLine("</table>");

        }

        ///////////////////////////////////////////////////////////////
        // MIDDLE TABLES
        ///////////////////////////////////////////////////////////////
        private void BuildMiddleTables(
            StringBuilder sb,
            PassportModel p,
            ShortCircuitSnapshot s,
            LabSettings l)
        {

            sb.AppendLine(@"
<table class='compact'>

<caption> 4 . Проверка соответствия паспортным данным </caption>

<tr>
<th>Параметр</th>
<th>Приведенное</th>
<th>Паспорт</th>
<th>Норма, %</th>
<th>Результат, %</th>
</tr>
");
            string cls = s.UkOtklon <= l.ShortCircuitUkDeviation ? "pass" : "fail";
            sb.AppendLine($@"
<tr class='center'>
<td> Uk, % </td>
<td>{s.CorrectedUkPercent:F1}</td>
<td>{p.UkPercent:F1}</td>
<td>{l.ShortCircuitUkDeviation:F1}</td>
<td class='center {cls}'>{s.UkOtklon:F1}</td>
</tr>
");

            string cls1 = s.PkOtklon <= l.ShortCircuitPkDeviation ? "pass" : "fail";
            sb.AppendLine($@"
<tr class='center'>
<td> Pk, Вт </td>
<td>{s.CorrectedLosses:F1}</td>
<td>{p.PkLoss:F1}</td>
<td>{l.ShortCircuitPkDeviation:F1}</td>
<td class='center {cls1}'>{s.PkOtklon:F1}</td>
</tr>
");

            sb.AppendLine("</table>");

            string text = (s.UkOtklon <= l.ShortCircuitUkDeviation) && (s.PkOtklon <= l.ShortCircuitPkDeviation)
                    ? "соответствуют"
                    : "не соответствуют";

            string cls2 = (s.UkOtklon <= l.ShortCircuitUkDeviation) && (s.PkOtklon <= l.ShortCircuitPkDeviation)
                ? "resultBox"
                : "resultBox resultBoxFail";

            sb.AppendLine(@"

<div class='noBreak'>
<div class='divcaption'>
5. Заключение
</div>
");

            sb.AppendLine($@"
<div class='{cls2}'>
По результатам испытания короткого замыкания после приведения к нормируемой температуре получены следующие значения: напряжение короткого замыкания Uкз = {s.CorrectedUkPercent:F2}% (паспортное значение {p.UkPercent:F2}%), потери нагрузки Pк = {s.CorrectedLosses:F0} Вт (паспортное значение {p.PkLoss:F0} Вт). Отклонение по Uкз составило {s.UkOtklon:F2}%, по потерям нагрузки — {s.PkOtklon:F2}%.
<br>
Полученные результаты {text} требованиям нормативной документации и паспортным данным.
</div>
");

            sb.AppendLine($@"
<div class='footer'>

<table class='signTable'>

<tr>

<td>
Испытатель: ______________{p.Engineer}
</td>

<td>
Проверил: ___________________
</td>

</tr>

</table>
</div>
</div>
");

        }



       
    }
}

