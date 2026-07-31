using System;
using System.Windows.Forms;

using ElstanLab.Pages.ReportPage.Reports;
using Microsoft.Web.WebView2.WinForms;
using System.IO;
using Microsoft.Web.WebView2.Core;

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

        private Button btnSaveHtml;

        private WebView2 browser;
        

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

            topPanel.Height = 55;

            topPanel.Padding = new Padding(8);

            topPanel.BackColor =
                System.Drawing.Color.FromArgb(
                    245,
                    245,
                    245);

            //------------------------------------------------
            // REPORT TYPE
            //------------------------------------------------

            cbType = new ComboBox();

            cbType.Left = 10;

            cbType.Top = 10;

            cbType.Width = 320;

            cbType.Height = 30;

            cbType.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    10F);

            cbType.DropDownStyle = ComboBoxStyle.DropDownList;

            cbType.Items.Add("Коэффициент трансформации"); 

            cbType.Items.Add("Сопротивление и потери КЗ");

            cbType.Items.Add("Потери и ток ХХ");

            cbType.Items.Add("Индуцированное напряжение");

            cbType.Items.Add("Приложенное напряжение");

            cbType.SelectedIndex = 0;

            //------------------------------------------------
            // GENERATE
            //------------------------------------------------

            btnGenerate = new Button();

            btnGenerate.Text =
                "Сформировать";

            btnGenerate.Left = 350;

            btnGenerate.Top = 8;

            btnGenerate.Width = 150;

            btnGenerate.Height = 34;

            btnGenerate.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    10F,
                    System.Drawing.FontStyle.Bold);

            //------------------------------------------------
            // PRINT
            //------------------------------------------------

            btnPrint = new Button();

            btnPrint.Text =
                "Печать";

            btnPrint.Left = 510;

            btnPrint.Top = 8;

            btnPrint.Width = 120;

            btnPrint.Height = 34;

            btnPrint.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    10F);

            //------------------------------------------------
            // SAVE HTML
            //------------------------------------------------

            btnSaveHtml = new Button();

            btnSaveHtml.Text =
                "Сохранить HTML";

            btnSaveHtml.Left = 640;

            btnSaveHtml.Top = 8;

            btnSaveHtml.Width = 160;

            btnSaveHtml.Height = 34;

            btnSaveHtml.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    10F);

            
            //------------------------------------------------
            // ADD
            //------------------------------------------------

            topPanel.Controls.Add(cbType);

            topPanel.Controls.Add(btnGenerate);

            topPanel.Controls.Add(btnPrint);

         //   topPanel.Controls.Add(btnSaveHtml);

            page.Controls.Add(topPanel);
        }

        //////////////////////////////////////////////////
        // BROWSER
        //////////////////////////////////////////////////

        private void BuildBrowser()
        {
            //browser = new WebBrowser();
            browser = new WebView2();

            browser.Dock = DockStyle.Fill;

            page.Controls.Add(browser);

            browser.CoreWebView2InitializationCompleted +=
                Browser_CoreWebView2InitializationCompleted;

            browser.EnsureCoreWebView2Async();
           
        }


        private void Browser_CoreWebView2InitializationCompleted(object sender,Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            if (!e.IsSuccess)
            {
                MessageBox.Show(
                    e.InitializationException.ToString(),
                    "WebView2");
            }
        }
        //////////////////////////////////////////////////
        // EVENTS
        //////////////////////////////////////////////////

        private void RegisterEvents()
        {
            btnGenerate.Click +=
                BtnGenerate_Click;

            btnPrint.Click +=
                BtnPrint_Click;

            btnSaveHtml.Click +=
                BtnSaveHtml_Click;
            
        }

        //////////////////////////////////////////////////
        // GENERATE
        //////////////////////////////////////////////////
        private async void BtnSavePdf_Click(object sender,EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "PDF (*.pdf)|*.pdf";

            sfd.FileName = GenerateFileName()
                .Replace(".html", ".pdf");

            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            await browser.CoreWebView2.PrintToPdfAsync(sfd.FileName);
            //await browser.CoreWebView2.ExecuteScriptAsync("window.print();");

            MessageBox.Show("PDF сохранен");
        }



        private void BtnGenerate_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                string html =
                    GenerateSelectedReport();

                if (string.IsNullOrEmpty(html))
                {
                    MessageBox.Show(
                        "Не удалось сформировать отчет");

                    return;
                }

                //      browser.DocumentText = html;
                browser.NavigateToString(html);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка формирования отчета:\r\n\r\n"
                    + ex.Message);
            }
        }

        //////////////////////////////////////////////////
        // PRINT
        //////////////////////////////////////////////////

        private void BtnPrint_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (browser.CoreWebView2 == null)
                {
                    MessageBox.Show(
                        "Отчет не сформирован");

                    return;
                }

                CoreWebView2PrintSettings printSettings = browser.CoreWebView2.Environment.CreatePrintSettings();

                // 3. ОТКЛЮЧАЕМ колонтитулы (заголовки и подвалы)
                printSettings.ShouldPrintHeaderAndFooter = false;
                
                browser.CoreWebView2.ExecuteScriptAsync("window.print();");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка печати:\r\n\r\n"
                    + ex.Message);
            }
        }

      
        //////////////////////////////////////////////////
        // SAVE HTML
        //////////////////////////////////////////////////

        private void BtnSaveHtml_Click(object sender,EventArgs e)
        {
            try
            {
                string html = GenerateSelectedReport();

                if (string.IsNullOrEmpty(html))
                {
                    MessageBox.Show(
                        "Нет данных для сохранения");

                    return;
                }

                SaveFileDialog sfd = new SaveFileDialog();

                sfd.Filter = "HTML file (*.html)|*.html";

                sfd.FileName =
                    GenerateFileName();

                if (sfd.ShowDialog()
                    != DialogResult.OK)
                {
                    return;
                }

                System.IO.File.WriteAllText(
                    sfd.FileName,
                    html,
                    System.Text.Encoding.UTF8);

                MessageBox.Show(
                    "Отчет сохранен");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка сохранения:\r\n\r\n"
                    + ex.Message);
            }
        }

        //////////////////////////////////////////////////
        // GENERATE REPORT
        //////////////////////////////////////////////////

        private string GenerateSelectedReport()
        {
            switch (cbType.SelectedIndex)
            {

                case 0:

                    return new RatioReportBuilder().Build(); 

                case 1:

                    return new ShortCircuitReportBuilder().Build();

                case 2:

                    return new NoLoadReportBuilder().Build();

                case 3:  
                    
                    return new IVWReportBuilder().Build();   // ← новый

                case 4:
                    return new AVReportBuilder().Build();   // ← новый
            }

            return "";
        }

        //////////////////////////////////////////////////
        // FILE NAME
        //////////////////////////////////////////////////

        private string GenerateFileName()
        {
            string type = "REPORT";

            switch (cbType.SelectedIndex)
            {
                case 0:
                    type = "NOLOAD";
                    break;

                case 1:
                    type = "SHORTCIRCUIT";
                    break;

                case 2:
                    type = "RATIO";
                    break;

                case 3: 
                    type = "INDUCED"; 
                    break;   // ← новый

                case 4: 
                    type = "APPLIED";
                    break;   // ← новый
            }

            return
                type
                + "_"
                + DateTime.Now.ToString(
                    "yyyyMMdd_HHmmss")
                + ".html";
        }
    }


}
