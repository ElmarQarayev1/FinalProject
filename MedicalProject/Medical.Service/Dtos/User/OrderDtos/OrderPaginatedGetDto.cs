using System;
namespace Medical.Service.Dtos.User.OrderDtos
{
	public class OrderPaginatedGetDto
	{
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string Phone { get; set; }
        public DateTime? CreatedAt { get; set; }
        public double TotalPrice { get; set; }
        public string Status { get; set; }


    }
}

