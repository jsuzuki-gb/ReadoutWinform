namespace ReadoutWinform
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.StartButton = new System.Windows.Forms.Button();
            this.StopButton = new System.Windows.Forms.Button();
            this.DataViewPanel = new System.Windows.Forms.Panel();
            this.DisplayRefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ROStatusStrip = new System.Windows.Forms.StatusStrip();
            this.ReadoutStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.MainTab = new System.Windows.Forms.TabControl();
            this.TODPage = new System.Windows.Forms.TabPage();
            this.SettingsPage = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SDTextBox = new System.Windows.Forms.TextBox();
            this.yTopTextBox = new System.Windows.Forms.TextBox();
            this.YAxisFixComboBox = new System.Windows.Forms.ComboBox();
            this.yBottomTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.freqTextBox1 = new System.Windows.Forms.TextBox();
            this.freqTextBox2 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.xAxisDisplaySpacingTextBox = new System.Windows.Forms.TextBox();
            this.settingApplyButton = new System.Windows.Forms.Button();
            this.programBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.ROStatusStrip.SuspendLayout();
            this.MainTab.SuspendLayout();
            this.TODPage.SuspendLayout();
            this.SettingsPage.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.programBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(6, 6);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(93, 34);
            this.StartButton.TabIndex = 0;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // StopButton
            // 
            this.StopButton.Location = new System.Drawing.Point(101, 6);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(88, 34);
            this.StopButton.TabIndex = 1;
            this.StopButton.Text = "Stop";
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // DataViewPanel
            // 
            this.DataViewPanel.BackColor = System.Drawing.Color.White;
            this.DataViewPanel.Location = new System.Drawing.Point(6, 46);
            this.DataViewPanel.Name = "DataViewPanel";
            this.DataViewPanel.Size = new System.Drawing.Size(934, 591);
            this.DataViewPanel.TabIndex = 2;
            // 
            // DisplayRefreshTimer
            // 
            this.DisplayRefreshTimer.Tick += new System.EventHandler(this.DisplayRefreshTimer_Tick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(978, 33);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(50, 29);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // ROStatusStrip
            // 
            this.ROStatusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.ROStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ReadoutStatus});
            this.ROStatusStrip.Location = new System.Drawing.Point(0, 714);
            this.ROStatusStrip.Name = "ROStatusStrip";
            this.ROStatusStrip.Size = new System.Drawing.Size(978, 30);
            this.ROStatusStrip.TabIndex = 4;
            this.ROStatusStrip.Text = "ROStatusStrip";
            // 
            // ReadoutStatus
            // 
            this.ReadoutStatus.Name = "ReadoutStatus";
            this.ReadoutStatus.Size = new System.Drawing.Size(53, 25);
            this.ReadoutStatus.Text = "hoge";
            this.ReadoutStatus.ToolTipText = "hoge";
            // 
            // MainTab
            // 
            this.MainTab.Controls.Add(this.TODPage);
            this.MainTab.Controls.Add(this.SettingsPage);
            this.MainTab.Location = new System.Drawing.Point(12, 36);
            this.MainTab.Name = "MainTab";
            this.MainTab.SelectedIndex = 0;
            this.MainTab.Size = new System.Drawing.Size(954, 675);
            this.MainTab.TabIndex = 5;
            // 
            // TODPage
            // 
            this.TODPage.Controls.Add(this.StartButton);
            this.TODPage.Controls.Add(this.StopButton);
            this.TODPage.Controls.Add(this.DataViewPanel);
            this.TODPage.Location = new System.Drawing.Point(4, 28);
            this.TODPage.Name = "TODPage";
            this.TODPage.Padding = new System.Windows.Forms.Padding(3);
            this.TODPage.Size = new System.Drawing.Size(946, 643);
            this.TODPage.TabIndex = 0;
            this.TODPage.Text = "TOD";
            this.TODPage.UseVisualStyleBackColor = true;
            // 
            // SettingsPage
            // 
            this.SettingsPage.Controls.Add(this.settingApplyButton);
            this.SettingsPage.Controls.Add(this.tableLayoutPanel1);
            this.SettingsPage.Location = new System.Drawing.Point(4, 28);
            this.SettingsPage.Name = "SettingsPage";
            this.SettingsPage.Padding = new System.Windows.Forms.Padding(3);
            this.SettingsPage.Size = new System.Drawing.Size(946, 643);
            this.SettingsPage.TabIndex = 1;
            this.SettingsPage.Text = "Settings";
            this.SettingsPage.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.xAxisDisplaySpacingTextBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.SDTextBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.yTopTextBox, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.YAxisFixComboBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.yBottomTextBox, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.freqTextBox1, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.freqTextBox2, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 1);
            this.tableLayoutPanel1.Font = new System.Drawing.Font("MS UI Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(17, 34);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 9;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(650, 518);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(290, 249);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 22);
            this.label6.TabIndex = 1;
            this.label6.Text = "#1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(268, 22);
            this.label1.TabIndex = 0;
            this.label1.Text = "Software downsample count";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 22);
            this.label2.TabIndex = 1;
            this.label2.Text = "Y-axis fix";
            // 
            // SDTextBox
            // 
            this.SDTextBox.Location = new System.Drawing.Point(328, 3);
            this.SDTextBox.Name = "SDTextBox";
            this.SDTextBox.Size = new System.Drawing.Size(150, 29);
            this.SDTextBox.TabIndex = 2;
            this.SDTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // yTopTextBox
            // 
            this.yTopTextBox.Location = new System.Drawing.Point(328, 123);
            this.yTopTextBox.Name = "yTopTextBox";
            this.yTopTextBox.Size = new System.Drawing.Size(150, 29);
            this.yTopTextBox.TabIndex = 3;
            this.yTopTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // YAxisFixComboBox
            // 
            this.YAxisFixComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.YAxisFixComboBox.FormattingEnabled = true;
            this.YAxisFixComboBox.Items.AddRange(new object[] {
            "True",
            "False"});
            this.YAxisFixComboBox.Location = new System.Drawing.Point(328, 83);
            this.YAxisFixComboBox.Name = "YAxisFixComboBox";
            this.YAxisFixComboBox.Size = new System.Drawing.Size(150, 30);
            this.YAxisFixComboBox.TabIndex = 4;
            this.YAxisFixComboBox.SelectedIndexChanged += new System.EventHandler(this.YAxisFixComboBox_SelectedIndexChanged);
            // 
            // yBottomTextBox
            // 
            this.yBottomTextBox.Location = new System.Drawing.Point(328, 163);
            this.yBottomTextBox.Name = "yBottomTextBox";
            this.yBottomTextBox.Size = new System.Drawing.Size(150, 29);
            this.yBottomTextBox.TabIndex = 5;
            this.yBottomTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(233, 169);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 22);
            this.label4.TabIndex = 7;
            this.label4.Text = "(bottom)";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(268, 129);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 22);
            this.label3.TabIndex = 6;
            this.label3.Text = "(top)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 200);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(106, 22);
            this.label5.TabIndex = 8;
            this.label5.Text = "Frequency";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(290, 289);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 22);
            this.label7.TabIndex = 9;
            this.label7.Text = "#2";
            // 
            // freqTextBox1
            // 
            this.freqTextBox1.Location = new System.Drawing.Point(328, 243);
            this.freqTextBox1.Name = "freqTextBox1";
            this.freqTextBox1.Size = new System.Drawing.Size(150, 29);
            this.freqTextBox1.TabIndex = 10;
            this.freqTextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // freqTextBox2
            // 
            this.freqTextBox2.Location = new System.Drawing.Point(328, 283);
            this.freqTextBox2.Name = "freqTextBox2";
            this.freqTextBox2.Size = new System.Drawing.Size(150, 29);
            this.freqTextBox2.TabIndex = 11;
            this.freqTextBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 40);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(213, 22);
            this.label8.TabIndex = 12;
            this.label8.Text = "X-axis display spacing";
            // 
            // xAxisDisplaySpacingTextBox
            // 
            this.xAxisDisplaySpacingTextBox.Location = new System.Drawing.Point(328, 43);
            this.xAxisDisplaySpacingTextBox.Name = "xAxisDisplaySpacingTextBox";
            this.xAxisDisplaySpacingTextBox.Size = new System.Drawing.Size(150, 29);
            this.xAxisDisplaySpacingTextBox.TabIndex = 13;
            this.xAxisDisplaySpacingTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // settingApplyButton
            // 
            this.settingApplyButton.Font = new System.Drawing.Font("MS UI Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.settingApplyButton.Location = new System.Drawing.Point(831, 577);
            this.settingApplyButton.Name = "settingApplyButton";
            this.settingApplyButton.Size = new System.Drawing.Size(90, 40);
            this.settingApplyButton.TabIndex = 1;
            this.settingApplyButton.Text = "Apply";
            this.settingApplyButton.UseVisualStyleBackColor = true;
            this.settingApplyButton.Click += new System.EventHandler(this.settingApplyButton_Click);
            // 
            // programBindingSource
            // 
            this.programBindingSource.DataSource = typeof(ReadoutWinform.Program);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(211, 30);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(978, 744);
            this.Controls.Add(this.MainTab);
            this.Controls.Add(this.ROStatusStrip);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Test";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ROStatusStrip.ResumeLayout(false);
            this.ROStatusStrip.PerformLayout();
            this.MainTab.ResumeLayout(false);
            this.TODPage.ResumeLayout(false);
            this.SettingsPage.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.programBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.Panel DataViewPanel;
        private System.Windows.Forms.Timer DisplayRefreshTimer;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.StatusStrip ROStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel ReadoutStatus;
        private System.Windows.Forms.TabControl MainTab;
        private System.Windows.Forms.TabPage TODPage;
        private System.Windows.Forms.TabPage SettingsPage;
        private System.Windows.Forms.BindingSource programBindingSource;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox SDTextBox;
        private System.Windows.Forms.TextBox yTopTextBox;
        private System.Windows.Forms.ComboBox YAxisFixComboBox;
        private System.Windows.Forms.TextBox yBottomTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox freqTextBox1;
        private System.Windows.Forms.TextBox freqTextBox2;
        private System.Windows.Forms.TextBox xAxisDisplaySpacingTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button settingApplyButton;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
    }
}

