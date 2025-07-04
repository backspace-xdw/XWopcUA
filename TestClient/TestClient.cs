using System;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

namespace TestClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("XWopcUA Test Client");
            Console.WriteLine("==================\n");

            try
            {
                // Test 1: Connect without encryption
                Console.WriteLine("Test 1: Connecting without encryption...");
                var session = await ConnectToServer("opc.tcp://localhost:4840", false);
                
                if (session != null && session.Connected)
                {
                    Console.WriteLine("✓ Connected successfully!\n");

                    // Test 2: Browse nodes
                    Console.WriteLine("Test 2: Browsing nodes...");
                    BrowseNodes(session);

                    // Test 3: Read values
                    Console.WriteLine("\nTest 3: Reading values...");
                    await ReadValues(session);

                    // Test 4: Write value
                    Console.WriteLine("\nTest 4: Writing value...");
                    await WriteValue(session);

                    // Test 5: Method call
                    Console.WriteLine("\nTest 5: Calling method...");
                    await CallMethod(session);

                    // Test 6: Subscribe to changes
                    Console.WriteLine("\nTest 6: Subscribing to value changes...");
                    await SubscribeToChanges(session);

                    // Disconnect
                    Console.WriteLine("\nDisconnecting...");
                    session.Close();
                    Console.WriteLine("✓ Disconnected successfully!");
                }
                else
                {
                    Console.WriteLine("✗ Connection failed!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static async Task<Session> ConnectToServer(string endpointUrl, bool useSecurity)
        {
            var config = new ApplicationConfiguration()
            {
                ApplicationName = "XWopcUA Test Client",
                ApplicationType = ApplicationType.Client,
                ApplicationUri = "urn:localhost:XWopcUA:TestClient",
                ProductUri = "urn:XWopcUA:TestClient",

                SecurityConfiguration = new SecurityConfiguration
                {
                    ApplicationCertificate = new CertificateIdentifier
                    {
                        StoreType = CertificateStoreType.Directory,
                        StorePath = @".\Certificates\App",
                        SubjectName = "CN=XWopcUA Test Client"
                    },
                    TrustedPeerCertificates = new CertificateTrustList
                    {
                        StoreType = CertificateStoreType.Directory,
                        StorePath = @".\Certificates\Trusted"
                    },
                    TrustedIssuerCertificates = new CertificateTrustList
                    {
                        StoreType = CertificateStoreType.Directory,
                        StorePath = @".\Certificates\Issuers"
                    },
                    RejectedCertificateStore = new CertificateTrustList
                    {
                        StoreType = CertificateStoreType.Directory,
                        StorePath = @".\Certificates\Rejected"
                    },
                    AutoAcceptUntrustedCertificates = true
                },

                TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 }
            };

            await config.Validate(ApplicationType.Client);

            var selectedEndpoint = CoreClientUtils.SelectEndpoint(endpointUrl, useSecurity, 15000);
            var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, EndpointConfiguration.Create(config));
            
            return await Session.Create(
                config,
                endpoint,
                false,
                "XWopcUA Test Session",
                60000,
                new UserIdentity(new AnonymousIdentityToken()),
                null);
        }

        static void BrowseNodes(Session session)
        {
            var nodesToBrowse = new BrowseDescriptionCollection
            {
                new BrowseDescription
                {
                    NodeId = Objects.ObjectsFolder,
                    BrowseDirection = BrowseDirection.Forward,
                    ReferenceTypeId = ReferenceTypes.HierarchicalReferences,
                    IncludeSubtypes = true,
                    NodeClassMask = (uint)NodeClass.Object,
                    ResultMask = (uint)BrowseResultMask.All
                }
            };

            session.Browse(
                null,
                null,
                0,
                nodesToBrowse,
                out BrowseResultCollection results,
                out DiagnosticInfoCollection diagnosticInfos);

            Console.WriteLine("Found nodes under Objects folder:");
            foreach (var reference in results[0].References)
            {
                Console.WriteLine($"  - {reference.DisplayName.Text} [{reference.NodeId}]");
            }
        }

        static async Task ReadValues(Session session)
        {
            var nodesToRead = new ReadValueIdCollection
            {
                new ReadValueId { NodeId = new NodeId("Temperature", 2), AttributeId = Attributes.Value },
                new ReadValueId { NodeId = new NodeId("Pressure", 2), AttributeId = Attributes.Value },
                new ReadValueId { NodeId = new NodeId("Status", 2), AttributeId = Attributes.Value },
                new ReadValueId { NodeId = new NodeId("Counter", 2), AttributeId = Attributes.Value },
                new ReadValueId { NodeId = new NodeId("Message", 2), AttributeId = Attributes.Value }
            };

            session.Read(
                null,
                0,
                TimestampsToReturn.Both,
                nodesToRead,
                out DataValueCollection values,
                out diagnosticInfos);

            Console.WriteLine("Current values:");
            Console.WriteLine($"  Temperature: {values[0].Value} °C");
            Console.WriteLine($"  Pressure: {values[1].Value} kPa");
            Console.WriteLine($"  Status: {values[2].Value}");
            Console.WriteLine($"  Counter: {values[3].Value}");
            Console.WriteLine($"  Message: {values[4].Value}");
        }

        static async Task WriteValue(Session session)
        {
            var nodeToWrite = new WriteValue
            {
                NodeId = new NodeId("Message", 2),
                AttributeId = Attributes.Value,
                Value = new DataValue(new Variant("Updated by XWopcUA Test Client at " + DateTime.Now))
            };

            var nodesToWrite = new WriteValueCollection { nodeToWrite };

            session.Write(
                null,
                nodesToWrite,
                out StatusCodeCollection results,
                out diagnosticInfos);

            if (StatusCode.IsGood(results[0]))
            {
                Console.WriteLine("✓ Successfully wrote new message value");
            }
            else
            {
                Console.WriteLine($"✗ Write failed: {results[0]}");
            }
        }

        static async Task CallMethod(Session session)
        {
            var objectId = new NodeId("TestData", 2);
            var methodId = new NodeId("Multiply", 2);

            var inputArguments = new VariantCollection
            {
                new Variant((double)5.0),
                new Variant((double)3.0)
            };

            var outputArguments = session.Call(
                objectId,
                methodId,
                inputArguments);

            if (outputArguments.Count > 0)
            {
                Console.WriteLine($"✓ Method result: 5.0 × 3.0 = {outputArguments[0]}");
            }
        }

        static async Task SubscribeToChanges(Session session)
        {
            var subscription = new Subscription(session.DefaultSubscription)
            {
                DisplayName = "Test Subscription",
                PublishingEnabled = true,
                PublishingInterval = 1000,
                Priority = 100
            };

            var monitoredItem = new MonitoredItem(subscription.DefaultItem)
            {
                DisplayName = "Temperature Monitor",
                StartNodeId = new NodeId("Temperature", 2),
                AttributeId = Attributes.Value,
                MonitoringMode = MonitoringMode.Reporting,
                SamplingInterval = 1000,
                QueueSize = 10,
                DiscardOldest = true
            };

            int notificationCount = 0;
            monitoredItem.Notification += (sender, e) =>
            {
                foreach (var value in monitoredItem.DequeueValues())
                {
                    Console.WriteLine($"  Temperature changed: {value.Value} °C at {value.SourceTimestamp:HH:mm:ss}");
                    notificationCount++;
                }
            };

            subscription.AddItem(monitoredItem);
            session.AddSubscription(subscription);
            subscription.Create();

            Console.WriteLine("Monitoring temperature for 5 seconds...");
            await Task.Delay(5000);

            Console.WriteLine($"✓ Received {notificationCount} notifications");
            subscription.Delete(true);
        }
    }
}