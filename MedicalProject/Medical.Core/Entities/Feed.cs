using System;
namespace Medical.Core.Entities
{
	public class Feed:BaseEntity
	{

		public string Name { get; set; }

		public string Desc { get; set; }

        public string ImageName { get; set; }

        public DateTime Date { get; set; }
	}
}

