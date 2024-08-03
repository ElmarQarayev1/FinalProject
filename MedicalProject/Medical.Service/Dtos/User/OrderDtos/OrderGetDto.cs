using System;
namespace Medical.Service.Dtos.User.OrderDtos
{
	public class OrderGetDto
	{
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string Phone { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
        public double TotalPrice { get; set; }
    }
}



