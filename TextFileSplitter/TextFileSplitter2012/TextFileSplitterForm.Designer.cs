namespace Martin.SQLServer.Dts
{
    partial class TextFileSplitterForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextFileSplitterForm));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tpConnection = new System.Windows.Forms.TabPage();
            this.dgvConnectionPreview = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbNumberOfRecordsToPreview = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbTextDelimiter = new System.Windows.Forms.TextBox();
            this.cbTreatNulls = new System.Windows.Forms.CheckBox();
            this.tbColumnDelimiter = new System.Windows.Forms.TextBox();
            this.cbDelimitedText = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbConnectionManager = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPreview = new System.Windows.Forms.Button();
            this.tpPassThrough = new System.Windows.Forms.TabPage();
            this.dgvPassThroughPreview = new System.Windows.Forms.DataGridView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.cbPTErrorDisposition = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbPTNumberOfRecordsToPreview = new System.Windows.Forms.NumericUpDown();
            this.btnPTPreview = new System.Windows.Forms.Button();
            this.dgvPassThrough = new System.Windows.Forms.DataGridView();
            this.clmPTColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmPTUsageOfColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.clmPTCodePage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmPTDataType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.clmPTLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmPTPrecision = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmPTScale = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tpOutputs = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgvOutputColumns = new System.Windows.Forms.DataGridView();
            this.clmOCColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmOCUsage = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.clmOCdotNetFormatString = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmOCCodePage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmOCDataType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.clmOCLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmOCPrecision = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmOCScale = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel8 = new System.Windows.Forms.Panel();
            this.btnAddColumn = new System.Windows.Forms.Button();
            this.btnRemoveColumn = new System.Windows.Forms.Button();
            this.dgvOutputPreview = new System.Windows.Forms.DataGridView();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label11 = new System.Windows.Forms.Label();
            this.cbOutputType = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbRowTypeValue = new System.Windows.Forms.TextBox();
            this.tbOutputName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cbOutputDisposition = new System.Windows.Forms.ComboBox();
            this.tbOutputNumberOfRecordsToPreview = new System.Windows.Forms.NumericUpDown();
            this.btnOutputPreview = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lbOutputs = new System.Windows.Forms.ListBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.btnGenerateOutputs = new System.Windows.Forms.Button();
            this.panel5 = new System.Windows.Forms.Panel();
            this.btnAddOutput = new System.Windows.Forms.Button();
            this.btnRemoveOutput = new System.Windows.Forms.Button();
            this.tpAbout = new System.Windows.Forms.TabPage();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.ttSplitter = new System.Windows.Forms.ToolTip(this.components);
            this.panel1.SuspendLayout();
            this.tcMain.SuspendLayout();
            this.tpConnection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConnectionPreview)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbNumberOfRecordsToPreview)).BeginInit();
            this.tpPassThrough.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPassThroughPreview)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbPTNumberOfRecordsToPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPassThrough)).BeginInit();
            this.tpOutputs.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOutputColumns)).BeginInit();
            this.panel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOutputPreview)).BeginInit();
            this.panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbOutputNumberOfRecordsToPreview)).BeginInit();
            this.panel4.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.tpAbout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 416);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(642, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnHelp);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 383);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(642, 33);
            this.panel1.TabIndex = 1;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(393, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "Ok";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(474, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(555, 4);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(75, 23);
            this.btnHelp.TabIndex = 0;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tpConnection);
            this.tcMain.Controls.Add(this.tpPassThrough);
            this.tcMain.Controls.Add(this.tpOutputs);
            this.tcMain.Controls.Add(this.tpAbout);
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Location = new System.Drawing.Point(0, 0);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(642, 383);
            this.tcMain.TabIndex = 2;
            // 
            // tpConnection
            // 
            this.tpConnection.Controls.Add(this.dgvConnectionPreview);
            this.tpConnection.Controls.Add(this.panel2);
            this.tpConnection.Location = new System.Drawing.Point(4, 22);
            this.tpConnection.Name = "tpConnection";
            this.tpConnection.Padding = new System.Windows.Forms.Padding(3);
            this.tpConnection.Size = new System.Drawing.Size(634, 357);
            this.tpConnection.TabIndex = 0;
            this.tpConnection.Text = "Connection";
            this.tpConnection.UseVisualStyleBackColor = true;
            // 
            // dgvConnectionPreview
            // 
            this.dgvConnectionPreview.AllowUserToAddRows = false;
            this.dgvConnectionPreview.AllowUserToDeleteRows = false;
            this.dgvConnectionPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvConnectionPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvConnectionPreview.Location = new System.Drawing.Point(3, 157);
            this.dgvConnectionPreview.Name = "dgvConnectionPreview";
            this.dgvConnectionPreview.ReadOnly = true;
            this.dgvConnectionPreview.Size = new System.Drawing.Size(628, 197);
            this.dgvConnectionPreview.TabIndex = 1;
            this.ttSplitter.SetToolTip(this.dgvConnectionPreview, "Click the Preview Button to fill this grid with data.");
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.tbNumberOfRecordsToPreview);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.tbTextDelimiter);
            this.panel2.Controls.Add(this.cbTreatNulls);
            this.panel2.Controls.Add(this.tbColumnDelimiter);
            this.panel2.Controls.Add(this.cbDelimitedText);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.cbConnectionManager);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.btnPreview);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(628, 154);
            this.panel2.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(467, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(132, 111);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(276, 130);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(140, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Number of Preview Records";
            // 
            // tbNumberOfRecordsToPreview
            // 
            this.tbNumberOfRecordsToPreview.Location = new System.Drawing.Point(422, 128);
            this.tbNumberOfRecordsToPreview.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.tbNumberOfRecordsToPreview.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tbNumberOfRecordsToPreview.Name = "tbNumberOfRecordsToPreview";
            this.tbNumberOfRecordsToPreview.Size = new System.Drawing.Size(120, 20);
            this.tbNumberOfRecordsToPreview.TabIndex = 10;
            this.ttSplitter.SetToolTip(this.tbNumberOfRecordsToPreview, "Select the number of records that will be read into the preview grid.");
            this.tbNumberOfRecordsToPreview.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(81, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Column Delimiter";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(95, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Text Delimiter";
            // 
            // tbTextDelimiter
            // 
            this.tbTextDelimiter.Location = new System.Drawing.Point(175, 99);
            this.tbTextDelimiter.Name = "tbTextDelimiter";
            this.tbTextDelimiter.Size = new System.Drawing.Size(237, 20);
            this.tbTextDelimiter.TabIndex = 7;
            this.ttSplitter.SetToolTip(this.tbTextDelimiter, "Enter the text delimiter that is used in the data records");
            this.tbTextDelimiter.TextChanged += new System.EventHandler(this.tbTextDelimiter_TextChanged);
            // 
            // cbTreatNulls
            // 
            this.cbTreatNulls.AutoSize = true;
            this.cbTreatNulls.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbTreatNulls.Location = new System.Drawing.Point(35, 125);
            this.cbTreatNulls.Name = "cbTreatNulls";
            this.cbTreatNulls.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbTreatNulls.Size = new System.Drawing.Size(154, 17);
            this.cbTreatNulls.TabIndex = 6;
            this.cbTreatNulls.Text = "Treat Empty String as Null?";
            this.ttSplitter.SetToolTip(this.cbTreatNulls, "Should empty strings be turned into Nulls?");
            this.cbTreatNulls.UseVisualStyleBackColor = true;
            this.cbTreatNulls.CheckedChanged += new System.EventHandler(this.cbTreatNulls_CheckedChanged);
            // 
            // tbColumnDelimiter
            // 
            this.tbColumnDelimiter.Location = new System.Drawing.Point(175, 49);
            this.tbColumnDelimiter.Name = "tbColumnDelimiter";
            this.tbColumnDelimiter.Size = new System.Drawing.Size(237, 20);
            this.tbColumnDelimiter.TabIndex = 5;
            this.ttSplitter.SetToolTip(this.tbColumnDelimiter, "Enter the column delimiter for the data records");
            this.tbColumnDelimiter.TextChanged += new System.EventHandler(this.tbColumnDelimiter_TextChanged);
            // 
            // cbDelimitedText
            // 
            this.cbDelimitedText.AutoSize = true;
            this.cbDelimitedText.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbDelimitedText.Location = new System.Drawing.Point(90, 75);
            this.cbDelimitedText.Name = "cbDelimitedText";
            this.cbDelimitedText.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbDelimitedText.Size = new System.Drawing.Size(99, 17);
            this.cbDelimitedText.TabIndex = 4;
            this.cbDelimitedText.Text = "Delimited Text?";
            this.ttSplitter.SetToolTip(this.cbDelimitedText, "Are the data records delimited?");
            this.cbDelimitedText.UseVisualStyleBackColor = true;
            this.cbDelimitedText.CheckedChanged += new System.EventHandler(this.cbDelimitedText_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(60, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Connection Manager";
            // 
            // cbConnectionManager
            // 
            this.cbConnectionManager.FormattingEnabled = true;
            this.cbConnectionManager.Location = new System.Drawing.Point(175, 21);
            this.cbConnectionManager.Name = "cbConnectionManager";
            this.cbConnectionManager.Size = new System.Drawing.Size(237, 21);
            this.cbConnectionManager.TabIndex = 2;
            this.ttSplitter.SetToolTip(this.cbConnectionManager, "Select the Connection Manager to use.");
            this.cbConnectionManager.SelectedIndexChanged += new System.EventHandler(this.cbConnectionManager_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(160, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select the Connection Manager:";
            // 
            // btnPreview
            // 
            this.btnPreview.Location = new System.Drawing.Point(548, 125);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 23);
            this.btnPreview.TabIndex = 0;
            this.btnPreview.Text = "Preview";
            this.ttSplitter.SetToolTip(this.btnPreview, "Click this to read the data from the Connection Manager and display it in the Pre" +
        "view below.");
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // tpPassThrough
            // 
            this.tpPassThrough.Controls.Add(this.dgvPassThroughPreview);
            this.tpPassThrough.Controls.Add(this.panel3);
            this.tpPassThrough.Controls.Add(this.dgvPassThrough);
            this.tpPassThrough.Location = new System.Drawing.Point(4, 22);
            this.tpPassThrough.Name = "tpPassThrough";
            this.tpPassThrough.Padding = new System.Windows.Forms.Padding(3);
            this.tpPassThrough.Size = new System.Drawing.Size(634, 357);
            this.tpPassThrough.TabIndex = 1;
            this.tpPassThrough.Text = "Pass Through";
            this.tpPassThrough.UseVisualStyleBackColor = true;
            // 
            // dgvPassThroughPreview
            // 
            this.dgvPassThroughPreview.AllowUserToAddRows = false;
            this.dgvPassThroughPreview.AllowUserToDeleteRows = false;
            this.dgvPassThroughPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPassThroughPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPassThroughPreview.Location = new System.Drawing.Point(3, 185);
            this.dgvPassThroughPreview.Name = "dgvPassThroughPreview";
            this.dgvPassThroughPreview.ReadOnly = true;
            this.dgvPassThroughPreview.Size = new System.Drawing.Size(628, 169);
            this.dgvPassThroughPreview.TabIndex = 2;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.cbPTErrorDisposition);
            this.panel3.Controls.Add(this.label6);
            this.panel3.Controls.Add(this.tbPTNumberOfRecordsToPreview);
            this.panel3.Controls.Add(this.btnPTPreview);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(3, 153);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(628, 32);
            this.panel3.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 8);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Error Disposition";
            // 
            // cbPTErrorDisposition
            // 
            this.cbPTErrorDisposition.FormattingEnabled = true;
            this.cbPTErrorDisposition.Location = new System.Drawing.Point(95, 5);
            this.cbPTErrorDisposition.Name = "cbPTErrorDisposition";
            this.cbPTErrorDisposition.Size = new System.Drawing.Size(175, 21);
            this.cbPTErrorDisposition.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(276, 8);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(140, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Number of Preview Records";
            // 
            // tbPTNumberOfRecordsToPreview
            // 
            this.tbPTNumberOfRecordsToPreview.Location = new System.Drawing.Point(422, 6);
            this.tbPTNumberOfRecordsToPreview.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.tbPTNumberOfRecordsToPreview.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tbPTNumberOfRecordsToPreview.Name = "tbPTNumberOfRecordsToPreview";
            this.tbPTNumberOfRecordsToPreview.Size = new System.Drawing.Size(120, 20);
            this.tbPTNumberOfRecordsToPreview.TabIndex = 13;
            this.ttSplitter.SetToolTip(this.tbPTNumberOfRecordsToPreview, "Select the number of records that will be read into the preview grid.");
            this.tbPTNumberOfRecordsToPreview.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // btnPTPreview
            // 
            this.btnPTPreview.Location = new System.Drawing.Point(548, 3);
            this.btnPTPreview.Name = "btnPTPreview";
            this.btnPTPreview.Size = new System.Drawing.Size(75, 23);
            this.btnPTPreview.TabIndex = 12;
            this.btnPTPreview.Text = "Preview";
            this.ttSplitter.SetToolTip(this.btnPTPreview, "Click this to read the data from the Connection Manager and display it in the Pre" +
        "view below.");
            this.btnPTPreview.UseVisualStyleBackColor = true;
            // 
            // dgvPassThrough
            // 
            this.dgvPassThrough.AllowUserToAddRows = false;
            this.dgvPassThrough.AllowUserToDeleteRows = false;
            this.dgvPassThrough.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPassThrough.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmPTColumnName,
            this.clmPTUsageOfColumn,
            this.clmPTCodePage,
            this.clmPTDataType,
            this.clmPTLength,
            this.clmPTPrecision,
            this.clmPTScale});
            this.dgvPassThrough.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvPassThrough.Location = new System.Drawing.Point(3, 3);
            this.dgvPassThrough.Name = "dgvPassThrough";
            this.dgvPassThrough.Size = new System.Drawing.Size(628, 150);
            this.dgvPassThrough.TabIndex = 0;
            // 
            // clmPTColumnName
            // 
            this.clmPTColumnName.HeaderText = "Column Name";
            this.clmPTColumnName.Name = "clmPTColumnName";
            // 
            // clmPTUsageOfColumn
            // 
            this.clmPTUsageOfColumn.HeaderText = "Usage";
            this.clmPTUsageOfColumn.Name = "clmPTUsageOfColumn";
            // 
            // clmPTCodePage
            // 
            this.clmPTCodePage.HeaderText = "CodePage";
            this.clmPTCodePage.Name = "clmPTCodePage";
            // 
            // clmPTDataType
            // 
            this.clmPTDataType.HeaderText = "Data Type";
            this.clmPTDataType.Name = "clmPTDataType";
            // 
            // clmPTLength
            // 
            this.clmPTLength.HeaderText = "Length";
            this.clmPTLength.Name = "clmPTLength";
            // 
            // clmPTPrecision
            // 
            this.clmPTPrecision.HeaderText = "Precision";
            this.clmPTPrecision.Name = "clmPTPrecision";
            // 
            // clmPTScale
            // 
            this.clmPTScale.HeaderText = "Scale";
            this.clmPTScale.Name = "clmPTScale";
            // 
            // tpOutputs
            // 
            this.tpOutputs.Controls.Add(this.splitContainer1);
            this.tpOutputs.Controls.Add(this.panel7);
            this.tpOutputs.Controls.Add(this.panel4);
            this.tpOutputs.Location = new System.Drawing.Point(4, 22);
            this.tpOutputs.Name = "tpOutputs";
            this.tpOutputs.Padding = new System.Windows.Forms.Padding(3);
            this.tpOutputs.Size = new System.Drawing.Size(634, 357);
            this.tpOutputs.TabIndex = 2;
            this.tpOutputs.Text = "Outputs";
            this.tpOutputs.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(170, 88);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgvOutputColumns);
            this.splitContainer1.Panel1.Controls.Add(this.panel8);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dgvOutputPreview);
            this.splitContainer1.Size = new System.Drawing.Size(461, 266);
            this.splitContainer1.SplitterDistance = 150;
            this.splitContainer1.TabIndex = 2;
            // 
            // dgvOutputColumns
            // 
            this.dgvOutputColumns.AllowUserToAddRows = false;
            this.dgvOutputColumns.AllowUserToDeleteRows = false;
            this.dgvOutputColumns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOutputColumns.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmOCColumnName,
            this.clmOCUsage,
            this.clmOCdotNetFormatString,
            this.clmOCCodePage,
            this.clmOCDataType,
            this.clmOCLength,
            this.clmOCPrecision,
            this.clmOCScale});
            this.dgvOutputColumns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvOutputColumns.Location = new System.Drawing.Point(0, 0);
            this.dgvOutputColumns.Name = "dgvOutputColumns";
            this.dgvOutputColumns.Size = new System.Drawing.Size(461, 120);
            this.dgvOutputColumns.TabIndex = 3;
            this.dgvOutputColumns.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvOutputColumns_DataError);
            // 
            // clmOCColumnName
            // 
            this.clmOCColumnName.HeaderText = "Column Name";
            this.clmOCColumnName.Name = "clmOCColumnName";
            // 
            // clmOCUsage
            // 
            this.clmOCUsage.HeaderText = "Usage";
            this.clmOCUsage.Name = "clmOCUsage";
            // 
            // clmOCdotNetFormatString
            // 
            this.clmOCdotNetFormatString.HeaderText = "Format String";
            this.clmOCdotNetFormatString.Name = "clmOCdotNetFormatString";
            // 
            // clmOCCodePage
            // 
            this.clmOCCodePage.HeaderText = "CodePage";
            this.clmOCCodePage.Name = "clmOCCodePage";
            // 
            // clmOCDataType
            // 
            this.clmOCDataType.HeaderText = "Data Type";
            this.clmOCDataType.Name = "clmOCDataType";
            // 
            // clmOCLength
            // 
            this.clmOCLength.HeaderText = "Length";
            this.clmOCLength.Name = "clmOCLength";
            // 
            // clmOCPrecision
            // 
            this.clmOCPrecision.HeaderText = "Precision";
            this.clmOCPrecision.Name = "clmOCPrecision";
            // 
            // clmOCScale
            // 
            this.clmOCScale.HeaderText = "Scale";
            this.clmOCScale.Name = "clmOCScale";
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.btnAddColumn);
            this.panel8.Controls.Add(this.btnRemoveColumn);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel8.Location = new System.Drawing.Point(0, 120);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(461, 30);
            this.panel8.TabIndex = 2;
            // 
            // btnAddColumn
            // 
            this.btnAddColumn.Location = new System.Drawing.Point(3, 3);
            this.btnAddColumn.Name = "btnAddColumn";
            this.btnAddColumn.Size = new System.Drawing.Size(75, 23);
            this.btnAddColumn.TabIndex = 0;
            this.btnAddColumn.Text = "Add";
            this.btnAddColumn.UseVisualStyleBackColor = true;
            // 
            // btnRemoveColumn
            // 
            this.btnRemoveColumn.Location = new System.Drawing.Point(84, 3);
            this.btnRemoveColumn.Name = "btnRemoveColumn";
            this.btnRemoveColumn.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveColumn.TabIndex = 1;
            this.btnRemoveColumn.Text = "Remove";
            this.btnRemoveColumn.UseVisualStyleBackColor = true;
            // 
            // dgvOutputPreview
            // 
            this.dgvOutputPreview.AllowUserToAddRows = false;
            this.dgvOutputPreview.AllowUserToDeleteRows = false;
            this.dgvOutputPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOutputPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvOutputPreview.Location = new System.Drawing.Point(0, 0);
            this.dgvOutputPreview.Name = "dgvOutputPreview";
            this.dgvOutputPreview.ReadOnly = true;
            this.dgvOutputPreview.Size = new System.Drawing.Size(461, 112);
            this.dgvOutputPreview.TabIndex = 0;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.label11);
            this.panel7.Controls.Add(this.cbOutputType);
            this.panel7.Controls.Add(this.label10);
            this.panel7.Controls.Add(this.tbRowTypeValue);
            this.panel7.Controls.Add(this.tbOutputName);
            this.panel7.Controls.Add(this.label9);
            this.panel7.Controls.Add(this.label8);
            this.panel7.Controls.Add(this.cbOutputDisposition);
            this.panel7.Controls.Add(this.tbOutputNumberOfRecordsToPreview);
            this.panel7.Controls.Add(this.btnOutputPreview);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel7.Location = new System.Drawing.Point(170, 3);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(461, 85);
            this.panel7.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(255, 8);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(31, 13);
            this.label11.TabIndex = 27;
            this.label11.Text = "Type";
            // 
            // cbOutputType
            // 
            this.cbOutputType.FormattingEnabled = true;
            this.cbOutputType.Location = new System.Drawing.Point(337, 3);
            this.cbOutputType.Name = "cbOutputType";
            this.cbOutputType.Size = new System.Drawing.Size(121, 21);
            this.cbOutputType.TabIndex = 26;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 33);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(76, 13);
            this.label10.TabIndex = 25;
            this.label10.Text = "Selector Value";
            // 
            // tbRowTypeValue
            // 
            this.tbRowTypeValue.Location = new System.Drawing.Point(92, 30);
            this.tbRowTypeValue.Name = "tbRowTypeValue";
            this.tbRowTypeValue.Size = new System.Drawing.Size(157, 20);
            this.tbRowTypeValue.TabIndex = 24;
            // 
            // tbOutputName
            // 
            this.tbOutputName.Location = new System.Drawing.Point(92, 3);
            this.tbOutputName.Name = "tbOutputName";
            this.tbOutputName.Size = new System.Drawing.Size(157, 20);
            this.tbOutputName.TabIndex = 23;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 8);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(70, 13);
            this.label9.TabIndex = 22;
            this.label9.Text = "Output Name";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 59);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(83, 13);
            this.label8.TabIndex = 21;
            this.label8.Text = "Error Disposition";
            // 
            // cbOutputDisposition
            // 
            this.cbOutputDisposition.FormattingEnabled = true;
            this.cbOutputDisposition.Location = new System.Drawing.Point(92, 56);
            this.cbOutputDisposition.Name = "cbOutputDisposition";
            this.cbOutputDisposition.Size = new System.Drawing.Size(157, 21);
            this.cbOutputDisposition.TabIndex = 20;
            // 
            // tbOutputNumberOfRecordsToPreview
            // 
            this.tbOutputNumberOfRecordsToPreview.Location = new System.Drawing.Point(255, 56);
            this.tbOutputNumberOfRecordsToPreview.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.tbOutputNumberOfRecordsToPreview.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tbOutputNumberOfRecordsToPreview.Name = "tbOutputNumberOfRecordsToPreview";
            this.tbOutputNumberOfRecordsToPreview.Size = new System.Drawing.Size(120, 20);
            this.tbOutputNumberOfRecordsToPreview.TabIndex = 18;
            this.ttSplitter.SetToolTip(this.tbOutputNumberOfRecordsToPreview, "Select the number of records that will be read into the preview grid.");
            this.tbOutputNumberOfRecordsToPreview.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // btnOutputPreview
            // 
            this.btnOutputPreview.Location = new System.Drawing.Point(381, 53);
            this.btnOutputPreview.Name = "btnOutputPreview";
            this.btnOutputPreview.Size = new System.Drawing.Size(75, 23);
            this.btnOutputPreview.TabIndex = 17;
            this.btnOutputPreview.Text = "Preview";
            this.ttSplitter.SetToolTip(this.btnOutputPreview, "Click this to read the data from the Connection Manager and display it in the Pre" +
        "view below.");
            this.btnOutputPreview.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.lbOutputs);
            this.panel4.Controls.Add(this.panel6);
            this.panel4.Controls.Add(this.panel5);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel4.Location = new System.Drawing.Point(3, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(167, 351);
            this.panel4.TabIndex = 0;
            // 
            // lbOutputs
            // 
            this.lbOutputs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbOutputs.FormattingEnabled = true;
            this.lbOutputs.Location = new System.Drawing.Point(0, 30);
            this.lbOutputs.Name = "lbOutputs";
            this.lbOutputs.Size = new System.Drawing.Size(167, 291);
            this.lbOutputs.TabIndex = 4;
            this.lbOutputs.SelectedIndexChanged += new System.EventHandler(this.lbOutputs_SelectedIndexChanged);
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.btnGenerateOutputs);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(167, 30);
            this.panel6.TabIndex = 3;
            // 
            // btnGenerateOutputs
            // 
            this.btnGenerateOutputs.Location = new System.Drawing.Point(3, 3);
            this.btnGenerateOutputs.Name = "btnGenerateOutputs";
            this.btnGenerateOutputs.Size = new System.Drawing.Size(156, 23);
            this.btnGenerateOutputs.TabIndex = 2;
            this.btnGenerateOutputs.Text = "Generate Outputs";
            this.btnGenerateOutputs.UseVisualStyleBackColor = true;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.btnAddOutput);
            this.panel5.Controls.Add(this.btnRemoveOutput);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel5.Location = new System.Drawing.Point(0, 321);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(167, 30);
            this.panel5.TabIndex = 1;
            // 
            // btnAddOutput
            // 
            this.btnAddOutput.Location = new System.Drawing.Point(3, 3);
            this.btnAddOutput.Name = "btnAddOutput";
            this.btnAddOutput.Size = new System.Drawing.Size(75, 23);
            this.btnAddOutput.TabIndex = 0;
            this.btnAddOutput.Text = "Add";
            this.btnAddOutput.UseVisualStyleBackColor = true;
            // 
            // btnRemoveOutput
            // 
            this.btnRemoveOutput.Location = new System.Drawing.Point(84, 3);
            this.btnRemoveOutput.Name = "btnRemoveOutput";
            this.btnRemoveOutput.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveOutput.TabIndex = 1;
            this.btnRemoveOutput.Text = "Remove";
            this.btnRemoveOutput.UseVisualStyleBackColor = true;
            // 
            // tpAbout
            // 
            this.tpAbout.Controls.Add(this.richTextBox1);
            this.tpAbout.Controls.Add(this.pictureBox2);
            this.tpAbout.Location = new System.Drawing.Point(4, 22);
            this.tpAbout.Name = "tpAbout";
            this.tpAbout.Padding = new System.Windows.Forms.Padding(3);
            this.tpAbout.Size = new System.Drawing.Size(634, 357);
            this.tpAbout.TabIndex = 3;
            this.tpAbout.Text = "About";
            this.tpAbout.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(8, 6);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(436, 345);
            this.richTextBox1.TabIndex = 14;
            this.richTextBox1.Text = "This component was written by Keith Martin.\nIt utilises a third party .Net Librar" +
    "y FileHelpers.\nSee http://filehelpers.sourceforge.net/ for details.";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(450, 6);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(176, 154);
            this.pictureBox2.TabIndex = 13;
            this.pictureBox2.TabStop = false;
            // 
            // TextFileSplitterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 438);
            this.Controls.Add(this.tcMain);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(658, 476);
            this.Name = "TextFileSplitterForm";
            this.Text = "Text File Splitter Source";
            this.Load += new System.EventHandler(this.TextFileSplitterForm_Load);
            this.panel1.ResumeLayout(false);
            this.tcMain.ResumeLayout(false);
            this.tpConnection.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvConnectionPreview)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbNumberOfRecordsToPreview)).EndInit();
            this.tpPassThrough.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPassThroughPreview)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbPTNumberOfRecordsToPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPassThrough)).EndInit();
            this.tpOutputs.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOutputColumns)).EndInit();
            this.panel8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOutputPreview)).EndInit();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbOutputNumberOfRecordsToPreview)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.tpAbout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tpConnection;
        private System.Windows.Forms.DataGridView dgvConnectionPreview;
        private System.Windows.Forms.ToolTip ttSplitter;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown tbNumberOfRecordsToPreview;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbTextDelimiter;
        private System.Windows.Forms.CheckBox cbTreatNulls;
        private System.Windows.Forms.TextBox tbColumnDelimiter;
        private System.Windows.Forms.CheckBox cbDelimitedText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbConnectionManager;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.TabPage tpPassThrough;
        private System.Windows.Forms.TabPage tpOutputs;
        private System.Windows.Forms.DataGridView dgvPassThroughPreview;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown tbPTNumberOfRecordsToPreview;
        private System.Windows.Forms.Button btnPTPreview;
        private System.Windows.Forms.DataGridView dgvPassThrough;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cbPTErrorDisposition;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmPTColumnName;
        private System.Windows.Forms.DataGridViewComboBoxColumn clmPTUsageOfColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmPTCodePage;
        private System.Windows.Forms.DataGridViewComboBoxColumn clmPTDataType;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmPTLength;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmPTPrecision;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmPTScale;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnRemoveOutput;
        private System.Windows.Forms.Button btnAddOutput;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Button btnAddColumn;
        private System.Windows.Forms.Button btnRemoveColumn;
        private System.Windows.Forms.DataGridView dgvOutputPreview;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbRowTypeValue;
        private System.Windows.Forms.TextBox tbOutputName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbOutputDisposition;
        private System.Windows.Forms.NumericUpDown tbOutputNumberOfRecordsToPreview;
        private System.Windows.Forms.Button btnOutputPreview;
        private System.Windows.Forms.ListBox lbOutputs;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Button btnGenerateOutputs;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.DataGridView dgvOutputColumns;
        private System.Windows.Forms.TabPage tpAbout;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmOCColumnName;
        private System.Windows.Forms.DataGridViewComboBoxColumn clmOCUsage;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmOCdotNetFormatString;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmOCCodePage;
        private System.Windows.Forms.DataGridViewComboBoxColumn clmOCDataType;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmOCLength;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmOCPrecision;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmOCScale;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cbOutputType;
    }
}