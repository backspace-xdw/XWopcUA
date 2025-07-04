namespace XWopcUA.Forms
{
    partial class CertificateManager
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabTrusted;
        private System.Windows.Forms.TabPage tabRejected;
        private System.Windows.Forms.ListView lstTrusted;
        private System.Windows.Forms.ListView lstRejected;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnMoveTo;
        private System.Windows.Forms.Button btnCreateSelfSigned;
        private System.Windows.Forms.Button btnViewDetails;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabTrusted = new System.Windows.Forms.TabPage();
            this.lstTrusted = new System.Windows.Forms.ListView();
            this.tabRejected = new System.Windows.Forms.TabPage();
            this.lstRejected = new System.Windows.Forms.ListView();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnMoveTo = new System.Windows.Forms.Button();
            this.btnCreateSelfSigned = new System.Windows.Forms.Button();
            this.btnViewDetails = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl.SuspendLayout();
            this.tabTrusted.SuspendLayout();
            this.tabRejected.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabTrusted);
            this.tabControl.Controls.Add(this.tabRejected);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(760, 400);
            this.tabControl.TabIndex = 0;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabTrusted
            // 
            this.tabTrusted.Controls.Add(this.lstTrusted);
            this.tabTrusted.Location = new System.Drawing.Point(4, 22);
            this.tabTrusted.Name = "tabTrusted";
            this.tabTrusted.Padding = new System.Windows.Forms.Padding(3);
            this.tabTrusted.Size = new System.Drawing.Size(752, 374);
            this.tabTrusted.TabIndex = 0;
            this.tabTrusted.Text = "Trusted Certificates";
            this.tabTrusted.UseVisualStyleBackColor = true;
            // 
            // lstTrusted
            // 
            this.lstTrusted.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstTrusted.FullRowSelect = true;
            this.lstTrusted.GridLines = true;
            this.lstTrusted.HideSelection = false;
            this.lstTrusted.Location = new System.Drawing.Point(3, 3);
            this.lstTrusted.Name = "lstTrusted";
            this.lstTrusted.Size = new System.Drawing.Size(746, 368);
            this.lstTrusted.TabIndex = 0;
            this.lstTrusted.UseCompatibleStateImageBehavior = false;
            this.lstTrusted.View = System.Windows.Forms.View.Details;
            this.lstTrusted.Columns.Add("Subject", 200);
            this.lstTrusted.Columns.Add("Thumbprint", 300);
            this.lstTrusted.Columns.Add("Expiry Date", 100);
            this.lstTrusted.Columns.Add("Issuer", 200);
            // 
            // tabRejected
            // 
            this.tabRejected.Controls.Add(this.lstRejected);
            this.tabRejected.Location = new System.Drawing.Point(4, 22);
            this.tabRejected.Name = "tabRejected";
            this.tabRejected.Padding = new System.Windows.Forms.Padding(3);
            this.tabRejected.Size = new System.Drawing.Size(752, 374);
            this.tabRejected.TabIndex = 1;
            this.tabRejected.Text = "Rejected Certificates";
            this.tabRejected.UseVisualStyleBackColor = true;
            // 
            // lstRejected
            // 
            this.lstRejected.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstRejected.FullRowSelect = true;
            this.lstRejected.GridLines = true;
            this.lstRejected.HideSelection = false;
            this.lstRejected.Location = new System.Drawing.Point(3, 3);
            this.lstRejected.Name = "lstRejected";
            this.lstRejected.Size = new System.Drawing.Size(746, 368);
            this.lstRejected.TabIndex = 0;
            this.lstRejected.UseCompatibleStateImageBehavior = false;
            this.lstRejected.View = System.Windows.Forms.View.Details;
            this.lstRejected.Columns.Add("Subject", 200);
            this.lstRejected.Columns.Add("Thumbprint", 300);
            this.lstRejected.Columns.Add("Expiry Date", 100);
            this.lstRejected.Columns.Add("Issuer", 200);
            // 
            // btnImport
            // 
            this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnImport.Location = new System.Drawing.Point(12, 418);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(100, 25);
            this.btnImport.TabIndex = 1;
            this.btnImport.Text = "Import...";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemove.Location = new System.Drawing.Point(118, 418);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(100, 25);
            this.btnRemove.TabIndex = 2;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnMoveTo
            // 
            this.btnMoveTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnMoveTo.Location = new System.Drawing.Point(224, 418);
            this.btnMoveTo.Name = "btnMoveTo";
            this.btnMoveTo.Size = new System.Drawing.Size(120, 25);
            this.btnMoveTo.TabIndex = 3;
            this.btnMoveTo.Text = "Move to Rejected";
            this.btnMoveTo.UseVisualStyleBackColor = true;
            this.btnMoveTo.Click += new System.EventHandler(this.btnMoveTo_Click);
            // 
            // btnCreateSelfSigned
            // 
            this.btnCreateSelfSigned.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateSelfSigned.Location = new System.Drawing.Point(552, 418);
            this.btnCreateSelfSigned.Name = "btnCreateSelfSigned";
            this.btnCreateSelfSigned.Size = new System.Drawing.Size(110, 25);
            this.btnCreateSelfSigned.TabIndex = 4;
            this.btnCreateSelfSigned.Text = "Create Self-Signed";
            this.btnCreateSelfSigned.UseVisualStyleBackColor = true;
            this.btnCreateSelfSigned.Click += new System.EventHandler(this.btnCreateSelfSigned_Click);
            // 
            // btnViewDetails
            // 
            this.btnViewDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnViewDetails.Location = new System.Drawing.Point(668, 418);
            this.btnViewDetails.Name = "btnViewDetails";
            this.btnViewDetails.Size = new System.Drawing.Size(100, 25);
            this.btnViewDetails.TabIndex = 5;
            this.btnViewDetails.Text = "View Details";
            this.btnViewDetails.UseVisualStyleBackColor = true;
            this.btnViewDetails.Click += new System.EventHandler(this.btnViewDetails_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 455);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(784, 22);
            this.statusStrip.TabIndex = 6;
            this.statusStrip.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(108, 17);
            this.lblStatus.Text = "Trusted: 0 | Rejected: 0";
            // 
            // CertificateManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 477);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.btnViewDetails);
            this.Controls.Add(this.btnCreateSelfSigned);
            this.Controls.Add(this.btnMoveTo);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.tabControl);
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "CertificateManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Certificate Manager";
            this.tabControl.ResumeLayout(false);
            this.tabTrusted.ResumeLayout(false);
            this.tabRejected.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}