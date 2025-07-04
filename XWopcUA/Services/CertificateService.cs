using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Opc.Ua;
using Opc.Ua.Security.Certificates;
using XWopcUA.Utils;

namespace XWopcUA.Services
{
    public class CertificateService
    {
        private readonly Logger _logger;

        public CertificateService()
        {
            _logger = Logger.Instance;
        }

        public X509Certificate2 LoadCertificate(string path, string password = null)
        {
            try
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"Certificate file not found: {path}");
                }

                X509Certificate2 certificate;
                string extension = Path.GetExtension(path).ToLower();

                switch (extension)
                {
                    case ".pfx":
                    case ".p12":
                        certificate = new X509Certificate2(path, password, 
                            X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
                        break;
                    case ".der":
                    case ".cer":
                    case ".crt":
                        certificate = new X509Certificate2(path);
                        break;
                    case ".pem":
                        certificate = LoadPemCertificate(path, password);
                        break;
                    default:
                        throw new NotSupportedException($"Certificate format not supported: {extension}");
                }

                _logger.Info($"Certificate loaded: {certificate.Subject}");
                return certificate;
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to load certificate from {path}", ex);
                throw;
            }
        }

        public void ImportCertificate(X509Certificate2 certificate, string storePath, bool isTrusted = true)
        {
            try
            {
                if (!Directory.Exists(storePath))
                {
                    Directory.CreateDirectory(storePath);
                }

                string fileName = GetCertificateFileName(certificate);
                string filePath = Path.Combine(storePath, fileName);

                byte[] certData = certificate.Export(X509ContentType.Cert);
                File.WriteAllBytes(filePath, certData);

                _logger.Info($"Certificate imported to {filePath}");
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to import certificate", ex);
                throw;
            }
        }

        public List<X509Certificate2> LoadCertificatesFromStore(string storePath)
        {
            var certificates = new List<X509Certificate2>();

            try
            {
                if (!Directory.Exists(storePath))
                {
                    Directory.CreateDirectory(storePath);
                    return certificates;
                }

                string[] certFiles = Directory.GetFiles(storePath, "*.*", SearchOption.TopDirectoryOnly);
                
                foreach (string file in certFiles)
                {
                    try
                    {
                        string extension = Path.GetExtension(file).ToLower();
                        if (extension == ".der" || extension == ".cer" || extension == ".crt" || extension == ".pem")
                        {
                            var cert = LoadCertificate(file);
                            certificates.Add(cert);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Warning($"Failed to load certificate from {file}: {ex.Message}");
                    }
                }

                _logger.Info($"Loaded {certificates.Count} certificates from {storePath}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to load certificates from store {storePath}", ex);
            }

            return certificates;
        }

        public void RemoveCertificate(X509Certificate2 certificate, string storePath)
        {
            try
            {
                string fileName = GetCertificateFileName(certificate);
                string filePath = Path.Combine(storePath, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.Info($"Certificate removed: {filePath}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to remove certificate", ex);
                throw;
            }
        }

        public bool ValidateCertificate(X509Certificate2 certificate, out string validationMessage)
        {
            var chain = new X509Chain();
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;

            bool isValid = chain.Build(certificate);
            
            if (!isValid)
            {
                var errors = new StringBuilder();
                foreach (X509ChainStatus status in chain.ChainStatus)
                {
                    errors.AppendLine($"{status.Status}: {status.StatusInformation}");
                }
                validationMessage = errors.ToString();
            }
            else
            {
                validationMessage = "Certificate is valid";
            }

            return isValid;
        }

        public void CreateSelfSignedCertificate(string applicationName, string storePath, int keySize = 2048, int lifetimeInMonths = 12)
        {
            try
            {
                if (!Directory.Exists(storePath))
                {
                    Directory.CreateDirectory(storePath);
                }

                var subject = new List<string>
                {
                    $"CN={applicationName}",
                    $"O=XWopcUA",
                    $"DC={System.Net.Dns.GetHostName()}"
                };

                var certificate = CertificateFactory.CreateCertificate(
                    $"urn:{System.Net.Dns.GetHostName()}:{applicationName}",
                    applicationName,
                    subject,
                    new List<string> { Utils.GetHostName(), "localhost", "127.0.0.1" }
                )
                .SetNotBefore(DateTime.UtcNow.AddDays(-1))
                .SetNotAfter(DateTime.UtcNow.AddMonths(lifetimeInMonths))
                .SetKeySize((ushort)keySize)
                .SetHashAlgorithm(X509Utils.GetRSAHashAlgorithmName(SecurityPolicies.Basic256Sha256))
                .CreateForRSA();

                string fileName = $"{applicationName}.der";
                string filePath = Path.Combine(storePath, fileName);

                byte[] certData = certificate.Export(X509ContentType.Cert);
                File.WriteAllBytes(filePath, certData);

                string privateKeyPath = Path.Combine(storePath, $"{applicationName}.pfx");
                byte[] pfxData = certificate.Export(X509ContentType.Pfx, string.Empty);
                File.WriteAllBytes(privateKeyPath, pfxData);

                _logger.Info($"Self-signed certificate created: {filePath}");
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to create self-signed certificate", ex);
                throw;
            }
        }

        private X509Certificate2 LoadPemCertificate(string path, string password)
        {
            string pemContent = File.ReadAllText(path);
            
            // Extract certificate data from PEM format
            const string beginCert = "-----BEGIN CERTIFICATE-----";
            const string endCert = "-----END CERTIFICATE-----";
            
            int startIndex = pemContent.IndexOf(beginCert) + beginCert.Length;
            int endIndex = pemContent.IndexOf(endCert);
            
            if (startIndex < beginCert.Length || endIndex < 0)
            {
                throw new InvalidOperationException("Invalid PEM certificate format");
            }

            string base64 = pemContent.Substring(startIndex, endIndex - startIndex)
                .Replace("\r", "").Replace("\n", "").Replace(" ", "");
            
            byte[] certData = Convert.FromBase64String(base64);
            return new X509Certificate2(certData);
        }

        private string GetCertificateFileName(X509Certificate2 certificate)
        {
            string thumbprint = certificate.Thumbprint;
            string subject = certificate.GetNameInfo(X509NameType.SimpleName, false);
            
            // Clean subject name for file system
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                subject = subject.Replace(c, '_');
            }

            return $"{subject}[{thumbprint}].der";
        }
    }
}