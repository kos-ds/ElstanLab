
using System;
using System.Text;

using ElstanLab.Models;

namespace ElstanLab.Pages.ReportPage.Reports
{
    public class NoLoadReportBuilder
        : BaseReportBuilder
    {

        ///////////////////////////////////////////////////////////////
        // BUILD
        ///////////////////////////////////////////////////////////////

        public override string Build()
        {
            PassportModel p = LabStorage.Passport;

            NoLoadSnapshot s = LabStorage.NoLoadSnapshots[LabStorage.CurrentNoLoad.rowcheckid];

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
ПОТЕРИ И ТОК ХОЛОСТОГО ХОДА
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
        // TOP TABLES
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

        private void BuildMeasuredData( StringBuilder sb, NoLoadSnapshot s)
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
<th>Cosφ, </th>
</tr>
");

sb.AppendLine($@"
<tr class='center'>
<td>{s.Pa:F1}</td>
<td>{s.Pb:F1}</td>
<td>{s.Pc:F1}</td>
<td>{s.Ptotal:F1}</td>
<td>{s.CosPhi:F3}</td>
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
            NoLoadSnapshot s,
            LabSettings l)
        {

            sb.AppendLine(@"
<table class='compact'>

<caption> 3. Проверка соответствия паспортным данным </caption>

<tr>
<th>Параметр</th>
<th>Измерено</th>
<th>Паспорт</th>
<th>Норма, %</th>
<th>Результат, %</th>
</tr>
");
            string cls = s.P0Otklon <= l.NoLoadP0Deviation ? "pass" : "fail";
            sb.AppendLine($@"
<tr class='center'>
<td> Потери ХХ </td>
<td>{s.Ptotal:F1}</td>
<td>{p.P0Loss:F1}</td>
<td>{l.NoLoadP0Deviation:F1}</td>
<td class='center {cls}'>{s.P0Otklon:F1}</td>
</tr>
");

            string cls1 = s.I0Otklon <= l.NoLoadI0Deviation ? "pass" : "fail";
            sb.AppendLine($@"
<tr class='center'>
<td> Ток ХХ </td>
<td>{s.I0:F1}</td>
<td>{p.I0Percent:F1}</td>
<td>{l.NoLoadI0Deviation:F1}</td>
<td class='center {cls1}'>{s.I0Otklon:F1}</td>
</tr>
");

            sb.AppendLine("</table>");

            string text = (s.P0Otklon <= l.NoLoadP0Deviation) &&(s.I0Otklon <= l.NoLoadI0Deviation)
                    ? "соответствуют"
                    : "не соответствуют";

            string cls2 = (s.P0Otklon <= l.NoLoadP0Deviation) && (s.I0Otklon <= l.NoLoadI0Deviation)
                ? "resultBox"
                : "resultBox resultBoxFail";

            sb.AppendLine(@"
<div class='divcaption'>
4. Заключение
</div>
");

            sb.AppendLine($@"
<div class='{cls2}'>
По результатам испытания трансформатора измеренный ток холостого хода составил I₀ = {s.I0.ToString("F1")}% при паспортном значении {p.I0Percent.ToString("F1")}%, потери холостого хода составили P₀ = {s.Ptotal.ToString("F0")} Вт при паспортном значении {p.P0Loss.ToString("F0")} Вт. 
<br>
Измеренные значения {text} требованиям нормативной документации и паспортным данным.
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
");

        }

      
    }
}

