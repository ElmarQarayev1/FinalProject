using System;
using Medical.Core.Enum;

namespace Medical.Core.Entities
{
	public class MedicineReview:BaseEntity
	{

		public string AppUserId { get; set; }

		public int MedicineId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ReviewStatus Status { get; set; } = ReviewStatus.Pending;

        public string Text { get; set; }

        public byte Rate { get; set; }

        public AppUser? AppUser { get; set; }

        public Medicine? Medicine { get; set; }


    }
}

