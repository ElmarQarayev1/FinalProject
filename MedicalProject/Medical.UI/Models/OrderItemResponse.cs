using System;
namespace Medical.UI.Models
{
	public class OrderItemResponse
	{
        public int MedicineId { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }
}

