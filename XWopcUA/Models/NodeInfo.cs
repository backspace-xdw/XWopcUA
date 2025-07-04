using Opc.Ua;
using System;

namespace XWopcUA.Models
{
    public class NodeInfo
    {
        public NodeId NodeId { get; set; }
        public string DisplayName { get; set; }
        public string BrowseName { get; set; }
        public NodeClass NodeClass { get; set; }
        public string Description { get; set; }
        public object Value { get; set; }
        public DateTime Timestamp { get; set; }
        public StatusCode StatusCode { get; set; }
        public string DataType { get; set; }
        public int ValueRank { get; set; }
        public bool IsVariable { get; set; }
        public bool IsMethod { get; set; }
        public bool IsObject { get; set; }

        public NodeInfo()
        {
            Timestamp = DateTime.UtcNow;
            StatusCode = StatusCodes.Good;
        }

        public override string ToString()
        {
            return $"{DisplayName} [{NodeId}]";
        }
    }
}