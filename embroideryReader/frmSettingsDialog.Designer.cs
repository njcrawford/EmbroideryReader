namespace embroideryReader
{
    partial class frmSettingsDialog
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
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.btnColor = new System.Windows.Forms.Button();
            this.lblColor = new System.Windows.Forms.Label();
            this.btnResetColor = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblThreadThickness = new System.Windows.Forms.Label();
            this.txtThreadThickness = new System.Windows.Forms.TextBox();
            this.lblPixelThick = new System.Windows.Forms.Label();
            this.grpBackground = new System.Windows.Forms.GroupBox();
            this.grpStitch = new System.Windows.Forms.GroupBox();
            this.chkDrawGrid = new System.Windows.Forms.CheckBox();
            this.lblUglyLength = new System.Windows.Forms.Label();
            this.lblPixelLength = new System.Windows.Forms.Label();
            this.txtThreshold = new System.Windows.Forms.TextBox();
            this.chkUglyStitches = new System.Windows.Forms.CheckBox();
            this.cmbLanguage = new System.Windows.Forms.ComboBox();
            this.grpLanguage = new System.Windows.Forms.GroupBox();
            this.grpBackground.SuspendLayout();
            this.grpStitch.SuspendLayout();
            this.grpLanguage.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnColor
            // 
            this.btnColor.Location = new System.Drawing.Point(171, 16);
            this.btnColor.Name = "btnColor";
            this.btnColor.Size = new System.Drawing.Size(75, 23);
            this.btnColor.TabIndex = 0;
            this.btnColor.Text = "Pick Color...";
            this.btnColor.UseVisualStyleBackColor = true;
            this.btnColor.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // lblColor
            // 
            this.lblColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblColor.Location = new System.Drawing.Point(6, 16);
            this.lblColor.Name = "lblColor";
            this.lblColor.Size = new System.Drawing.Size(130, 52);
            this.lblColor.TabIndex = 1;
            this.lblColor.Text = "Background Color";
            this.lblColor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnResetColor
            // 
            this.btnResetColor.Location = new System.Drawing.Point(171, 45);
            this.btnResetColor.Name = "btnResetColor";
            this.btnResetColor.Size = new System.Drawing.Size(75, 23);
            this.btnResetColor.TabIndex = 2;
            this.btnResetColor.Text = "Reset Color";
            this.btnResetColor.UseVisualStyleBackColor = true;
            this.btnResetColor.Click += new System.EventHandler(this.btnResetColor_Click);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(128, 284);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(209, 284);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblThreadThickness
            // 
            this.lblThreadThickness.AutoSize = true;
            this.lblThreadThickness.Location = new System.Drawing.Point(6, 16);
            this.lblThreadThickness.Name = "lblThreadThickness";
            this.lblThreadThickness.Size = new System.Drawing.Size(92, 13);
            this.lblThreadThickness.TabIndex = 5;
            this.lblThreadThickness.Text = "Thread thickness:";
            // 
            // txtThreadThickness
            // 
            this.txtThreadThickness.Location = new System.Drawing.Point(104, 13);
            this.txtThreadThickness.Name = "txtThreadThickness";
            this.txtThreadThickness.Size = new System.Drawing.Size(32, 20);
            this.txtThreadThickness.TabIndex = 6;
            // 
            // lblPixelThick
            // 
            this.lblPixelThick.AutoSize = true;
            this.lblPixelThick.Location = new System.Drawing.Point(142, 16);
            this.lblPixelThick.Name = "lblPixelThick";
            this.lblPixelThick.Size = new System.Drawing.Size(33, 13);
            this.lblPixelThick.TabIndex = 7;
            this.lblPixelThick.Text = "pixels";
            // 
            // grpBackground
            // 
            this.grpBackground.Controls.Add(this.lblColor);
            this.grpBackground.Controls.Add(this.btnColor);
            this.grpBackground.Controls.Add(this.btnResetColor);
            this.grpBackground.Location = new System.Drawing.Point(12, 12);
            this.grpBackground.Name = "grpBackground";
            this.grpBackground.Size = new System.Drawing.Size(271, 79);
            this.grpBackground.TabIndex = 8;
            this.grpBackground.TabStop = false;
            this.grpBackground.Text = "Background";
            // 
            // grpStitch
            // 
            this.grpStitch.Controls.Add(this.chkDrawGrid);
            this.grpStitch.Controls.Add(this.lblUglyLength);
            this.grpStitch.Controls.Add(this.lblPixelLength);
            this.grpStitch.Controls.Add(this.txtThreshold);
            this.grpStitch.Controls.Add(this.chkUglyStitches);
            this.grpStitch.Controls.Add(this.lblThreadThickness);
            this.grpStitch.Controls.Add(this.txtThreadThickness);
            this.grpStitch.Controls.Add(this.lblPixelThick);
            this.grpStitch.Location = new System.Drawing.Point(12, 97);
            this.grpStitch.Name = "grpStitch";
            this.grpStitch.Size = new System.Drawing.Size(271, 108);
            this.grpStitch.TabIndex = 3;
            this.grpStitch.TabStop = false;
            this.grpStitch.Text = "Stitch drawing";
            // 
            // chkDrawGrid
            // 
            this.chkDrawGrid.AutoSize = true;
            this.chkDrawGrid.Location = new System.Drawing.Point(9, 81);
            this.chkDrawGrid.Name = "chkDrawGrid";
            this.chkDrawGrid.Size = new System.Drawing.Size(131, 17);
            this.chkDrawGrid.TabIndex = 12;
            this.chkDrawGrid.Text = "Draw background grid";
            this.chkDrawGrid.UseVisualStyleBackColor = true;
            // 
            // lblUglyLength
            // 
            this.lblUglyLength.AutoSize = true;
            this.lblUglyLength.Location = new System.Drawing.Point(57, 58);
            this.lblUglyLength.Name = "lblUglyLength";
            this.lblUglyLength.Size = new System.Drawing.Size(67, 13);
            this.lblUglyLength.TabIndex = 11;
            this.lblUglyLength.Text = "Ugly Length:";
            // 
            // lblPixelLength
            // 
            this.lblPixelLength.AutoSize = true;
            this.lblPixelLength.Location = new System.Drawing.Point(168, 58);
            this.lblPixelLength.Name = "lblPixelLength";
            this.lblPixelLength.Size = new System.Drawing.Size(33, 13);
            this.lblPixelLength.TabIndex = 10;
            this.lblPixelLength.Text = "pixels";
            // 
            // txtThreshold
            // 
            this.txtThreshold.Location = new System.Drawing.Point(130, 55);
            this.txtThreshold.Name = "txtThreshold";
            this.txtThreshold.Size = new System.Drawing.Size(32, 20);
            this.txtThreshold.TabIndex = 9;
            // 
            // chkUglyStitches
            // 
            this.chkUglyStitches.AutoSize = true;
            this.chkUglyStitches.Location = new System.Drawing.Point(9, 38);
            this.chkUglyStitches.Name = "chkUglyStitches";
            this.chkUglyStitches.Size = new System.Drawing.Size(131, 17);
            this.chkUglyStitches.TabIndex = 8;
            this.chkUglyStitches.Text = "Remove \'ugly\' stitches";
            this.chkUglyStitches.UseVisualStyleBackColor = true;
            // 
            // cmbLanguage
            // 
            this.cmbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLanguage.FormattingEnabled = true;
            this.cmbLanguage.Location = new System.Drawing.Point(14, 19);
            this.cmbLanguage.Name = "cmbLanguage";
            this.cmbLanguage.Size = new System.Drawing.Size(121, 21);
            this.cmbLanguage.TabIndex = 10;
            this.cmbLanguage.SelectedIndexChanged += new System.EventHandler(this.cmbLanguage_SelectedIndexChanged);
            // 
            // grpLanguage
            // 
            this.grpLanguage.Controls.Add(this.cmbLanguage);
            this.grpLanguage.Location = new System.Drawing.Point(13, 212);
            this.grpLanguage.Name = "grpLanguage";
            this.grpLanguage.Size = new System.Drawing.Size(270, 56);
            this.grpLanguage.TabIndex = 11;
            this.grpLanguage.TabStop = false;
            this.grpLanguage.Text = "Language";
            // 
            // frmSettingsDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(296, 319);
            this.Controls.Add(this.grpLanguage);
            this.Controls.Add(this.grpStitch);
            this.Controls.Add(this.grpBackground);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSettingsDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Embroidery Reader Settings";
            this.grpBackground.ResumeLayout(false);
            this.grpStitch.ResumeLayout(false);
            this.grpStitch.PerformLayout();
            this.grpLanguage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button btnColor;
        private System.Windows.Forms.Label lblColor;
        private System.Windows.Forms.Button btnResetColor;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblThreadThickness;
        private System.Windows.Forms.TextBox txtThreadThickness;
        private System.Windows.Forms.Label lblPixelThick;
        private System.Windows.Forms.GroupBox grpBackground;
        private System.Windows.Forms.GroupBox grpStitch;
        private System.Windows.Forms.Label lblPixelLength;
        private System.Windows.Forms.TextBox txtThreshold;
        private System.Windows.Forms.CheckBox chkUglyStitches;
        private System.Windows.Forms.Label lblUglyLength;
        private System.Windows.Forms.CheckBox chkDrawGrid;
        private System.Windows.Forms.ComboBox cmbLanguage;
        private System.Windows.Forms.GroupBox grpLanguage;
    }
}
