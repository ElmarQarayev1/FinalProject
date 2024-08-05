using System;
namespace Medical.Service.Dtos.User.OrderDtos
{
	public class OrderGetDtoForUserProfile
	{
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public double TotalPrice { get; set; }

        public int TotalItemCount { get; set; }

        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();

        public string Status { get; set; }
    }
}

