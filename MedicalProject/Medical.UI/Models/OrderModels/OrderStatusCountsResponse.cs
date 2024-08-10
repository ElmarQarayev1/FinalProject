using System;
using System.Text.Json.Serialization;
namespace Medical.UI.Models
{
	public class OrderStatusCountsResponse
	{
        [JsonPropertyName("acceptedCount")]
        public int AcceptedCount { get; set; }
        [JsonPropertyName("rejectedCount")]
        public int RejectedCount { get; set; }
        [JsonPropertyName("pendingCount")]
        public int PendingCount { get; set; }
    }
}

