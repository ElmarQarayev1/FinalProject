using System;
using Medical.Core.Enum;
using System.ComponentModel.DataAnnotations;

namespace Medical.Core.Entities
{
	public class Order:BaseEntity
	{
        public string? AppUserId { get; set; }
      
        public string? FullName { get; set; }
       
        public string? Email { get; set; }
      
        public string Phone { get; set; }
   
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public string Address { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public OrderStatus Status { get; set; }

        public AppUser? AppUser { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
