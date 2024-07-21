using System;
namespace Medical.Core.Entities
{
	public class Department:BaseEntity
	{
		public string Name { get; set; }

		public string ImageName { get; set; }

		public List<Doctor> Doctors { get; set; }


	}
}

