using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using XWopcUA.Models;
using XWopcUA.Utils;

namespace XWopcUA.Services
{
    public class OpcUaClient
    {
        private ApplicationConfiguration _configuration;
        private Session _session;
        private readonly Logger _logger;
        private ConnectionSettings _settings;

        public bool IsConnected => _session != null && _session.Connected;
        public Session Session => _session;

        public OpcUaClient()
        {
            _logger = Logger.Instance;
        }

        public async Task ConnectAsync(ConnectionSettings settings)
        {
            try
            {
                _settings = settings;
                _logger.Info($"Connecting to {settings.ServerUrl}");

                // Create application configuration
                _configuration = await CreateApplicationConfiguration(settings);

                // Create endpoint description
                var selectedEndpoint = CoreClientUtils.SelectEndpoint(
                    settings.ServerUrl,
                    settings.UseCertificate,
                    settings.SessionTimeout);

                if (settings.SecurityMode != MessageSecurityMode.None)
                {
                    selectedEndpoint = UpdateEndpointSecuritySettings(selectedEndpoint, settings);
                }

                _logger.Info($"Selected endpoint: {selectedEndpoint.EndpointUrl}");

                // Create endpoint configuration
                var endpointConfiguration = EndpointConfiguration.Create(_configuration);
                var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);

                // Create session
                var userIdentity = CreateUserIdentity(settings);
                
                _session = await Session.Create(
                    _configuration,
                    endpoint,
                    false,
                    settings.ApplicationName,
                    (uint)settings.SessionTimeout,
                    userIdentity,
                    null);

                _session.KeepAliveInterval = settings.KeepAliveInterval;
                _session.KeepAlive += Session_KeepAlive;

                _logger.Info("Connected successfully");
            }
            catch (Exception ex)
            {
                _logger.Error("Connection failed", ex);
                throw;
            }
        }

