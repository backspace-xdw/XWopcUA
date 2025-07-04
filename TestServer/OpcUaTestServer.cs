using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Server;

namespace TestServer
{
    public class OpcUaTestServer : StandardServer
    {
        private TestNodeManager _nodeManager;

        protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
        {
            Console.WriteLine("Creating node managers.");

            List<INodeManager> nodeManagers = new List<INodeManager>();
            
            // Create custom node manager
            _nodeManager = new TestNodeManager(server, configuration);
            nodeManagers.Add(_nodeManager);

            return new MasterNodeManager(server, configuration, null, nodeManagers.ToArray());
        }

        protected override ServerProperties LoadServerProperties()
        {
            ServerProperties properties = new ServerProperties();
            properties.ProductUri = "urn:XWopcUA:TestServer";
            properties.ProductName = "XWopcUA Test Server";
            properties.ManufacturerName = "XWopcUA";
            properties.SoftwareVersion = Utils.GetAssemblySoftwareVersion();
            properties.BuildNumber = Utils.GetAssemblyBuildNumber();
            properties.BuildDate = Utils.GetAssemblyTimestamp();
            return properties;
        }
    }

    public class TestNodeManager : CustomNodeManager2
    {
        private Timer _simulationTimer;
        private Random _random = new Random();
        private BaseDataVariableState _temperatureVariable;
        private BaseDataVariableState _pressureVariable;
        private BaseDataVariableState _statusVariable;
        private BaseDataVariableState _counterVariable;
        private int _counter = 0;

        public TestNodeManager(IServerInternal server, ApplicationConfiguration configuration)
            : base(server, configuration, "http://opcua.xwopcua.test")
        {
        }

        protected override NodeStateCollection LoadPredefinedNodes(ISystemContext context)
        {
            NodeStateCollection predefinedNodes = new NodeStateCollection();

            // Create a folder for test variables
            FolderState testFolder = new FolderState(null);
            testFolder.NodeId = new NodeId("TestData", NamespaceIndex);
            testFolder.BrowseName = new QualifiedName("TestData", NamespaceIndex);
            testFolder.DisplayName = new LocalizedText("Test Data");
            testFolder.TypeDefinitionId = ObjectTypeIds.FolderType;
            testFolder.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);
            predefinedNodes.Add(testFolder);

            // Temperature variable (analog value)
            _temperatureVariable = new BaseDataVariableState(testFolder);
            _temperatureVariable.NodeId = new NodeId("Temperature", NamespaceIndex);
            _temperatureVariable.BrowseName = new QualifiedName("Temperature", NamespaceIndex);
            _temperatureVariable.DisplayName = new LocalizedText("Temperature");
            _temperatureVariable.TypeDefinitionId = VariableTypeIds.BaseDataVariableType;
            _temperatureVariable.DataType = DataTypeIds.Double;
            _temperatureVariable.ValueRank = ValueRanks.Scalar;
            _temperatureVariable.Value = 20.5;
            _temperatureVariable.StatusCode = StatusCodes.Good;
            _temperatureVariable.Timestamp = DateTime.UtcNow;
            testFolder.AddChild(_temperatureVariable);

            // Pressure variable (analog value)
            _pressureVariable = new BaseDataVariableState(testFolder);
            _pressureVariable.NodeId = new NodeId("Pressure", NamespaceIndex);
            _pressureVariable.BrowseName = new QualifiedName("Pressure", NamespaceIndex);
            _pressureVariable.DisplayName = new LocalizedText("Pressure");
            _pressureVariable.TypeDefinitionId = VariableTypeIds.BaseDataVariableType;
            _pressureVariable.DataType = DataTypeIds.Double;
            _pressureVariable.ValueRank = ValueRanks.Scalar;
            _pressureVariable.Value = 101.325;
            _pressureVariable.StatusCode = StatusCodes.Good;
            _pressureVariable.Timestamp = DateTime.UtcNow;
            testFolder.AddChild(_pressureVariable);

