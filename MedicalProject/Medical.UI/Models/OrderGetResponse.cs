using System;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Medical.UI.Models
{
	public class OrderGetResponse
	{
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string Phone { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedAtFormatted => CreatedAt?.ToString("MM/dd/yyyy HH:mm"); 
        public List<OrderItemResponse> OrderItems { get; set; }
        public double TotalPrice { get; set; }
    }
}

