namespace ElstanLab.Pages.ReportPage.Reports
{
    public static class ReportStyles
    {
        public static string CSS =
        @"

<html>

<head>

<meta charset='utf-8'>

<style>

@page
{
    size:A4;
    margin:20mm;
}

body
{
    font-family:'Segoe UI';
    font-size:13px;
    color:#222;
}

.page
{
    width:100%;
}

.protocol-header
{
    border:2px solid #444;
    padding:15px;
    margin-bottom:20px;
}

.company
{
    text-align:center;
    font-size:24px;
    font-weight:700;
}

.protocol-title
{
    text-align:center;
    font-size:22px;
    margin-top:10px;
    font-weight:700;
}

.protocol-subtitle
{
    text-align:center;
    font-size:18px;
    margin-top:10px;
}

.section-title
{
    margin-top:25px;
    margin-bottom:10px;
    border-left:5px solid #444;
    padding-left:10px;
    font-size:18px;
    font-weight:700;
}

table
{
    width:100%;
    border-collapse:collapse;
    margin-top:10px;
}

th
{
    background:#DCE6F1;
}

th, td
{
    border:1px solid #666;
    padding:8px;
    text-align:center;
}

tr:nth-child(even)
{
    background:#F7F7F7;
}

.pass
{
    color:#0A7A0A;
    font-weight:700;
}

.fail
{
    color:#C00000;
    font-weight:700;
}

.conclusion
{
    margin-top:25px;
    border:2px solid #444;
    padding:15px;
    background:#FAFAFA;
    line-height:24px;
}

.footer
{
    margin-top:40px;
}

.sign-table td
{
    border:none;
    padding-top:40px;
}

</style>

</head>

";
    }
}
