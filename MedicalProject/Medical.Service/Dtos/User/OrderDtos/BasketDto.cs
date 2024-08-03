using System;
namespace Medical.Service.Dtos.User.OrderDtos
{
	public class BasketDto
	{
        public List<BasketItemDto> Items { get; set; } = new List<BasketItemDto>();

        public double TotalPrice { get; set; }
    }
}

