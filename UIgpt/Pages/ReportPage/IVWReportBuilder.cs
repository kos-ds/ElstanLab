using ElstanLab.Models;
using System;
using System.Text;

namespace ElstanLab.Pages.ReportPage.Reports
{
    public class IVWReportBuilder : BaseReportBuilder
    {
        public override string Build()
        {
            PassportModel p = LabStorage.Passport;
            IVWSnapshot s = LabStorage.CurrentIVW;
            LabSettings l = LabStorage.labsett;

            StringBuilder sb = new StringBuilder();

            BeginHtml(sb);
            BuildStyle(sb);

            // Подключаем Chart.js
            sb.AppendLine(@"
<script src=""https://cdn.jsdelivr.net/npm/chart.js@4.4.1/dist/chart.umd.min.js""></script>
");

            sb.AppendLine(@"
<body>

<div class='protocolNumber'>
ПРОТОКОЛ № _______
</div>

<div class='protocolTitle'>
<h1>
ИСПЫТАНИЕ ИНДУЦИРОВАННЫМ НАПРЯЖЕНИЕМ
</h1>
</div>
");

            BuildTopTables(sb, p);
            BuildMeasuredData(sb, s);
            BuildStabilityTable(sb, s);
            BuildCharts(sb, s);                 // ← графики
            BuildConclusion(sb, p, s);

            sb.AppendLine(@"
</body>
</html>
");

            return sb.ToString();
        }

        ///////////////////////////////////////////////////////////////
        // 4. ГРАФИКИ ЗАПИСИ
        ///////////////////////////////////////////////////////////////

        private void BuildCharts(StringBuilder sb, IVWSnapshot s)
        {
            if (s.Times == null || s.Times.Count < 2)
            {
                sb.AppendLine(@"
<div class='divcaption'>4. Графики записи</div>
<p style='color:#888;'>Недостаточно данных для построения графиков.</p>
");
                return;
            }

            double t0 = s.Times[0];

            var labels = new StringBuilder();
            var ua = new StringBuilder();
            var ub = new StringBuilder();
            var uc = new StringBuilder();
            var ia = new StringBuilder();
            var ib = new StringBuilder();
            var ic = new StringBuilder();

            for (int i = 0; i < s.Times.Count; i++)
            {
                double tRel = s.Times[i] - t0;

                if (i > 0)
                {
                    labels.Append(',');
                    ua.Append(','); ub.Append(','); uc.Append(',');
                    ia.Append(','); ib.Append(','); ic.Append(',');
                }

                labels.Append(tRel.ToString("F1", System.Globalization.CultureInfo.InvariantCulture));
                ua.Append(s.Ua[i].ToString("F2", System.Globalization.CultureInfo.InvariantCulture));
                ub.Append(s.Ub[i].ToString("F2", System.Globalization.CultureInfo.InvariantCulture));
                uc.Append(s.Uc[i].ToString("F2", System.Globalization.CultureInfo.InvariantCulture));
                ia.Append(s.Ia[i].ToString("F3", System.Globalization.CultureInfo.InvariantCulture));
                ib.Append(s.Ib[i].ToString("F3", System.Globalization.CultureInfo.InvariantCulture));
                ic.Append(s.Ic[i].ToString("F3", System.Globalization.CultureInfo.InvariantCulture));
            }

            sb.AppendLine(@"
<div class='divcaption'>4. Графики записи (" + LabStorage.labsett.IVWTime + @" сек)</div>

<div class='chart-container'>
    <canvas id='chartU'></canvas>
</div>

<div class='chart-container'>
    <canvas id='chartI'></canvas>
</div>

<script>
(function() {
    const labels = [" + labels + @"];
    const dataUa = [" + ua + @"];
    const dataUb = [" + ub + @"];
    const dataUc = [" + uc + @"];
    const dataIa = [" + ia + @"];
    const dataIb = [" + ib + @"];
    const dataIc = [" + ic + @"];

    const commonOptions = {
        responsive: true,
        maintainAspectRatio: false,          // важно — берёт высоту от контейнера
        animation: false,                    // быстрее и стабильнее при печати
        interaction: {
            mode: 'index',
            intersect: false
        },
        plugins: {
            legend: {
                position: 'top',
                labels: { boxWidth: 12, font: { size: 11 } }
            },
            tooltip: { enabled: true }
        },
        scales: {
            x: {
                title: {
                    display: true,
                    text: 'Время, с',
                    font: { size: 11 }
                },
                ticks: {
                    maxTicksLimit: 10,
                    font: { size: 10 }
                },
                grid: { color: '#eee' }
            },
            y: {
                title: {
                    display: true,
                    text: '',
                    font: { size: 11 }
                },
                ticks: { font: { size: 10 } },
                grid: { color: '#eee' },
                beginAtZero: false
            }
        }
    };

    // ===== Напряжение =====
    new Chart(document.getElementById('chartU'), {
        type: 'line',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Ua',
                    data: dataUa,
                    borderColor: '#e74c3c',
                    backgroundColor: 'rgba(231, 76, 60, 0.07)',
                    borderWidth: 1.8,
                    pointRadius: 0,
                    tension: 0.15
                },
                {
                    label: 'Ub',
                    data: dataUb,
                    borderColor: '#27ae60',
                    backgroundColor: 'rgba(39, 174, 96, 0.07)',
                    borderWidth: 1.8,
                    pointRadius: 0,
                    tension: 0.15
                },
                {
                    label: 'Uc',
                    data: dataUc,
                    borderColor: '#2980b9',
                    backgroundColor: 'rgba(41, 128, 185, 0.07)',
                    borderWidth: 1.8,
                    pointRadius: 0,
                    tension: 0.15
                }
            ]
        },
        options: {
            ...commonOptions,
            plugins: {
                ...commonOptions.plugins,
                title: {
                    display: true,
                    text: 'Напряжение по фазам',
                    font: { size: 14, weight: 'bold' },
                    padding: { bottom: 8 }
                }
            },
            scales: {
                ...commonOptions.scales,
                y: {
                    ...commonOptions.scales.y,
                    title: { display: true, text: 'Напряжение, В', font: { size: 11 } }
                }
            }
        }
    });

    // ===== Ток =====
    new Chart(document.getElementById('chartI'), {
        type: 'line',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Ia',
                    data: dataIa,
                    borderColor: '#e74c3c',
                    backgroundColor: 'rgba(231, 76, 60, 0.07)',
                    borderWidth: 1.8,
                    pointRadius: 0,
                    tension: 0.15
                },
                {
                    label: 'Ib',
                    data: dataIb,
                    borderColor: '#27ae60',
                    backgroundColor: 'rgba(39, 174, 96, 0.07)',
                    borderWidth: 1.8,
                    pointRadius: 0,
                    tension: 0.15
                },
                {
                    label: 'Ic',
                    data: dataIc,
                    borderColor: '#2980b9',
                    backgroundColor: 'rgba(41, 128, 185, 0.07)',
                    borderWidth: 1.8,
                    pointRadius: 0,
                    tension: 0.15
                }
            ]
        },
        options: {
            ...commonOptions,
            plugins: {
                ...commonOptions.plugins,
                title: {
                    display: true,
                    text: 'Ток по фазам',
                    font: { size: 14, weight: 'bold' },
                    padding: { bottom: 8 }
                }
            },
            scales: {
                ...commonOptions.scales,
                y: {
                    ...commonOptions.scales.y,
                    title: { display: true, text: 'Ток, А', font: { size: 11 } }
                }
            }
        }
    });
})();
</script>
");
        }