            // Status variable (boolean)
            _statusVariable = new BaseDataVariableState(testFolder);
            _statusVariable.NodeId = new NodeId("Status", NamespaceIndex);
            _statusVariable.BrowseName = new QualifiedName("Status", NamespaceIndex);
            _statusVariable.DisplayName = new LocalizedText("System Status");
            _statusVariable.TypeDefinitionId = VariableTypeIds.BaseDataVariableType;
            _statusVariable.DataType = DataTypeIds.Boolean;
            _statusVariable.ValueRank = ValueRanks.Scalar;
            _statusVariable.Value = true;
            _statusVariable.StatusCode = StatusCodes.Good;
            _statusVariable.Timestamp = DateTime.UtcNow;
            testFolder.AddChild(_statusVariable);

            // Counter variable (integer)
            _counterVariable = new BaseDataVariableState(testFolder);
            _counterVariable.NodeId = new NodeId("Counter", NamespaceIndex);
            _counterVariable.BrowseName = new QualifiedName("Counter", NamespaceIndex);
            _counterVariable.DisplayName = new LocalizedText("Counter");
            _counterVariable.TypeDefinitionId = VariableTypeIds.BaseDataVariableType;
            _counterVariable.DataType = DataTypeIds.Int32;
            _counterVariable.ValueRank = ValueRanks.Scalar;
            _counterVariable.Value = 0;
            _counterVariable.StatusCode = StatusCodes.Good;
            _counterVariable.Timestamp = DateTime.UtcNow;
            testFolder.AddChild(_counterVariable);

            // Add array variable for testing
            BaseDataVariableState arrayVariable = new BaseDataVariableState(testFolder);
            arrayVariable.NodeId = new NodeId("ArrayData", NamespaceIndex);
            arrayVariable.BrowseName = new QualifiedName("ArrayData", NamespaceIndex);
            arrayVariable.DisplayName = new LocalizedText("Array Data");
            arrayVariable.TypeDefinitionId = VariableTypeIds.BaseDataVariableType;
            arrayVariable.DataType = DataTypeIds.Double;
            arrayVariable.ValueRank = ValueRanks.OneDimension;
            arrayVariable.Value = new double[] { 1.1, 2.2, 3.3, 4.4, 5.5 };
            arrayVariable.StatusCode = StatusCodes.Good;
            arrayVariable.Timestamp = DateTime.UtcNow;
            testFolder.AddChild(arrayVariable);

            // Add string variable
            BaseDataVariableState stringVariable = new BaseDataVariableState(testFolder);
            stringVariable.NodeId = new NodeId("Message", NamespaceIndex);
            stringVariable.BrowseName = new QualifiedName("Message", NamespaceIndex);
            stringVariable.DisplayName = new LocalizedText("Message");
            stringVariable.TypeDefinitionId = VariableTypeIds.BaseDataVariableType;
            stringVariable.DataType = DataTypeIds.String;
            stringVariable.ValueRank = ValueRanks.Scalar;
            stringVariable.Value = "Hello from XWopcUA Test Server!";
            stringVariable.StatusCode = StatusCodes.Good;
            stringVariable.Timestamp = DateTime.UtcNow;
            testFolder.AddChild(stringVariable);

            // Add method
            MethodState multiplyMethod = new MethodState(testFolder);
            multiplyMethod.NodeId = new NodeId("Multiply", NamespaceIndex);
            multiplyMethod.BrowseName = new QualifiedName("Multiply", NamespaceIndex);
            multiplyMethod.DisplayName = new LocalizedText("Multiply");
            multiplyMethod.OnCallMethod = new GenericMethodCalledEventHandler(OnMultiply);

            // Set input arguments
            multiplyMethod.InputArguments = new PropertyState<Argument[]>(multiplyMethod);
            multiplyMethod.InputArguments.NodeId = new NodeId("Multiply_InputArguments", NamespaceIndex);
            multiplyMethod.InputArguments.BrowseName = BrowseNames.InputArguments;
            multiplyMethod.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            multiplyMethod.InputArguments.DataType = DataTypeIds.Argument;
            multiplyMethod.InputArguments.ValueRank = ValueRanks.OneDimension;
            multiplyMethod.InputArguments.Value = new Argument[]
            {
                new Argument() { Name = "a", Description = "First number", DataType = DataTypeIds.Double, ValueRank = ValueRanks.Scalar },
                new Argument() { Name = "b", Description = "Second number", DataType = DataTypeIds.Double, ValueRank = ValueRanks.Scalar }
            };

