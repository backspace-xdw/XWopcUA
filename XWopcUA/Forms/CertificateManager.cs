using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using XWopcUA.Models;
using XWopcUA.Services;
using XWopcUA.Utils;

namespace XWopcUA.Forms
{
    public partial class CertificateManager : Form
    {
        private readonly CertificateService _certService;
        private readonly ConnectionSettings _settings;
        private readonly Logger _logger;

        public CertificateManager(ConnectionSettings settings)
        {
            InitializeComponent();
            _settings = settings;
            _certService = new CertificateService();
            _logger = Logger.Instance;
            LoadCertificates();
        }

        private void LoadCertificates()
        {
            try
            {
                lstTrusted.Items.Clear();
                lstRejected.Items.Clear();

                // Load trusted certificates
                var trustedCerts = _certService.LoadCertificatesFromStore(_settings.TrustedCertificatesPath);
                foreach (var cert in trustedCerts)
                {
                    var item = new ListViewItem(new[]
                    {
                        cert.GetNameInfo(X509NameType.SimpleName, false),
                        cert.Thumbprint,
                        cert.NotAfter.ToString("yyyy-MM-dd"),
                        cert.Issuer
                    })
                    {
                        Tag = cert
                    };
                    lstTrusted.Items.Add(item);
                }

                // Load rejected certificates
                var rejectedCerts = _certService.LoadCertificatesFromStore(_settings.RejectedCertificatesPath);
                foreach (var cert in rejectedCerts)
                {
                    var item = new ListViewItem(new[]
                    {
                        cert.GetNameInfo(X509NameType.SimpleName, false),
                        cert.Thumbprint,
                        cert.NotAfter.ToString("yyyy-MM-dd"),
                        cert.Issuer
                    })
                    {
                        Tag = cert
                    };
                    lstRejected.Items.Add(item);
                }

                UpdateStatus();
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to load certificates", ex);
                MessageBox.Show($"Failed to load certificates: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Certificate files (*.der;*.cer;*.crt;*.pem;*.pfx)|*.der;*.cer;*.crt;*.pem;*.pfx|All files (*.*)|*.*";
                dlg.Title = "Import Certificate";
                dlg.Multiselect = true;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (string fileName in dlg.FileNames)
                    {
                        try
                        {
                            X509Certificate2 cert = null;
                            string password = null;

                            if (fileName.EndsWith(".pfx", StringComparison.OrdinalIgnoreCase))
                            {
                                using (var passwordForm = new PasswordInputForm())
                                {
                                    if (passwordForm.ShowDialog() == DialogResult.OK)
                                    {
                                        password = passwordForm.Password;
                                    }
                                }
                            }

                            cert = _certService.LoadCertificate(fileName, password);
                            
                            // Validate certificate
                            string validationMessage;
                            bool isValid = _certService.ValidateCertificate(cert, out validationMessage);

                            var result = MessageBox.Show(
                                $"Certificate: {cert.Subject}\n" +
                                $"Thumbprint: {cert.Thumbprint}\n" +
                                $"Valid From: {cert.NotBefore}\n" +
                                $"Valid To: {cert.NotAfter}\n" +
                                $"Validation: {validationMessage}\n\n" +
                                "Do you want to trust this certificate?",
                                "Import Certificate",
                                MessageBoxButtons.YesNoCancel,
                                MessageBoxIcon.Question);

                            if (result == DialogResult.Yes)
                            {
                                _certService.ImportCertificate(cert, _settings.TrustedCertificatesPath);
                                _logger.Info($"Certificate imported as trusted: {cert.Subject}");
                            }
                            else if (result == DialogResult.No)
                            {
                                _certService.ImportCertificate(cert, _settings.RejectedCertificatesPath);
                                _logger.Info($"Certificate imported as rejected: {cert.Subject}");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error($"Failed to import certificate: {fileName}", ex);
                            MessageBox.Show($"Failed to import {fileName}: {ex.Message}", "Import Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    LoadCertificates();
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            ListView activeList = tabControl.SelectedTab == tabTrusted ? lstTrusted : lstRejected;
            
            if (activeList.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a certificate to remove", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                "Are you sure you want to remove the selected certificate(s)?",
                "Confirm Remove",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                string storePath = activeList == lstTrusted ? 
                    _settings.TrustedCertificatesPath : _settings.RejectedCertificatesPath;

                foreach (ListViewItem item in activeList.SelectedItems)
                {
                    try
                    {
                        var cert = (X509Certificate2)item.Tag;
                        _certService.RemoveCertificate(cert, storePath);
                        _logger.Info($"Certificate removed: {cert.Subject}");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Failed to remove certificate", ex);
                        MessageBox.Show($"Failed to remove certificate: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                LoadCertificates();
            }
        }

        private void btnMoveTo_Click(object sender, EventArgs e)
        {
            ListView activeList = tabControl.SelectedTab == tabTrusted ? lstTrusted : lstRejected;
            
            if (activeList.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a certificate to move", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string fromPath = activeList == lstTrusted ? 
                _settings.TrustedCertificatesPath : _settings.RejectedCertificatesPath;
            string toPath = activeList == lstTrusted ? 
                _settings.RejectedCertificatesPath : _settings.TrustedCertificatesPath;
            string action = activeList == lstTrusted ? "reject" : "trust";

            var result = MessageBox.Show(
                $"Are you sure you want to {action} the selected certificate(s)?",
                "Confirm Move",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                foreach (ListViewItem item in activeList.SelectedItems)
                {
                    try
                    {
                        var cert = (X509Certificate2)item.Tag;
                        _certService.RemoveCertificate(cert, fromPath);
                        _certService.ImportCertificate(cert, toPath);
                        _logger.Info($"Certificate moved from {fromPath} to {toPath}: {cert.Subject}");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Failed to move certificate", ex);
                        MessageBox.Show($"Failed to move certificate: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                LoadCertificates();
            }
        }

        private void btnCreateSelfSigned_Click(object sender, EventArgs e)
        {
            using (var createForm = new CreateCertificateForm())
            {
                if (createForm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _certService.CreateSelfSignedCertificate(
                            createForm.ApplicationName,
                            _settings.TrustedCertificatesPath,
                            createForm.KeySize,
                            createForm.LifetimeMonths);

                        MessageBox.Show("Self-signed certificate created successfully", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        LoadCertificates();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Failed to create self-signed certificate", ex);
                        MessageBox.Show($"Failed to create certificate: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnViewDetails_Click(object sender, EventArgs e)
        {
            ListView activeList = tabControl.SelectedTab == tabTrusted ? lstTrusted : lstRejected;
            
            if (activeList.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a certificate to view", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var cert = (X509Certificate2)activeList.SelectedItems[0].Tag;
            X509Certificate2UI.DisplayCertificate(cert);
        }

        private void UpdateStatus()
        {
            lblStatus.Text = $"Trusted: {lstTrusted.Items.Count} | Rejected: {lstRejected.Items.Count}";
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isTrustedTab = tabControl.SelectedTab == tabTrusted;
            btnMoveTo.Text = isTrustedTab ? "Move to Rejected" : "Move to Trusted";
        }
    }

    // Simple password input dialog
    public class PasswordInputForm : Form
    {
        private TextBox txtPassword;
        private Button btnOK;
        private Button btnCancel;

        public string Password => txtPassword.Text;

        public PasswordInputForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Enter Password";
            this.Size = new System.Drawing.Size(350, 150);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var lblPassword = new Label
            {
                Text = "Password:",
                Location = new System.Drawing.Point(12, 20),
                Size = new System.Drawing.Size(60, 23)
            };

            txtPassword = new TextBox
            {
                Location = new System.Drawing.Point(80, 20),
                Size = new System.Drawing.Size(240, 20),
                PasswordChar = '*'
            };

            btnOK = new Button
            {
                Text = "OK",
                Location = new System.Drawing.Point(165, 60),
                Size = new System.Drawing.Size(75, 23),
                DialogResult = DialogResult.OK
            };

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new System.Drawing.Point(245, 60),
                Size = new System.Drawing.Size(75, 23),
                DialogResult = DialogResult.Cancel
            };

            this.Controls.AddRange(new Control[] { lblPassword, txtPassword, btnOK, btnCancel });
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }
    }

    // Create certificate dialog
    public class CreateCertificateForm : Form
    {
        private TextBox txtApplicationName;
        private NumericUpDown numKeySize;
        private NumericUpDown numLifetime;
        private Button btnOK;
        private Button btnCancel;

        public string ApplicationName => txtApplicationName.Text;
        public int KeySize => (int)numKeySize.Value;
        public int LifetimeMonths => (int)numLifetime.Value;

        public CreateCertificateForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Create Self-Signed Certificate";
            this.Size = new System.Drawing.Size(400, 220);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var lblAppName = new Label
            {
                Text = "Application Name:",
                Location = new System.Drawing.Point(12, 20),
                Size = new System.Drawing.Size(120, 23)
            };

            txtApplicationName = new TextBox
            {
                Location = new System.Drawing.Point(140, 20),
                Size = new System.Drawing.Size(230, 20),
                Text = "XWopcUA Client"
            };

            var lblKeySize = new Label
            {
                Text = "Key Size (bits):",
                Location = new System.Drawing.Point(12, 60),
                Size = new System.Drawing.Size(120, 23)
            };

            numKeySize = new NumericUpDown
            {
                Location = new System.Drawing.Point(140, 60),
                Size = new System.Drawing.Size(100, 20),
                Minimum = 1024,
                Maximum = 4096,
                Increment = 1024,
                Value = 2048
            };

            var lblLifetime = new Label
            {
                Text = "Lifetime (months):",
                Location = new System.Drawing.Point(12, 100),
                Size = new System.Drawing.Size(120, 23)
            };

            numLifetime = new NumericUpDown
            {
                Location = new System.Drawing.Point(140, 100),
                Size = new System.Drawing.Size(100, 20),
                Minimum = 1,
                Maximum = 120,
                Value = 12
            };

            btnOK = new Button
            {
                Text = "Create",
                Location = new System.Drawing.Point(210, 140),
                Size = new System.Drawing.Size(75, 23),
                DialogResult = DialogResult.OK
            };

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new System.Drawing.Point(295, 140),
                Size = new System.Drawing.Size(75, 23),
                DialogResult = DialogResult.Cancel
            };

            this.Controls.AddRange(new Control[] 
            { 
                lblAppName, txtApplicationName, 
                lblKeySize, numKeySize, 
                lblLifetime, numLifetime, 
                btnOK, btnCancel 
            });
            
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }
    }
}