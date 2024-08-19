using System;
namespace Medical.Service.Dtos.User.OrderDtos
{
	public class OrderItemDto
	{
        public string MedicineName { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }
}

