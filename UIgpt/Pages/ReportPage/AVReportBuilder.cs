using ElstanLab.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ElstanLab.Pages.ReportPage.Reports
{
    public class AVReportBuilder : BaseReportBuilder
    {
        public override string Build()
        {
            PassportModel p = LabStorage.Passport;
            LabSettings l = LabStorage.labsett;

            // Берём выбранные пользователем испытания
            List<AVSnapshot> tests = LabStorage.AVSnapshotsForReport;
            if (tests == null || tests.Count == 0)
            {
                // запасной вариант — старое одиночное поле
                if (LabStorage.CurrentAV != null)
                    tests = new List<AVSnapshot> { LabStorage.CurrentAV };
                else
                    tests = new List<AVSnapshot>();
            }

            StringBuilder sb = new StringBuilder();
            BeginHtml(sb);
            BuildStyle(sb);

            sb.AppendLine(@"
<script src=""https://cdn.jsdelivr.net/npm/chart.js@4.4.1/dist/chart.umd.min.js""></script>
<body>
<div class='protocolNumber'>ПРОТОКОЛ № _______</div>
<div class='protocolTitle'>
    <h1>ИСПЫТАНИЕ ПРИЛОЖЕННЫМ НАПРЯЖЕНИЕМ</h1>
</div>");

            BuildTopTables(sb, p);

            if (tests.Count == 0)
            {
                sb.AppendLine("<p style='color:#c00; font-weight:bold;'>Нет выбранных результатов испытаний для отчёта.</p>");
            }
            else
            {
                // Общая сводная таблица
                BuildSummaryTable(sb, tests, l);

                // Подробные блоки по каждой обмотке
                int sectionNum = 3;
                foreach (var s in tests.OrderBy(t => t.Winding)) // сначала ВН, потом НН
                {
                    BuildOneTestSection(sb, p, s, l, sectionNum);
                    sectionNum += 3; // 3 раздела на одно испытание
                }

                BuildFinalConclusion(sb, p, tests, l);
            }

            sb.AppendLine("</body></html>");
            return sb.ToString();
        }

        //================================================
        // 1. Паспорт
        //================================================
        private void BuildTopTables(StringBuilder sb, PassportModel p)
        {
            sb.AppendLine(@"
<div class='flexRow'>
<div class='flexCol'>
<table class='compact'>
<caption>1. Паспортные данные трансформатора</caption>");

            AddRow4(sb, "Дата испытания", DateTime.Now.ToString("dd.MM.yyyy"), "Напряжение ВН, кВ", p.HVVoltage.ToString("F3"));
            AddRow4(sb, "Производитель", p.Factory ?? "", "Ток ВН, А", p.IHV.ToString("F1"));
            AddRow4(sb, "Тип трансформатора", p.Type ?? "", "Напряжение НН, кВ", p.LVVoltage.ToString("F3"));
            AddRow4(sb, "Заводской номер", p.Serial ?? "", "Ток НН, А", p.ILV.ToString("F1"));
            AddRow4(sb, "Номинальная мощность, кВА", p.PowerKva.ToString("F0"), "Охлаждение", p.Cooling ?? "");
            AddRow4(sb, "Частота, Гц", "50", "Группа соединения", p.VectorGroup ?? "");
            AddRow4(sb, "Год выпуска", p.Year.ToString(), "Испытатель", p.Engineer ?? "");

            sb.AppendLine("</table></div></div>");
        }

        //================================================
        // 2. Сводная таблица по всем выбранным испытаниям
        //================================================
        private void BuildSummaryTable(StringBuilder sb, List<AVSnapshot> tests, LabSettings l)
        {
            sb.AppendLine(@"
<table class='compact'>
<caption>2. Сводные результаты испытаний</caption>
<tr>
    <th>Обмотка</th>
    <th>Норма, кВ</th>
    <th>Uср, кВ</th>
    <th>% от нормы</th>
    <th>Iср, мА</th>
    <th>ΔU, %</th>
    <th>ΔI, %</th>
    <th>Результат</th>
</tr>");

            foreach (var s in tests.OrderBy(t => t.Winding))
            {
                double pct = s.RequiredU > 0.01 ? s.Uavg / s.RequiredU * 100.0 : 0;
                string cls = s.Passed ? "pass" : "fail";
                string res = s.Passed ? "ПРОЙДЕНО" : "НЕ ПРОЙДЕНО";

                sb.AppendLine($@"
<tr class='center'>
    <td><b>{s.Winding}</b></td>
    <td>{s.RequiredU:F1}</td>
    <td>{s.Uavg:F2}</td>
    <td>{pct:F1}</td>
    <td>{s.Iavg:F2}</td>
    <td>{s.MaxUDev:F2}</td>
    <td>{s.MaxIDev:F2}</td>
    <td class='{cls}'><b>{res}</b></td>
</tr>");
            }

            sb.AppendLine("</table>");
        }

        //================================================
        // Подробный блок по одному испытанию
        //================================================
        private void BuildOneTestSection(StringBuilder sb, PassportModel p, AVSnapshot s, LabSettings l, int startNum)
        {
            string winding = string.IsNullOrEmpty(s.Winding) ? "ВН" : s.Winding;
            double classKV = (winding == "ВН") ? p.HVVoltage : p.LVVoltage;
            double percent = s.RequiredU > 0.01 ? s.Uavg / s.RequiredU * 100.0 : 0;

            // --- Условия ---
            sb.AppendLine($@"
<div class='divcaption'>{startNum}. Испытание обмотки {winding}</div>
<table class='compact'>
<tr>
    <th style='width:50%'>Параметр</th>
    <th>Значение</th>
</tr>
<tr><td>Испытываемая обмотка</td><td><b>{winding}</b></td></tr>
<tr><td>Класс напряжения обмотки, кВ</td><td>{classKV:F2}</td></tr>
<tr><td>Нормируемое испытательное напряжение, кВ</td><td><b>{s.RequiredU:F1}</b></td></tr>
<tr><td>Длительность приложения, с</td><td>{l.AVTime}</td></tr>
<tr><td>Допустимое отклонение U и I, %</td><td>{l.AVDeviation:F1}</td></tr>
</table>");

            // --- Измеренные значения ---
            sb.AppendLine($@"
<table class='compact'>
<caption>{startNum + 1}. Измеренные значения (обмотка {winding})</caption>
<tr>
    <th>Параметр</th>
    <th>Значение</th>
</tr>
<tr class='center'><td>Приложенное напряжение U<sub>ср</sub>, кВ</td><td><b>{s.Uavg:F2}</b></td></tr>
<tr class='center'><td>% от нормируемого значения</td><td><b>{percent:F1} %</b></td></tr>
<tr class='center'><td>Ток утечки I<sub>ср</sub>, мА</td><td><b>{s.Iavg:F2}</b></td></tr>
<tr class='center'><td>Макс. отклонение напряжения, %</td><td>{s.MaxUDev:F2}</td></tr>
<tr class='center'><td>Макс. отклонение тока, %</td><td>{s.MaxIDev:F2}</td></tr>
</table>");

            // --- Стабильность ---
            double limit = l.AVDeviation;
            string clsU = s.MaxUDev <= limit ? "pass" : "fail";
            string clsI = s.MaxIDev <= limit ? "pass" : "fail";

            sb.AppendLine($@"
<table class='compact'>
<caption>{startNum + 2}. Проверка стабильности (обмотка {winding})</caption>
<tr>
    <th>Параметр</th>
    <th>Макс. отклонение, %</th>
    <th>Норма, %</th>
    <th>Результат</th>
</tr>
<tr class='center'>
    <td>Напряжение</td>
    <td>{s.MaxUDev:F2}</td>
    <td>{limit:F1}</td>
    <td class='{clsU}'>{(s.MaxUDev <= limit ? "норма" : "превышение")}</td>
</tr>
<tr class='center'>
    <td>Ток утечки</td>
    <td>{s.MaxIDev:F2}</td>
    <td>{limit:F1}</td>
    <td class='{clsI}'>{(s.MaxIDev <= limit ? "норма" : "превышение")}</td>
</tr>
</table>");

            // --- Графики ---
            BuildChartsForOne(sb, s, l, winding);
        }

        //================================================
        // Графики для одного испытания
        //================================================
        private void BuildChartsForOne(StringBuilder sb, AVSnapshot s, LabSettings l, string winding)
        {
            if (s.Times == null || s.Times.Count < 2)
            {
                sb.AppendLine($"<p style='color:#888;'>Недостаточно данных для графиков обмотки {winding}.</p>");
                return;
            }

            double t0 = s.Times[0];
            var labels = new StringBuilder();
            var dataU = new StringBuilder();
            var dataI = new StringBuilder();

            for (int i = 0; i < s.Times.Count; i++)
            {
                if (i > 0) { labels.Append(','); dataU.Append(','); dataI.Append(','); }
                labels.Append((s.Times[i] - t0).ToString("F1", CultureInfo.InvariantCulture));
                dataU.Append(s.Ua[i].ToString("F3", CultureInfo.InvariantCulture));
                dataI.Append(s.Ia[i].ToString("F3", CultureInfo.InvariantCulture));
            }

            string chartIdU = "chartU_" + winding;
            string chartIdI = "chartI_" + winding;

            sb.AppendLine($@"
<div class='divcaption'>Графики записи — обмотка {winding} ({l.AVTime} с)</div>
<div class='chart-container'><canvas id='{chartIdU}'></canvas></div>
<div class='chart-container'><canvas id='{chartIdI}'></canvas></div>

<script>
(function() {{
    const labels = [{labels}];
    const dataU  = [{dataU}];
    const dataI  = [{dataI}];

    const common = {{
        responsive: true,
        maintainAspectRatio: false,
        animation: false,
        plugins: {{ legend: {{ position: 'top' }} }},
        scales: {{
            x: {{ title: {{ display: true, text: 'Время, с' }}, ticks: {{ maxTicksLimit: 10 }} }},
            y: {{ beginAtZero: false }}
        }}
    }};

    new Chart(document.getElementById('{chartIdU}'), {{
        type: 'line',
        data: {{
            labels: labels,
            datasets: [{{
                label: 'U приложенное, кВ',
                data: dataU,
                borderColor: '#c0392b',
                borderWidth: 2,
                pointRadius: 0,
                tension: 0.15
            }}]
        }},
        options: {{
            ...common,
            plugins: {{ ...common.plugins, title: {{ display: true, text: 'Приложенное напряжение ({winding})' }} }},
            scales: {{ ...common.scales, y: {{ ...common.scales.y, title: {{ display: true, text: 'кВ' }} }} }}
        }}
    }});

    new Chart(document.getElementById('{chartIdI}'), {{
        type: 'line',
        data: {{
            labels: labels,
            datasets: [{{
                label: 'I утечки, мА',
                data: dataI,
                borderColor: '#2980b9',
                borderWidth: 2,
                pointRadius: 0,
                tension: 0.15
            }}]
        }},
        options: {{
            ...common,
            plugins: {{ ...common.plugins, title: {{ display: true, text: 'Ток утечки ({winding})' }} }},
            scales: {{ ...common.scales, y: {{ ...common.scales.y, title: {{ display: true, text: 'мА' }} }} }}
        }}
    }});
}})();
</script>");
        }

        //================================================
        // Итоговое заключение
        //================================================
        private void BuildFinalConclusion(StringBuilder sb, PassportModel p, List<AVSnapshot> tests, LabSettings l)
        {
            bool allPassed = tests.All(t => t.Passed);
            string resultText = allPassed ? "соответствуют" : "не соответствуют";
            string boxClass = allPassed ? "resultBox" : "resultBox resultBoxFail";

            var windings = string.Join(" и ", tests.Select(t => t.Winding).Distinct());

            sb.AppendLine(@"
<div class='divcaption'>Заключение</div>");

            sb.AppendLine($@"
<div class='{boxClass}'>
По результатам испытания приложенным напряжением обмоток <b>{windings}</b><br>
в течение {l.AVTime} секунд каждая:<br><br>");

            foreach (var s in tests.OrderBy(t => t.Winding))
            {
                double pct = s.RequiredU > 0.01 ? s.Uavg / s.RequiredU * 100.0 : 0;
                string res = s.Passed ? "ПРОЙДЕНО" : "НЕ ПРОЙДЕНО";
                sb.AppendLine($@"
Обмотка <b>{s.Winding}</b>: U<sub>ср</sub> = {s.Uavg:F2} кВ ({pct:F1} % от нормы {s.RequiredU:F1} кВ), 
I<sub>ср</sub> = {s.Iavg:F2} мА — <b>{res}</b><br>");
            }

            sb.AppendLine($@"
<br>
Измеренные значения <b>{resultText}</b> требованиям нормативной документации<br>
(отклонение параметров не более {l.AVDeviation:F1} %, пробоя и недопустимого роста тока не наблюдалось).
</div>");

            sb.AppendLine($@"
<div class='footer'>
<table class='signTable'>
<tr>
    <td>Испытатель: ______________ {p.Engineer}</td>
    <td>Проверил: ___________________</td>
</tr>
</table>
</div>");
        }

        //================================================
        // Стили (те же)
        //================================================
        private void BuildStyle(StringBuilder sb)
        {
            sb.AppendLine(@"
<style>
@page { size: A4; margin: 14mm; }
body { font-family: Arial, sans-serif; font-size: 12px; color: #222; margin: 0; padding: 0; }
.chart-container {
    position: relative; width: 100%; height: 240px;
    margin: 8px 0 16px 0; background: #fafafa;
    border: 1px solid #ccc; border-radius: 4px; overflow: hidden;
}
.chart-container canvas { width: 100% !important; height: 100% !important; }
@media print {
    .chart-container { height: 165px !important; break-inside: avoid; page-break-inside: avoid; }
}
h1 { text-align: center; font-size: 19px; margin: 0; }
table { width: 100%; border-collapse: collapse; margin-top: 6px; margin-bottom: 12px; }
th { background: #e8edf5; border: 1px solid #666; padding: 5px 6px; text-align: center; font-weight: bold; }
td { border: 1px solid #888; padding: 5px 6px; vertical-align: middle; }
.divcaption {
    font-size: 15px; font-weight: bold; margin: 18px 0 6px 0;
    border-bottom: 2px solid #444; padding-bottom: 3px;
}
.center { text-align: center; }
.pass { color: #0A8A0A; font-weight: bold; }
.fail { color: #D00000; font-weight: bold; }
.resultBox {
    border: 2px solid #4CAF50; background: #F1FFF1;
    padding: 14px 16px; margin-top: 10px; font-size: 13.5px;
    font-weight: bold; color: #1D6F1D; text-align: center; line-height: 1.45;
}
.resultBoxFail {
    border: 2px solid #D00000; background: #FFF2F2; color: #B00000;
}
.footer { margin-top: 32px; }
.flexRow { display: flex; gap: 16px; }
.flexCol { flex: 1; }
.protocolTitle { margin: 8px 0 14px 0; }
.protocolNumber { font-size: 17px; font-weight: bold; text-align: center; margin-bottom: 6px; }
.signTable td { border: 0; padding-top: 26px; }
.compact td, .compact th { padding: 4px 6px; }
.compact caption { font-size: 14px; font-weight: bold; text-align: left; padding-bottom: 4px; }
</style>");
        }
    }
}