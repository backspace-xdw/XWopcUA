namespace XWopcUA.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.GroupBox grpConnection;
        private System.Windows.Forms.Label lblServerUrl;
        private System.Windows.Forms.TextBox txtServerUrl;
        private System.Windows.Forms.Label lblSecurityMode;
        private System.Windows.Forms.ComboBox cmbSecurityMode;
        private System.Windows.Forms.Label lblSecurityPolicy;
        private System.Windows.Forms.ComboBox cmbSecurityPolicy;
        private System.Windows.Forms.CheckBox chkUseAuth;
        private System.Windows.Forms.GroupBox grpAuthentication;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.CheckBox chkUseCertificate;
        private System.Windows.Forms.GroupBox grpCertificate;
        private System.Windows.Forms.Label lblCertPath;
        private System.Windows.Forms.TextBox txtCertificatePath;
        private System.Windows.Forms.Button btnBrowseCert;
        private System.Windows.Forms.Label lblCertPassword;
        private System.Windows.Forms.TextBox txtCertPassword;
        private System.Windows.Forms.Button btnCertManager;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblConnectionStatus;
        private System.Windows.Forms.GroupBox grpNodeOperations;
        private System.Windows.Forms.Label lblNodeId;
        private System.Windows.Forms.TextBox txtNodeId;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.Button btnWrite;
        private System.Windows.Forms.Button btnSubscribe;
        private System.Windows.Forms.GroupBox grpLogs;
        private System.Windows.Forms.ListBox lstLogs;

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
            this.grpConnection = new System.Windows.Forms.GroupBox();
            this.lblConnectionStatus = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.chkUseCertificate = new System.Windows.Forms.CheckBox();
            this.grpCertificate = new System.Windows.Forms.GroupBox();
            this.btnCertManager = new System.Windows.Forms.Button();
            this.txtCertPassword = new System.Windows.Forms.TextBox();
            this.lblCertPassword = new System.Windows.Forms.Label();
            this.btnBrowseCert = new System.Windows.Forms.Button();
            this.txtCertificatePath = new System.Windows.Forms.TextBox();
            this.lblCertPath = new System.Windows.Forms.Label();
            this.chkUseAuth = new System.Windows.Forms.CheckBox();
            this.grpAuthentication = new System.Windows.Forms.GroupBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.cmbSecurityPolicy = new System.Windows.Forms.ComboBox();
            this.lblSecurityPolicy = new System.Windows.Forms.Label();
            this.cmbSecurityMode = new System.Windows.Forms.ComboBox();
            this.lblSecurityMode = new System.Windows.Forms.Label();
            this.txtServerUrl = new System.Windows.Forms.TextBox();
            this.lblServerUrl = new System.Windows.Forms.Label();
            this.grpNodeOperations = new System.Windows.Forms.GroupBox();
            this.btnSubscribe = new System.Windows.Forms.Button();
            this.btnWrite = new System.Windows.Forms.Button();
            this.btnRead = new System.Windows.Forms.Button();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.lblValue = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtNodeId = new System.Windows.Forms.TextBox();
            this.lblNodeId = new System.Windows.Forms.Label();
            this.grpLogs = new System.Windows.Forms.GroupBox();
            this.lstLogs = new System.Windows.Forms.ListBox();
            this.grpConnection.SuspendLayout();
            this.grpCertificate.SuspendLayout();
            this.grpAuthentication.SuspendLayout();
            this.grpNodeOperations.SuspendLayout();
            this.grpLogs.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpConnection
            // 
            this.grpConnection.Controls.Add(this.lblConnectionStatus);
            this.grpConnection.Controls.Add(this.btnConnect);
            this.grpConnection.Controls.Add(this.chkUseCertificate);
            this.grpConnection.Controls.Add(this.grpCertificate);
            this.grpConnection.Controls.Add(this.chkUseAuth);
            this.grpConnection.Controls.Add(this.grpAuthentication);
            this.grpConnection.Controls.Add(this.cmbSecurityPolicy);
            this.grpConnection.Controls.Add(this.lblSecurityPolicy);
            this.grpConnection.Controls.Add(this.cmbSecurityMode);
            this.grpConnection.Controls.Add(this.lblSecurityMode);
            this.grpConnection.Controls.Add(this.txtServerUrl);
            this.grpConnection.Controls.Add(this.lblServerUrl);
            this.grpConnection.Location = new System.Drawing.Point(12, 12);
            this.grpConnection.Name = "grpConnection";
            this.grpConnection.Size = new System.Drawing.Size(580, 380);
            this.grpConnection.TabIndex = 0;
            this.grpConnection.TabStop = false;
            this.grpConnection.Text = "Connection Settings";
            // 
            // lblConnectionStatus
            // 
            this.lblConnectionStatus.AutoSize = true;
            this.lblConnectionStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblConnectionStatus.ForeColor = System.Drawing.Color.Red;
            this.lblConnectionStatus.Location = new System.Drawing.Point(380, 350);
            this.lblConnectionStatus.Name = "lblConnectionStatus";
            this.lblConnectionStatus.Size = new System.Drawing.Size(95, 15);
            this.lblConnectionStatus.TabIndex = 11;
            this.lblConnectionStatus.Text = "Disconnected";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(480, 345);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(90, 25);
            this.btnConnect.TabIndex = 10;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // chkUseCertificate
            // 
            this.chkUseCertificate.AutoSize = true;
            this.chkUseCertificate.Location = new System.Drawing.Point(10, 240);
            this.chkUseCertificate.Name = "chkUseCertificate";
            this.chkUseCertificate.Size = new System.Drawing.Size(95, 17);
            this.chkUseCertificate.TabIndex = 9;
            this.chkUseCertificate.Text = "Use Certificate";
            this.chkUseCertificate.UseVisualStyleBackColor = true;
            this.chkUseCertificate.CheckedChanged += new System.EventHandler(this.chkUseCertificate_CheckedChanged);
            // 
            // grpCertificate
            // 
            this.grpCertificate.Controls.Add(this.btnCertManager);
            this.grpCertificate.Controls.Add(this.txtCertPassword);
            this.grpCertificate.Controls.Add(this.lblCertPassword);
            this.grpCertificate.Controls.Add(this.btnBrowseCert);
            this.grpCertificate.Controls.Add(this.txtCertificatePath);
            this.grpCertificate.Controls.Add(this.lblCertPath);
            this.grpCertificate.Enabled = false;
            this.grpCertificate.Location = new System.Drawing.Point(10, 260);
            this.grpCertificate.Name = "grpCertificate";
            this.grpCertificate.Size = new System.Drawing.Size(560, 80);
            this.grpCertificate.TabIndex = 8;
            this.grpCertificate.TabStop = false;
            this.grpCertificate.Text = "Certificate Settings";
            // 
            // btnCertManager
            // 
            this.btnCertManager.Location = new System.Drawing.Point(450, 45);
            this.btnCertManager.Name = "btnCertManager";
            this.btnCertManager.Size = new System.Drawing.Size(100, 23);
            this.btnCertManager.TabIndex = 5;
            this.btnCertManager.Text = "Cert Manager";
            this.btnCertManager.UseVisualStyleBackColor = true;
            this.btnCertManager.Click += new System.EventHandler(this.btnCertManager_Click);
            // 
            // txtCertPassword
            // 
            this.txtCertPassword.Location = new System.Drawing.Point(100, 45);
            this.txtCertPassword.Name = "txtCertPassword";
            this.txtCertPassword.PasswordChar = '*';
            this.txtCertPassword.Size = new System.Drawing.Size(340, 20);
            this.txtCertPassword.TabIndex = 4;
            // 
            // lblCertPassword
            // 
            this.lblCertPassword.AutoSize = true;
            this.lblCertPassword.Location = new System.Drawing.Point(10, 48);
            this.lblCertPassword.Name = "lblCertPassword";
            this.lblCertPassword.Size = new System.Drawing.Size(56, 13);
            this.lblCertPassword.TabIndex = 3;
            this.lblCertPassword.Text = "Password:";
            // 
            // btnBrowseCert
            // 
            this.btnBrowseCert.Location = new System.Drawing.Point(450, 18);
            this.btnBrowseCert.Name = "btnBrowseCert";
            this.btnBrowseCert.Size = new System.Drawing.Size(100, 23);
            this.btnBrowseCert.TabIndex = 2;
            this.btnBrowseCert.Text = "Browse...";
            this.btnBrowseCert.UseVisualStyleBackColor = true;
            this.btnBrowseCert.Click += new System.EventHandler(this.btnBrowseCert_Click);
            // 
            // txtCertificatePath
            // 
            this.txtCertificatePath.Location = new System.Drawing.Point(100, 20);
            this.txtCertificatePath.Name = "txtCertificatePath";
            this.txtCertificatePath.Size = new System.Drawing.Size(340, 20);
            this.txtCertificatePath.TabIndex = 1;
            // 
            // lblCertPath
            // 
            this.lblCertPath.AutoSize = true;
            this.lblCertPath.Location = new System.Drawing.Point(10, 23);
            this.lblCertPath.Name = "lblCertPath";
            this.lblCertPath.Size = new System.Drawing.Size(82, 13);
            this.lblCertPath.TabIndex = 0;
            this.lblCertPath.Text = "Certificate Path:";
            // 
            // chkUseAuth
            // 
            this.chkUseAuth.AutoSize = true;
            this.chkUseAuth.Location = new System.Drawing.Point(10, 130);
            this.chkUseAuth.Name = "chkUseAuth";
            this.chkUseAuth.Size = new System.Drawing.Size(117, 17);
            this.chkUseAuth.TabIndex = 7;
            this.chkUseAuth.Text = "Use Authentication";
            this.chkUseAuth.UseVisualStyleBackColor = true;
            this.chkUseAuth.CheckedChanged += new System.EventHandler(this.chkUseAuth_CheckedChanged);
            // 
            // grpAuthentication
            // 
            this.grpAuthentication.Controls.Add(this.txtPassword);
            this.grpAuthentication.Controls.Add(this.lblPassword);
            this.grpAuthentication.Controls.Add(this.txtUsername);
            this.grpAuthentication.Controls.Add(this.lblUsername);
            this.grpAuthentication.Enabled = false;
            this.grpAuthentication.Location = new System.Drawing.Point(10, 150);
            this.grpAuthentication.Name = "grpAuthentication";
            this.grpAuthentication.Size = new System.Drawing.Size(560, 80);
            this.grpAuthentication.TabIndex = 6;
            this.grpAuthentication.TabStop = false;
            this.grpAuthentication.Text = "Authentication";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(100, 45);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(450, 20);
            this.txtPassword.TabIndex = 3;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(10, 48);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(56, 13);
            this.lblPassword.TabIndex = 2;
            this.lblPassword.Text = "Password:";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(100, 20);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(450, 20);
            this.txtUsername.TabIndex = 1;
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(10, 23);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(58, 13);
            this.lblUsername.TabIndex = 0;
            this.lblUsername.Text = "Username:";
            // 
            // cmbSecurityPolicy
            // 
            this.cmbSecurityPolicy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSecurityPolicy.Enabled = false;
            this.cmbSecurityPolicy.FormattingEnabled = true;
            this.cmbSecurityPolicy.Location = new System.Drawing.Point(110, 95);
            this.cmbSecurityPolicy.Name = "cmbSecurityPolicy";
            this.cmbSecurityPolicy.Size = new System.Drawing.Size(460, 21);
            this.cmbSecurityPolicy.TabIndex = 5;
            // 
            // lblSecurityPolicy
            // 
            this.lblSecurityPolicy.AutoSize = true;
            this.lblSecurityPolicy.Enabled = false;
            this.lblSecurityPolicy.Location = new System.Drawing.Point(10, 98);
            this.lblSecurityPolicy.Name = "lblSecurityPolicy";
            this.lblSecurityPolicy.Size = new System.Drawing.Size(78, 13);
            this.lblSecurityPolicy.TabIndex = 4;
            this.lblSecurityPolicy.Text = "Security Policy:";
            // 
            // cmbSecurityMode
            // 
            this.cmbSecurityMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSecurityMode.FormattingEnabled = true;
            this.cmbSecurityMode.Items.AddRange(new object[] {
            "None",
            "Sign",
            "SignAndEncrypt"});
            this.cmbSecurityMode.Location = new System.Drawing.Point(110, 60);
            this.cmbSecurityMode.Name = "cmbSecurityMode";
            this.cmbSecurityMode.Size = new System.Drawing.Size(460, 21);
            this.cmbSecurityMode.TabIndex = 3;
            this.cmbSecurityMode.SelectedIndexChanged += new System.EventHandler(this.cmbSecurityMode_SelectedIndexChanged);
            // 
            // lblSecurityMode
            // 
            this.lblSecurityMode.AutoSize = true;
            this.lblSecurityMode.Location = new System.Drawing.Point(10, 63);
            this.lblSecurityMode.Name = "lblSecurityMode";
            this.lblSecurityMode.Size = new System.Drawing.Size(79, 13);
            this.lblSecurityMode.TabIndex = 2;
            this.lblSecurityMode.Text = "Security Mode:";
            // 
            // txtServerUrl
            // 
            this.txtServerUrl.Location = new System.Drawing.Point(110, 25);
            this.txtServerUrl.Name = "txtServerUrl";
            this.txtServerUrl.Size = new System.Drawing.Size(460, 20);
            this.txtServerUrl.TabIndex = 1;
            // 
            // lblServerUrl
            // 
            this.lblServerUrl.AutoSize = true;
            this.lblServerUrl.Location = new System.Drawing.Point(10, 28);
            this.lblServerUrl.Name = "lblServerUrl";
            this.lblServerUrl.Size = new System.Drawing.Size(66, 13);
            this.lblServerUrl.TabIndex = 0;
            this.lblServerUrl.Text = "Server URL:";
            // 
            // grpNodeOperations
            // 
            this.grpNodeOperations.Controls.Add(this.btnSubscribe);
            this.grpNodeOperations.Controls.Add(this.btnWrite);
            this.grpNodeOperations.Controls.Add(this.btnRead);
            this.grpNodeOperations.Controls.Add(this.txtValue);
            this.grpNodeOperations.Controls.Add(this.lblValue);
            this.grpNodeOperations.Controls.Add(this.btnBrowse);
            this.grpNodeOperations.Controls.Add(this.txtNodeId);
            this.grpNodeOperations.Controls.Add(this.lblNodeId);
            this.grpNodeOperations.Enabled = false;
            this.grpNodeOperations.Location = new System.Drawing.Point(598, 12);
            this.grpNodeOperations.Name = "grpNodeOperations";
            this.grpNodeOperations.Size = new System.Drawing.Size(574, 180);
            this.grpNodeOperations.TabIndex = 1;
            this.grpNodeOperations.TabStop = false;
            this.grpNodeOperations.Text = "Node Operations";
            // 
            // btnSubscribe
            // 
            this.btnSubscribe.Location = new System.Drawing.Point(300, 140);
            this.btnSubscribe.Name = "btnSubscribe";
            this.btnSubscribe.Size = new System.Drawing.Size(90, 25);
            this.btnSubscribe.TabIndex = 7;
            this.btnSubscribe.Text = "Subscribe";
            this.btnSubscribe.UseVisualStyleBackColor = true;
            this.btnSubscribe.Click += new System.EventHandler(this.btnSubscribe_Click);
            // 
            // btnWrite
            // 
            this.btnWrite.Location = new System.Drawing.Point(200, 140);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(90, 25);
            this.btnWrite.TabIndex = 6;
            this.btnWrite.Text = "Write";
            this.btnWrite.UseVisualStyleBackColor = true;
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            // 
            // btnRead
            // 
            this.btnRead.Location = new System.Drawing.Point(100, 140);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(90, 25);
            this.btnRead.TabIndex = 5;
            this.btnRead.Text = "Read";
            this.btnRead.UseVisualStyleBackColor = true;
            this.btnRead.Click += new System.EventHandler(this.btnRead_Click);
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(100, 80);
            this.txtValue.Multiline = true;
            this.txtValue.Name = "txtValue";
            this.txtValue.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtValue.Size = new System.Drawing.Size(464, 50);
            this.txtValue.TabIndex = 4;
            // 
            // lblValue
            // 
            this.lblValue.AutoSize = true;
            this.lblValue.Location = new System.Drawing.Point(10, 83);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(37, 13);
            this.lblValue.TabIndex = 3;
            this.lblValue.Text = "Value:";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(474, 28);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(90, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtNodeId
            // 
            this.txtNodeId.Location = new System.Drawing.Point(100, 30);
            this.txtNodeId.Name = "txtNodeId";
            this.txtNodeId.Size = new System.Drawing.Size(364, 20);
            this.txtNodeId.TabIndex = 1;
            // 
            // lblNodeId
            // 
            this.lblNodeId.AutoSize = true;
            this.lblNodeId.Location = new System.Drawing.Point(10, 33);
            this.lblNodeId.Name = "lblNodeId";
            this.lblNodeId.Size = new System.Drawing.Size(50, 13);
            this.lblNodeId.TabIndex = 0;
            this.lblNodeId.Text = "Node ID:";
            // 
            // grpLogs
            // 
            this.grpLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLogs.Controls.Add(this.lstLogs);
            this.grpLogs.Location = new System.Drawing.Point(12, 398);
            this.grpLogs.Name = "grpLogs";
            this.grpLogs.Size = new System.Drawing.Size(1160, 350);
            this.grpLogs.TabIndex = 2;
            this.grpLogs.TabStop = false;
            this.grpLogs.Text = "Application Logs";
            // 
            // lstLogs
            // 
            this.lstLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstLogs.Font = new System.Drawing.Font("Consolas", 9F);
            this.lstLogs.FormattingEnabled = true;
            this.lstLogs.HorizontalScrollbar = true;
            this.lstLogs.ItemHeight = 14;
            this.lstLogs.Location = new System.Drawing.Point(10, 20);
            this.lstLogs.Name = "lstLogs";
            this.lstLogs.Size = new System.Drawing.Size(1140, 312);
            this.lstLogs.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 761);
            this.Controls.Add(this.grpLogs);
            this.Controls.Add(this.grpNodeOperations);
            this.Controls.Add(this.grpConnection);
            this.Name = "MainForm";
            this.Text = "XWopcUA - OPC UA Client";
            this.grpConnection.ResumeLayout(false);
            this.grpConnection.PerformLayout();
            this.grpCertificate.ResumeLayout(false);
            this.grpCertificate.PerformLayout();
            this.grpAuthentication.ResumeLayout(false);
            this.grpAuthentication.PerformLayout();
            this.grpNodeOperations.ResumeLayout(false);
            this.grpNodeOperations.PerformLayout();
            this.grpLogs.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}