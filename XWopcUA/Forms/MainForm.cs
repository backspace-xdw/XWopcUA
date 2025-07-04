using System;
using System.Drawing;
using System.Windows.Forms;
using XWopcUA.Models;
using XWopcUA.Services;
using XWopcUA.Utils;
using Opc.Ua;

namespace XWopcUA.Forms
{
    public partial class MainForm : Form
    {
        private OpcUaClient _opcClient;
        private ConnectionSettings _settings;
        private Logger _logger;

        public MainForm()
        {
            InitializeComponent();
            _logger = Logger.Instance;
            _logger.LogMessageReceived += OnLogMessageReceived;
            _settings = new ConnectionSettings();
            _opcClient = new OpcUaClient();
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "XWopcUA - OPC UA Client";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            LoadConnectionSettings();
            UpdateConnectionStatus(false);
            _logger.Info("XWopcUA Client started");
        }

        private void LoadConnectionSettings()
        {
            txtServerUrl.Text = _settings.ServerUrl;
            cmbSecurityMode.SelectedIndex = 0;
            chkUseAuth.Checked = false;
            chkUseCertificate.Checked = false;
        }

        private void UpdateConnectionStatus(bool connected)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(UpdateConnectionStatus), connected);
                return;
            }

            lblConnectionStatus.Text = connected ? "Connected" : "Disconnected";
            lblConnectionStatus.ForeColor = connected ? Color.Green : Color.Red;
            btnConnect.Text = connected ? "Disconnect" : "Connect";
            btnConnect.Enabled = true;

            grpNodeOperations.Enabled = connected;
            btnBrowse.Enabled = connected;
            btnRead.Enabled = connected;
            btnWrite.Enabled = connected;
            btnSubscribe.Enabled = connected;
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                btnConnect.Enabled = false;
                
                if (_opcClient.IsConnected)
                {
                    await _opcClient.DisconnectAsync();
                    UpdateConnectionStatus(false);
                    _logger.Info("Disconnected from OPC UA server");
                }
                else
                {
                    UpdateSettingsFromUI();
                    await _opcClient.ConnectAsync(_settings);
                    UpdateConnectionStatus(true);
                    _logger.Info($"Connected to OPC UA server: {_settings.ServerUrl}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Connection error: {ex.Message}", ex);
                MessageBox.Show($"Connection error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateConnectionStatus(false);
            }
        }

        private void UpdateSettingsFromUI()
        {
            _settings.ServerUrl = txtServerUrl.Text;
            _settings.SecurityMode = (MessageSecurityMode)cmbSecurityMode.SelectedIndex;
            
            if (_settings.SecurityMode != MessageSecurityMode.None)
            {
                _settings.SecurityPolicy = cmbSecurityPolicy.SelectedItem?.ToString() ?? SecurityPolicies.Basic256Sha256;
            }

            _settings.UseAuthentication = chkUseAuth.Checked;
            if (_settings.UseAuthentication)
            {
                _settings.Username = txtUsername.Text;
                _settings.Password = txtPassword.Text;
            }

            _settings.UseCertificate = chkUseCertificate.Checked;
            if (_settings.UseCertificate)
            {
                _settings.ClientCertificatePath = txtCertificatePath.Text;
                _settings.ClientCertificatePassword = txtCertPassword.Text;
            }
        }

        private void btnBrowseCert_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Certificate files (*.der;*.pem;*.pfx)|*.der;*.pem;*.pfx|All files (*.*)|*.*";
                dlg.Title = "Select Client Certificate";
                
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtCertificatePath.Text = dlg.FileName;
                }
            }
        }

        private void btnCertManager_Click(object sender, EventArgs e)
        {
            using (CertificateManager certForm = new CertificateManager(_settings))
            {
                certForm.ShowDialog(this);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (!_opcClient.IsConnected)
            {
                MessageBox.Show("Please connect to server first", "Not Connected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (NodeBrowser browser = new NodeBrowser(_opcClient))
            {
                if (browser.ShowDialog(this) == DialogResult.OK && browser.SelectedNode != null)
                {
                    txtNodeId.Text = browser.SelectedNode.NodeId.ToString();
                }
            }
        }

        private async void btnRead_Click(object sender, EventArgs e)
        {
            if (!_opcClient.IsConnected)
            {
                MessageBox.Show("Please connect to server first", "Not Connected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string nodeIdStr = txtNodeId.Text.Trim();
                if (string.IsNullOrEmpty(nodeIdStr))
                {
                    MessageBox.Show("Please enter a Node ID", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                NodeId nodeId = NodeId.Parse(nodeIdStr);
                var value = await _opcClient.ReadNodeAsync(nodeId);
                
                txtValue.Text = value?.ToString() ?? "null";
                _logger.Info($"Read value from {nodeIdStr}: {value}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Read error: {ex.Message}", ex);
                MessageBox.Show($"Read error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnWrite_Click(object sender, EventArgs e)
        {
            if (!_opcClient.IsConnected)
            {
                MessageBox.Show("Please connect to server first", "Not Connected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string nodeIdStr = txtNodeId.Text.Trim();
                string valueStr = txtValue.Text.Trim();
                
                if (string.IsNullOrEmpty(nodeIdStr) || string.IsNullOrEmpty(valueStr))
                {
                    MessageBox.Show("Please enter both Node ID and Value", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                NodeId nodeId = NodeId.Parse(nodeIdStr);
                await _opcClient.WriteNodeAsync(nodeId, valueStr);
                
                _logger.Info($"Written value to {nodeIdStr}: {valueStr}");
                MessageBox.Show("Value written successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _logger.Error($"Write error: {ex.Message}", ex);
                MessageBox.Show($"Write error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSubscribe_Click(object sender, EventArgs e)
        {
            if (!_opcClient.IsConnected)
            {
                MessageBox.Show("Please connect to server first", "Not Connected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Subscription feature will be implemented", "Coming Soon", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void chkUseAuth_CheckedChanged(object sender, EventArgs e)
        {
            grpAuthentication.Enabled = chkUseAuth.Checked;
        }

        private void chkUseCertificate_CheckedChanged(object sender, EventArgs e)
        {
            grpCertificate.Enabled = chkUseCertificate.Checked;
        }

        private void cmbSecurityMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool securityEnabled = cmbSecurityMode.SelectedIndex > 0;
            lblSecurityPolicy.Enabled = securityEnabled;
            cmbSecurityPolicy.Enabled = securityEnabled;
            
            if (securityEnabled && cmbSecurityPolicy.Items.Count == 0)
            {
                cmbSecurityPolicy.Items.AddRange(new object[] 
                {
                    SecurityPolicies.Basic128Rsa15,
                    SecurityPolicies.Basic256,
                    SecurityPolicies.Basic256Sha256,
                    SecurityPolicies.Aes128_Sha256_RsaOaep,
                    SecurityPolicies.Aes256_Sha256_RsaPss
                });
                cmbSecurityPolicy.SelectedIndex = 2;
            }
        }

        private void OnLogMessageReceived(object sender, LogEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler<LogEventArgs>(OnLogMessageReceived), sender, e);
                return;
            }

            string logEntry = $"[{e.Timestamp:HH:mm:ss}] [{e.Level}] {e.Message}";
            lstLogs.Items.Add(logEntry);
            
            if (lstLogs.Items.Count > 1000)
            {
                lstLogs.Items.RemoveAt(0);
            }
            
            lstLogs.SelectedIndex = lstLogs.Items.Count - 1;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_opcClient?.IsConnected == true)
            {
                _opcClient.DisconnectAsync().Wait(5000);
            }
            base.OnFormClosed(e);
        }
    }
}