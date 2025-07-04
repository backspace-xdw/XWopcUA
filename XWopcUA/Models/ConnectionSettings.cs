using Opc.Ua;
using System;

namespace XWopcUA.Models
{
    public class ConnectionSettings
    {
        public string ServerUrl { get; set; }
        public string ApplicationName { get; set; } = "XWopcUA Client";
        public MessageSecurityMode SecurityMode { get; set; } = MessageSecurityMode.None;
        public string SecurityPolicy { get; set; } = SecurityPolicies.None;
        public bool UseAuthentication { get; set; } = false;
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseCertificate { get; set; } = false;
        public string ClientCertificatePath { get; set; }
        public string ClientCertificatePassword { get; set; }
        public string TrustedCertificatesPath { get; set; }
        public string RejectedCertificatesPath { get; set; }
        public string IssuerCertificatesPath { get; set; }
        public int SessionTimeout { get; set; } = 60000;
        public int KeepAliveInterval { get; set; } = 5000;

        public ConnectionSettings()
        {
            ServerUrl = "opc.tcp://localhost:4840";
            TrustedCertificatesPath = @".\Certificates\Trusted";
            RejectedCertificatesPath = @".\Certificates\Rejected";
            IssuerCertificatesPath = @".\Certificates\Issuers";
        }
    }
}