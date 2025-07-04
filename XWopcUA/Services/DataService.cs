using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;
using XWopcUA.Models;
using XWopcUA.Utils;

namespace XWopcUA.Services
{
    public class DataService
    {
        private readonly Logger _logger;
        private readonly Dictionary<uint, Subscription> _subscriptions;
        private uint _subscriptionIdCounter = 1;

        public event EventHandler<DataChangeEventArgs> DataChanged;

        public DataService()
        {
            _logger = Logger.Instance;
            _subscriptions = new Dictionary<uint, Subscription>();
        }

        public async Task<List<DataValue>> ReadMultipleNodesAsync(Session session, List<NodeId> nodeIds)
        {
            if (session == null || !session.Connected)
                throw new InvalidOperationException("Session not connected");

            try
            {
                var nodesToRead = new ReadValueIdCollection();
                foreach (var nodeId in nodeIds)
                {
                    nodesToRead.Add(new ReadValueId
                    {
                        NodeId = nodeId,
                        AttributeId = Attributes.Value
                    });
                }

                session.Read(
                    null,
                    0,
                    TimestampsToReturn.Both,
                    nodesToRead,
                    out DataValueCollection values,
                    out DiagnosticInfoCollection diagnosticInfos);

                return new List<DataValue>(values);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to read multiple nodes", ex);
                throw;
            }
        }

        public async Task WriteMultipleNodesAsync(Session session, Dictionary<NodeId, object> nodeValues)
        {
            if (session == null || !session.Connected)
                throw new InvalidOperationException("Session not connected");

            try
            {
                var nodesToWrite = new WriteValueCollection();
                foreach (var kvp in nodeValues)
                {
                    nodesToWrite.Add(new WriteValue
                    {
                        NodeId = kvp.Key,
                        AttributeId = Attributes.Value,
                        Value = new DataValue(new Variant(kvp.Value))
                    });
                }

                session.Write(
                    null,
                    nodesToWrite,
                    out StatusCodeCollection results,
                    out DiagnosticInfoCollection diagnosticInfos);

                for (int i = 0; i < results.Count; i++)
                {
                    if (!StatusCode.IsGood(results[i]))
                    {
                        _logger.Error($"Failed to write node {nodesToWrite[i].NodeId}: {results[i]}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to write multiple nodes", ex);
                throw;
            }
        }

        public uint CreateSubscription(Session session, string displayName, int publishingInterval = 1000)
        {
            if (session == null || !session.Connected)
                throw new InvalidOperationException("Session not connected");

            try
            {
                var subscription = new Subscription(session.DefaultSubscription)
                {
                    DisplayName = displayName,
                    PublishingEnabled = true,
                    PublishingInterval = publishingInterval,
                    KeepAliveCount = 10,
                    LifetimeCount = 100,
                    MaxNotificationsPerPublish = 1000,
                    Priority = 100
                };

                session.AddSubscription(subscription);
                subscription.Create();

                uint subscriptionId = _subscriptionIdCounter++;
                _subscriptions[subscriptionId] = subscription;

                _logger.Info($"Created subscription '{displayName}' with ID {subscriptionId}");
                return subscriptionId;
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to create subscription", ex);
                throw;
            }
        }

        public void AddMonitoredItem(uint subscriptionId, NodeId nodeId, string displayName, 
            int samplingInterval = 1000)
        {
            if (!_subscriptions.TryGetValue(subscriptionId, out Subscription subscription))
                throw new ArgumentException($"Subscription {subscriptionId} not found");

            try
            {
                var monitoredItem = new MonitoredItem(subscription.DefaultItem)
                {
                    DisplayName = displayName,
                    StartNodeId = nodeId,
                    AttributeId = Attributes.Value,
                    MonitoringMode = MonitoringMode.Reporting,
                    SamplingInterval = samplingInterval,
                    QueueSize = 10,
                    DiscardOldest = true
                };

                monitoredItem.Notification += OnMonitoredItemNotification;
                subscription.AddItem(monitoredItem);
                subscription.ApplyChanges();

                _logger.Info($"Added monitored item '{displayName}' to subscription {subscriptionId}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to add monitored item to subscription {subscriptionId}", ex);
                throw;
            }
        }

        public void RemoveMonitoredItem(uint subscriptionId, NodeId nodeId)
        {
            if (!_subscriptions.TryGetValue(subscriptionId, out Subscription subscription))
                throw new ArgumentException($"Subscription {subscriptionId} not found");

            try
            {
                var itemToRemove = subscription.MonitoredItems.FirstOrDefault(
                    item => item.StartNodeId.Equals(nodeId));

                if (itemToRemove != null)
                {
                    itemToRemove.Notification -= OnMonitoredItemNotification;
                    subscription.RemoveItem(itemToRemove);
                    subscription.ApplyChanges();
                    _logger.Info($"Removed monitored item {nodeId} from subscription {subscriptionId}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to remove monitored item from subscription {subscriptionId}", ex);
                throw;
            }
        }

        public void DeleteSubscription(uint subscriptionId)
        {
            if (!_subscriptions.TryGetValue(subscriptionId, out Subscription subscription))
                return;

            try
            {
                foreach (var item in subscription.MonitoredItems)
                {
                    item.Notification -= OnMonitoredItemNotification;
                }

                subscription.Delete(true);
                _subscriptions.Remove(subscriptionId);
                _logger.Info($"Deleted subscription {subscriptionId}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to delete subscription {subscriptionId}", ex);
            }
        }

        public void DeleteAllSubscriptions()
        {
            foreach (var subscriptionId in _subscriptions.Keys.ToList())
            {
                DeleteSubscription(subscriptionId);
            }
        }

        private void OnMonitoredItemNotification(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
        {
            try
            {
                foreach (var value in monitoredItem.DequeueValues())
                {
                    var args = new DataChangeEventArgs
                    {
                        NodeId = monitoredItem.StartNodeId,
                        DisplayName = monitoredItem.DisplayName,
                        Value = value.Value,
                        StatusCode = value.StatusCode,
                        SourceTimestamp = value.SourceTimestamp,
                        ServerTimestamp = value.ServerTimestamp
                    };

                    DataChanged?.Invoke(this, args);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error processing monitored item notification", ex);
            }
        }

        public async Task<HistoryReadResultCollection> ReadHistoryAsync(Session session, 
            NodeId nodeId, DateTime startTime, DateTime endTime, uint maxValues = 100)
        {
            if (session == null || !session.Connected)
                throw new InvalidOperationException("Session not connected");

            try
            {
                var historyReadValueId = new HistoryReadValueId
                {
                    NodeId = nodeId
                };

                var historyReadValueIds = new HistoryReadValueIdCollection { historyReadValueId };

                var readRawDetails = new ReadRawModifiedDetails
                {
                    StartTime = startTime,
                    EndTime = endTime,
                    NumValuesPerNode = maxValues,
                    IsReadModified = false,
                    ReturnBounds = true
                };

                session.HistoryRead(
                    null,
                    new ExtensionObject(readRawDetails),
                    TimestampsToReturn.Both,
                    false,
                    historyReadValueIds,
                    out HistoryReadResultCollection results,
                    out DiagnosticInfoCollection diagnosticInfos);

                return results;
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to read history for node {nodeId}", ex);
                throw;
            }
        }
    }

    public class DataChangeEventArgs : EventArgs
    {
        public NodeId NodeId { get; set; }
        public string DisplayName { get; set; }
        public object Value { get; set; }
        public StatusCode StatusCode { get; set; }
        public DateTime SourceTimestamp { get; set; }
        public DateTime ServerTimestamp { get; set; }
    }
}