using System;
namespace Medical.Core.Entities
{
	public class Category:BaseEntity
	{
		public string Name { get; set; }

		public List<Medicine> Medicines { get; set; }
	}
}

