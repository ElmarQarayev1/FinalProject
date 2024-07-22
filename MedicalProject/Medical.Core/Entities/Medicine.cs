using System;
namespace Medical.Core.Entities
{
	public class Medicine:AuditEntity
	{
		public string Name { get; set; }

		public double Price { get; set; }

		public string Desc { get; set; }

		public int CategoryId { get; set; }

		public Category? Category { get; set; }

		public List<MedicineImage> MedicineImages { get; set; }

		public List<MedicineReview>? MedicineReviews { get; set; }
	}
}