        public async Task DisconnectAsync()
        {
            try
            {
                if (_session != null)
                {
                    _session.KeepAlive -= Session_KeepAlive;
                    _session.Close();
                    _session.Dispose();
                    _session = null;
                    _logger.Info("Disconnected from server");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Disconnect error", ex);
                throw;
            }
        }

        public async Task<object> ReadNodeAsync(NodeId nodeId)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to server");

            try
            {
                var nodesToRead = new ReadValueIdCollection
                {
                    new ReadValueId
                    {
                        NodeId = nodeId,
                        AttributeId = Attributes.Value
                    }
                };

                _session.Read(
                    null,
                    0,
                    TimestampsToReturn.Both,
                    nodesToRead,
                    out DataValueCollection values,
                    out DiagnosticInfoCollection diagnosticInfos);

                if (StatusCode.IsGood(values[0].StatusCode))
                {
                    return values[0].Value;
                }
                else
                {
                    throw new Exception($"Read failed: {values[0].StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Read node error: {nodeId}", ex);
                throw;
            }
        }

        public async Task WriteNodeAsync(NodeId nodeId, object value)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to server");

            try
            {
                var nodesToWrite = new WriteValueCollection
                {
                    new WriteValue
                    {
                        NodeId = nodeId,
                        AttributeId = Attributes.Value,
                        Value = new DataValue(new Variant(value))
                    }
                };

                _session.Write(
                    null,
                    nodesToWrite,
                    out StatusCodeCollection results,
                    out DiagnosticInfoCollection diagnosticInfos);

                if (!StatusCode.IsGood(results[0]))
                {
                    throw new Exception($"Write failed: {results[0]}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Write node error: {nodeId}", ex);
                throw;
            }
        }

        public List<NodeInfo> Browse(NodeId nodeId)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to server");

            var nodes = new List<NodeInfo>();

            try
            {
                var nodeToBrowse = new BrowseDescription
                {
                    NodeId = nodeId,
                    BrowseDirection = BrowseDirection.Forward,
                    ReferenceTypeId = ReferenceTypes.HierarchicalReferences,
                    IncludeSubtypes = true,
                    NodeClassMask = (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
                    ResultMask = (uint)BrowseResultMask.All
                };

                var browseDescriptions = new BrowseDescriptionCollection { nodeToBrowse };

                _session.Browse(
                    null,
                    null,
                    0,
                    browseDescriptions,
                    out BrowseResultCollection results,
                    out DiagnosticInfoCollection diagnosticInfos);

                if (results != null && results.Count > 0 && results[0].References != null)
                {
                    foreach (var reference in results[0].References)
                    {
                        var node = new NodeInfo
                        {
                            NodeId = (NodeId)reference.NodeId,
                            DisplayName = reference.DisplayName.Text,
                            BrowseName = reference.BrowseName.Name,
                            NodeClass = reference.NodeClass,
                            IsVariable = reference.NodeClass == NodeClass.Variable,
                            IsMethod = reference.NodeClass == NodeClass.Method,
                            IsObject = reference.NodeClass == NodeClass.Object
                        };
                        nodes.Add(node);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Browse error for node: {nodeId}", ex);
                throw;
            }

            return nodes;
        }

        private async Task<ApplicationConfiguration> CreateApplicationConfiguration(ConnectionSettings settings)
        {
            var config = new ApplicationConfiguration
            {
                ApplicationName = settings.ApplicationName,
                ApplicationType = ApplicationType.Client,
                ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:{settings.ApplicationName}",
                ProductUri = $"urn:XWopcUA",

                SecurityConfiguration = new SecurityConfiguration
                {
                    ApplicationCertificate = new CertificateIdentifier
                    {
                        StoreType = CertificateStoreType.Directory,
                        StorePath = settings.UseCertificate && !string.IsNullOrEmpty(settings.ClientCertificatePath) 
                            ? System.IO.Path.GetDirectoryName(settings.ClientCertificatePath) 
                            : @".\Certificates\App",
                        SubjectName = settings.ApplicationName
                    },
                    TrustedPeerCertificates = new CertificateTrustList
                    {
                        StoreType = CertificateStoreType.Directory,
                        StorePath = settings.TrustedCertificatesPath
                    },
                    TrustedIssuerCertificates = new CertificateTrustList
                    {
                        StoreType = CertificateStoreType.Directory,
                        StorePath = settings.IssuerCertificatesPath
                    },
                    RejectedCertificateStore = new CertificateTrustList
                    {
                        StoreType = CertificateStoreType.Directory,
                        StorePath = settings.RejectedCertificatesPath
                    },
                    AutoAcceptUntrustedCertificates = false,
                    RejectSHA1SignedCertificates = false
                },

                TransportConfigurations = new TransportConfigurationCollection(),
                TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 }
            };

            if (settings.UseCertificate && !string.IsNullOrEmpty(settings.ClientCertificatePath))
            {
                var certificate = new X509Certificate2(
                    settings.ClientCertificatePath,
                    settings.ClientCertificatePassword,
                    X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
                
                config.SecurityConfiguration.ApplicationCertificate.Certificate = certificate;
            }

            await config.Validate(ApplicationType.Client);
            
            if (config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
            {
                config.CertificateValidator.CertificateValidation += CertificateValidator_CertificateValidation;
            }

            return config;
        }

        private EndpointDescription UpdateEndpointSecuritySettings(EndpointDescription endpoint, ConnectionSettings settings)
        {
            endpoint.SecurityMode = settings.SecurityMode;
            endpoint.SecurityPolicyUri = settings.SecurityPolicy;
            return endpoint;
        }

        private IUserIdentity CreateUserIdentity(ConnectionSettings settings)
        {
            if (settings.UseAuthentication && !string.IsNullOrEmpty(settings.Username))
            {
                return new UserIdentity(settings.Username, settings.Password);
            }
            return new UserIdentity(new AnonymousIdentityToken());
        }

        private void Session_KeepAlive(ISession session, KeepAliveEventArgs e)
        {
            if (e.Status != null && ServiceResult.IsNotGood(e.Status))
            {
                _logger.Warning($"Keep alive status: {e.Status}");
                if (!session.Connected)
                {
                    _logger.Error("Session disconnected");
                }
            }
        }

        private void CertificateValidator_CertificateValidation(CertificateValidator sender, CertificateValidationEventArgs e)
        {
            if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
            {
                e.Accept = true;
                _logger.Warning($"Accepted untrusted certificate: {e.Certificate.Subject}");
            }
        }
    }
}