        ///////////////////////////////////////////////////////////////
        // STYLE (тот же, что в NoLoad)
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


.chart-container {
    position: relative;
    width: 100%;
    height: 280px;
    margin: 12px 0 24px 0;
    background: #fafafa;
    border: 1px solid #ccc;
    border-radius: 4px;
    overflow: hidden;          
}

.chart-container canvas {
    width: 100% !important;
    height: 100% !important;
}

@media print {
    .chart-container {
        height: 190px !important;
        break-inside: avoid;
        page-break-inside: avoid;
    }
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

.center { text-align: center; }
.right  { text-align: right; }

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

.compact td { padding: 5px; }
.compact th { padding: 5px; }

.compact caption
{
    font-size: 20px;
    font-weight: bold;
    text-align: left;
    padding-bottom: 5px;
}

</style>
");
        }

        ///////////////////////////////////////////////////////////////
        // 1. ПАСПОРТНЫЕ ДАННЫЕ
        ///////////////////////////////////////////////////////////////

        private void BuildTopTables(StringBuilder sb, PassportModel p)
        {
            sb.AppendLine(@"
<div class='flexRow'>
<div class='flexCol'>
<table class='compact'>
<caption>1. Паспортные данные</caption>
");

            AddRow4(sb, "Дата испытания", DateTime.Now.ToString("dd.MM.yyyy"), "Напряжение ВН, кВ", p.HVVoltage.ToString("F3"));
            AddRow4(sb, "Производитель тр-ра", p.Factory, "Ток ВН, А", p.IHV.ToString("F1"));
            AddRow4(sb, "Тип трансформатора", p.Type, "Напряжение НН, кВ", p.LVVoltage.ToString("F3"));
            AddRow4(sb, "Заводской номер", p.Serial, "Ток НН, А", p.ILV.ToString("F1"));
            AddRow4(sb, "Номинальная мощность, кВА", p.PowerKva.ToString("F0"), "Охлаждение", p.Cooling);
            AddRow4(sb, "Частота, Гц", "50", "Группа соединения", p.VectorGroup);
            AddRow4(sb, "Год выпуска", p.Year.ToString(), "Испытатель", p.Engineer ?? "");

            sb.AppendLine(@"
</table>
</div>
</div>
");
        }

        ///////////////////////////////////////////////////////////////
        // 2. ИЗМЕРЕННЫЕ СРЕДНИЕ ЗНАЧЕНИЯ
        ///////////////////////////////////////////////////////////////

        private void BuildMeasuredData(StringBuilder sb, IVWSnapshot s)
        {
            sb.AppendLine(@"
<table class='compact'>
<caption>2. Измеренные средние значения (за время испытания)</caption>

<tr>
<th>Фаза</th>
<th>U поданное, В</th>
<th>U приложен, В</th>
<th>Ток, А</th>
</tr>
");

            sb.AppendLine($@"
<tr class='center'>
<td>A</td>
<td>{s.UaMean:F1}</td>
<td>{s.UaMean*3.47826:F1}</td>
<td>{s.IaMean:F2}</td>
</tr>
<tr class='center'>
<td>B</td>
<td>{s.UbMean:F1}</td>
<td>{s.UbMean*3.47826:F1}</td>
<td>{s.IbMean:F2}</td>
</tr>
<tr class='center'>
<td>C</td>
<td>{s.UcMean:F1}</td>
<td>{s.UcMean*3.47826:F1}</td>
<td>{s.IcMean:F2}</td>
</tr>
<tr class='center'>
<td><b>Среднее</b></td>
<td><b>{s.Uavg:F1}</b></td>
<td><b>{s.Uavg*3.47826:F1}</b></td>
<td><b>{s.Iavg:F2}</b></td>
</tr>
");

            sb.AppendLine("</table>");
        }

        ///////////////////////////////////////////////////////////////
        // 3. СТАБИЛЬНОСТЬ (ОТКЛОНЕНИЯ)
        ///////////////////////////////////////////////////////////////

        private void BuildStabilityTable(StringBuilder sb, IVWSnapshot s)
        {
             double limit = LabStorage.labsett.IVWDeviation;   // % — как в анализе

            sb.AppendLine(@"
<table class='compact'>
<caption>3. Проверка стабильности параметров</caption>

<tr>
<th>Параметр</th>
<th>Макс. отклонение, %</th>
<th>Норма, %</th>
<th>Результат</th>
</tr>
");

            string clsUa = s.UaDev <= limit ? "pass" : "fail";
            string clsUb = s.UbDev <= limit ? "pass" : "fail";
            string clsUc = s.UcDev <= limit ? "pass" : "fail";
            string clsIa = s.IaDev <= limit ? "pass" : "fail";
            string clsIb = s.IbDev <= limit ? "pass" : "fail";
            string clsIc = s.IcDev <= limit ? "pass" : "fail";

            sb.AppendLine($@"
<tr class='center'>
<td>Ua</td>
<td>{s.UaDev:F2}</td>
<td>{limit:F0}</td>
<td class='{clsUa}'>{(s.UaDev <= limit ? "норма" : "превышение")}</td>
</tr>
<tr class='center'>
<td>Ub</td>
<td>{s.UbDev:F2}</td>
<td>{limit:F0}</td>
<td class='{clsUb}'>{(s.UbDev <= limit ? "норма" : "превышение")}</td>
</tr>
<tr class='center'>
<td>Uc</td>
<td>{s.UcDev:F2}</td>
<td>{limit:F0}</td>
<td class='{clsUc}'>{(s.UcDev <= limit ? "норма" : "превышение")}</td>
</tr>
<tr class='center'>
<td>Ia</td>
<td>{s.IaDev:F2}</td>
<td>{limit:F0}</td>
<td class='{clsIa}'>{(s.IaDev <= limit ? "норма" : "превышение")}</td>
</tr>
<tr class='center'>
<td>Ib</td>
<td>{s.IbDev:F2}</td>
<td>{limit:F0}</td>
<td class='{clsIb}'>{(s.IbDev <= limit ? "норма" : "превышение")}</td>
</tr>
<tr class='center'>
<td>Ic</td>
<td>{s.IcDev:F2}</td>
<td>{limit:F0}</td>
<td class='{clsIc}'>{(s.IcDev <= limit ? "норма" : "превышение")}</td>
</tr>
");

            // Итоговая строка
            string clsMaxU = s.MaxUDev <= limit ? "pass" : "fail";
            string clsMaxI = s.MaxIDev <= limit ? "pass" : "fail";

            sb.AppendLine($@"
<tr class='center'>
<td><b>Макс. по напряжению</b></td>
<td><b>{s.MaxUDev:F2}</b></td>
<td>{limit:F0}</td>
<td class='{clsMaxU}'><b>{(s.MaxUDev <= limit ? "норма" : "превышение")}</b></td>
</tr>
<tr class='center'>
<td><b>Макс. по току</b></td>
<td><b>{s.MaxIDev:F2}</b></td>
<td>{limit:F0}</td>
<td class='{clsMaxI}'><b>{(s.MaxIDev <= limit ? "норма" : "превышение")}</b></td>
</tr>
");

            sb.AppendLine("</table>");
        }

        ///////////////////////////////////////////////////////////////
        // 4. ЗАКЛЮЧЕНИЕ
        ///////////////////////////////////////////////////////////////

        private void BuildConclusion(StringBuilder sb, PassportModel p, IVWSnapshot s)
        {
            string text = s.Passed ? "соответствуют" : "не соответствуют";

            string cls = s.Passed
                ? "resultBox"
                : "resultBox resultBoxFail";

            sb.AppendLine(@"
<div class='divcaption'>
5. Заключение
</div>
");

            sb.AppendLine($@"
<div class='{cls}'>
По результатам испытания индуцированным напряжением в течение {LabStorage.labsett.IVWTime:F0} секунд:<br>
среднее напряжение составило U<sub>ср</sub> = {s.Uavg*3.47826:F1} В,<br>
средний ток составил I<sub>ср</sub> = {s.Iavg:F2} А.<br><br>
Максимальное отклонение напряжения от среднего — {s.MaxUDev:F2} %,<br>
максимальное отклонение тока от среднего — {s.MaxIDev:F2} %.<br><br>
Измеренные значения {text} требованиям нормативной документации<br>
(отклонение не более {LabStorage.labsett.IVWDeviation:F1} %).
</div>
");

            sb.AppendLine($@"
<div class='footer'>
<table class='signTable'>
<tr>
<td>Испытатель: ______________{p.Engineer}</td>
<td>Проверил: ___________________</td>
</tr>
</table>
</div>
");
        }
    }
}