using System;
namespace Medical.Service.Dtos.User.OrderDtos
{
	public class OrderItemDto
	{
        public int MedicineId { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }
}

