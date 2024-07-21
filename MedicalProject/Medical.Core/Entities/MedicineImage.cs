using System;
namespace Medical.Core.Entities
{
	public class MedicineImage:BaseEntity
	{
		public int MedicineId { get; set; }

		public Medicine Medicine { get; set; }

		public string ImageName { get; set; }
	}
}