            // Set output arguments
            multiplyMethod.OutputArguments = new PropertyState<Argument[]>(multiplyMethod);
            multiplyMethod.OutputArguments.NodeId = new NodeId("Multiply_OutputArguments", NamespaceIndex);
            multiplyMethod.OutputArguments.BrowseName = BrowseNames.OutputArguments;
            multiplyMethod.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            multiplyMethod.OutputArguments.DataType = DataTypeIds.Argument;
            multiplyMethod.OutputArguments.ValueRank = ValueRanks.OneDimension;
            multiplyMethod.OutputArguments.Value = new Argument[]
            {
                new Argument() { Name = "result", Description = "Result of multiplication", DataType = DataTypeIds.Double, ValueRank = ValueRanks.Scalar }
            };

            testFolder.AddChild(multiplyMethod);

            // Start simulation timer
            _simulationTimer = new Timer(UpdateSimulatedValues, null, 1000, 1000);

            return predefinedNodes;
        }

        private ServiceResult OnMultiply(ISystemContext context, MethodState method, IList<object> inputArguments, IList<object> outputArguments)
        {
            if (inputArguments.Count != 2)
            {
                return StatusCodes.BadArgumentsMissing;
            }

            try
            {
                double a = Convert.ToDouble(inputArguments[0]);
                double b = Convert.ToDouble(inputArguments[1]);
                outputArguments[0] = a * b;
                return ServiceResult.Good;
            }
            catch
            {
                return StatusCodes.BadInvalidArgument;
            }
        }

        private void UpdateSimulatedValues(object state)
        {
            try
            {
                lock (Lock)
                {
                    // Update temperature (20 +/- 5)
                    double temp = 20.0 + (_random.NextDouble() - 0.5) * 10;
                    _temperatureVariable.Value = Math.Round(temp, 2);
                    _temperatureVariable.Timestamp = DateTime.UtcNow;
                    _temperatureVariable.ClearChangeMasks(SystemContext, false);

                    // Update pressure (101.325 +/- 10)
                    double pressure = 101.325 + (_random.NextDouble() - 0.5) * 20;
                    _pressureVariable.Value = Math.Round(pressure, 2);
                    _pressureVariable.Timestamp = DateTime.UtcNow;
                    _pressureVariable.ClearChangeMasks(SystemContext, false);

                    // Toggle status every 10 seconds
                    if (_counter % 10 == 0)
                    {
                        _statusVariable.Value = !(bool)_statusVariable.Value;
                        _statusVariable.Timestamp = DateTime.UtcNow;
                        _statusVariable.ClearChangeMasks(SystemContext, false);
                    }

                    // Update counter
                    _counter++;
                    _counterVariable.Value = _counter;
                    _counterVariable.Timestamp = DateTime.UtcNow;
                    _counterVariable.ClearChangeMasks(SystemContext, false);
                }
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Error updating simulated values");
            }
        }

