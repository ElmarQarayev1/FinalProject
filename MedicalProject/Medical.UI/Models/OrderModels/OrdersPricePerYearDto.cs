using System;
using System.Text.Json.Serialization;

namespace Medical.UI.Models.OrderModels
{
	public class OrdersPricePerYearDto
	{
        [JsonPropertyName("years")]
        public List<string> Years { get; set; }

        [JsonPropertyName("ordersPrice")]
        public List<double> OrdersPrice { get; set; }
    }
}

