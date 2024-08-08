using System;
namespace Medical.Service.Dtos.Admin.OrderDtos
{
	public class OrderStatusCountsDto
	{
        public int AcceptedCount { get; set; }
        public int RejectedCount { get; set; }
        public int PendingCount { get; set; }
    }
}