        public override void Dispose()
        {
            if (_simulationTimer != null)
            {
                _simulationTimer.Dispose();
                _simulationTimer = null;
            }
            base.Dispose();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("XWopcUA Test Server");
            Console.WriteLine("===================");

            try
            {
                // Create and start the server
                TestServerManager serverManager = new TestServerManager();
                serverManager.StartServer();

                Console.WriteLine("\nServer is running at: opc.tcp://localhost:4840");
                Console.WriteLine("\nAvailable endpoints:");
                Console.WriteLine("- No Security: opc.tcp://localhost:4840");
                Console.WriteLine("- Basic256Sha256 - Sign: opc.tcp://localhost:4840");
                Console.WriteLine("- Basic256Sha256 - SignAndEncrypt: opc.tcp://localhost:4840");
                
                Console.WriteLine("\nTest variables:");
                Console.WriteLine("- Temperature (Double): Simulated temperature value");
                Console.WriteLine("- Pressure (Double): Simulated pressure value");
                Console.WriteLine("- Status (Boolean): System status");
                Console.WriteLine("- Counter (Int32): Incrementing counter");
                Console.WriteLine("- ArrayData (Double[]): Array of values");
                Console.WriteLine("- Message (String): Static message");
                Console.WriteLine("- Multiply method: Multiply two numbers");

                Console.WriteLine("\nPress Ctrl+C to stop the server...");

                // Wait for Ctrl+C
                ManualResetEvent quitEvent = new ManualResetEvent(false);
                Console.CancelKeyPress += (sender, eArgs) => {
                    quitEvent.Set();
                    eArgs.Cancel = true;
                };
                quitEvent.WaitOne();

                // Stop the server
                Console.WriteLine("\nStopping server...");
                serverManager.StopServer();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }

    public class TestServerManager
    {
        private OpcUaTestServer _server;

        public async void StartServer()
        {
            ApplicationInstance application = new ApplicationInstance();
            application.ApplicationType = ApplicationType.Server;
            application.ConfigSectionName = "TestServer";

            // Load or create application configuration
            ApplicationConfiguration config = await CreateApplicationConfiguration();
            
            // Check application certificate
            await application.CheckApplicationInstanceCertificate(false, 0).ConfigureAwait(false);

            // Create and start the server
            _server = new OpcUaTestServer();
            await application.Start(_server).ConfigureAwait(false);
        }

        public void StopServer()
        {
            _server?.Stop();
        }

        private async Task<ApplicationConfiguration> CreateApplicationConfiguration()
        {
            ApplicationConfiguration config = new ApplicationConfiguration();
            
            config.ApplicationName = "XWopcUA Test Server";
            config.ApplicationType = ApplicationType.Server;
            config.ApplicationUri = "urn:localhost:XWopcUA:TestServer";
            config.ProductUri = "urn:XWopcUA:TestServer";

            // Security configuration
            config.SecurityConfiguration = new SecurityConfiguration();
            config.SecurityConfiguration.ApplicationCertificate = new CertificateIdentifier()
            {
                StoreType = CertificateStoreType.Directory,
                StorePath = @".\Certificates\App",
                SubjectName = "CN=XWopcUA Test Server, O=XWopcUA, DC=" + Utils.GetHostName()
            };
            config.SecurityConfiguration.TrustedPeerCertificates = new CertificateTrustList()
            {
                StoreType = CertificateStoreType.Directory,
                StorePath = @".\Certificates\Trusted"
            };
            config.SecurityConfiguration.TrustedIssuerCertificates = new CertificateTrustList()
            {
                StoreType = CertificateStoreType.Directory,
                StorePath = @".\Certificates\Issuers"
            };
            config.SecurityConfiguration.RejectedCertificateStore = new CertificateTrustList()
            {
                StoreType = CertificateStoreType.Directory,
                StorePath = @".\Certificates\Rejected"
            };
            config.SecurityConfiguration.AutoAcceptUntrustedCertificates = true;

            // Server configuration
            config.ServerConfiguration = new ServerConfiguration();
            config.ServerConfiguration.BaseAddresses.Add("opc.tcp://localhost:4840");
            config.ServerConfiguration.SecurityPolicies.Add(new ServerSecurityPolicy()
            {
                SecurityMode = MessageSecurityMode.None,
                SecurityPolicyUri = SecurityPolicies.None
            });
            config.ServerConfiguration.SecurityPolicies.Add(new ServerSecurityPolicy()
            {
                SecurityMode = MessageSecurityMode.Sign,
                SecurityPolicyUri = SecurityPolicies.Basic256Sha256
            });
            config.ServerConfiguration.SecurityPolicies.Add(new ServerSecurityPolicy()
            {
                SecurityMode = MessageSecurityMode.SignAndEncrypt,
                SecurityPolicyUri = SecurityPolicies.Basic256Sha256
            });

            config.ServerConfiguration.UserTokenPolicies.Add(new UserTokenPolicy()
            {
                TokenType = UserTokenType.Anonymous,
                SecurityPolicyUri = SecurityPolicies.None
            });
            config.ServerConfiguration.UserTokenPolicies.Add(new UserTokenPolicy()
            {
                TokenType = UserTokenType.UserName,
                SecurityPolicyUri = SecurityPolicies.Basic256Sha256
            });

            // Transport quotas
            config.TransportQuotas = new TransportQuotas();
            config.TransportQuotas.OperationTimeout = 60000;
            config.TransportQuotas.MaxStringLength = 67108864;
            config.TransportQuotas.MaxByteStringLength = 67108864;
            config.TransportQuotas.MaxArrayLength = 65535;
            config.TransportQuotas.MaxMessageSize = 67108864;
            config.TransportQuotas.MaxBufferSize = 65535;
            config.TransportQuotas.ChannelLifetime = 300000;
            config.TransportQuotas.SecurityTokenLifetime = 3600000;

            await config.Validate(ApplicationType.Server).ConfigureAwait(false);

            return config;
        }
    }
}