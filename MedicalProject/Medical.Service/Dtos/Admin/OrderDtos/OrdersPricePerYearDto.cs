using System;
namespace Medical.Service.Dtos.Admin.OrderDtos
{
	public class OrdersPricePerYearDto
	{
        public List<string> Years { get; set; }
        public List<double> OrdersPrice { get; set; }
    }
}

