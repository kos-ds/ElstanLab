namespace UIgpt
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabPassport = new System.Windows.Forms.TabPage();
            this.tabRatio = new System.Windows.Forms.TabPage();
            this.tabShortCircuit = new System.Windows.Forms.TabPage();
            this.tabNoLoad = new System.Windows.Forms.TabPage();
            this.tabIVW = new System.Windows.Forms.TabPage();
            this.tabOther = new System.Windows.Forms.TabPage();
            this.tabReport = new System.Windows.Forms.TabPage();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblConnection = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblMode = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabMain.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabPassport);
            this.tabMain.Controls.Add(this.tabRatio);
            this.tabMain.Controls.Add(this.tabShortCircuit);
            this.tabMain.Controls.Add(this.tabNoLoad);
            this.tabMain.Controls.Add(this.tabIVW);
            this.tabMain.Controls.Add(this.tabOther);
            this.tabMain.Controls.Add(this.tabReport);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 0);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(1184, 739);
            this.tabMain.TabIndex = 0;
            this.tabMain.SelectedIndexChanged += new System.EventHandler(this.tabMain_SelectedIndexChanged);
            // 
            // tabPassport
            // 
            this.tabPassport.Location = new System.Drawing.Point(4, 22);
            this.tabPassport.Name = "tabPassport";
            this.tabPassport.Padding = new System.Windows.Forms.Padding(3);
            this.tabPassport.Size = new System.Drawing.Size(1176, 713);
            this.tabPassport.TabIndex = 0;
            this.tabPassport.Text = "Паспорт изделия";
            this.tabPassport.UseVisualStyleBackColor = true;
            this.tabPassport.Click += new System.EventHandler(this.tabPassport_Click);
            // 
            // tabRatio
            // 
            this.tabRatio.Location = new System.Drawing.Point(4, 22);
            this.tabRatio.Name = "tabRatio";
            this.tabRatio.Padding = new System.Windows.Forms.Padding(3);
            this.tabRatio.Size = new System.Drawing.Size(1176, 713);
            this.tabRatio.TabIndex = 1;
            this.tabRatio.Text = "КТР";
            this.tabRatio.UseVisualStyleBackColor = true;
            // 
            // tabShortCircuit
            // 
            this.tabShortCircuit.Location = new System.Drawing.Point(4, 22);
            this.tabShortCircuit.Name = "tabShortCircuit";
            this.tabShortCircuit.Size = new System.Drawing.Size(1176, 713);
            this.tabShortCircuit.TabIndex = 2;
            this.tabShortCircuit.Text = "Сопротивление и потери КЗ";
            this.tabShortCircuit.UseVisualStyleBackColor = true;
            // 
            // tabNoLoad
            // 
            this.tabNoLoad.Location = new System.Drawing.Point(4, 22);
            this.tabNoLoad.Name = "tabNoLoad";
            this.tabNoLoad.Size = new System.Drawing.Size(1176, 713);
            this.tabNoLoad.TabIndex = 3;
            this.tabNoLoad.Text = "Холостой ход";
            this.tabNoLoad.UseVisualStyleBackColor = true;
            // 
            // tabIVW
            // 
            this.tabIVW.Location = new System.Drawing.Point(4, 22);
            this.tabIVW.Name = "tabIVW";
            this.tabIVW.Size = new System.Drawing.Size(1176, 713);
            this.tabIVW.TabIndex = 5;
            this.tabIVW.Text = "IVW";
            this.tabIVW.UseVisualStyleBackColor = true;
            // 
            // tabOther
            // 
            this.tabOther.Location = new System.Drawing.Point(4, 22);
            this.tabOther.Name = "tabOther";
            this.tabOther.Size = new System.Drawing.Size(1176, 713);
            this.tabOther.TabIndex = 6;
            this.tabOther.Text = "Дополнительно";
            this.tabOther.UseVisualStyleBackColor = true;
            // 
            // tabReport
            // 
            this.tabReport.Location = new System.Drawing.Point(4, 22);
            this.tabReport.Name = "tabReport";
            this.tabReport.Size = new System.Drawing.Size(1176, 713);
            this.tabReport.TabIndex = 7;
            this.tabReport.Text = "Отчет";
            this.tabReport.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblConnection,
            this.lblMode});
            this.statusStrip1.Location = new System.Drawing.Point(0, 739);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1184, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            this.statusStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.statusStrip1_ItemClicked);
            // 
            // lblConnection
            // 
            this.lblConnection.ForeColor = System.Drawing.Color.Red;
            this.lblConnection.Margin = new System.Windows.Forms.Padding(100, 3, 0, 2);
            this.lblConnection.Name = "lblConnection";
            this.lblConnection.Size = new System.Drawing.Size(60, 17);
            this.lblConnection.Text = "Нет связи";
            // 
            // lblMode
            // 
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(56, 17);
            this.lblMode.Text = "Режим: -";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1184, 761);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.statusStrip1);
            this.MinimumSize = new System.Drawing.Size(1100, 700);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Лаборатория испытания трансформаторов";
            this.tabMain.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabPassport;
        private System.Windows.Forms.TabPage tabRatio;
        private System.Windows.Forms.TabPage tabShortCircuit;
        private System.Windows.Forms.TabPage tabNoLoad;
        private System.Windows.Forms.TabPage tabIVW;
        private System.Windows.Forms.TabPage tabOther;
        private System.Windows.Forms.TabPage tabReport;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblConnection;
        private System.Windows.Forms.ToolStripStatusLabel lblMode;
    }
}

