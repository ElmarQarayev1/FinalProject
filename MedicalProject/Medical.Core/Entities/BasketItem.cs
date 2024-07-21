using System;
namespace Medical.Core.Entities
{
	public class BasketItem:BaseEntity
	{
        public string AppUserId { get; set; }

        public int MedicineId { get; set; }

        public int Count { get; set; }

        public AppUser? AppUser { get; set; }

        public Medicine? Medicine { get; set; }
    }
}

