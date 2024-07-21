using System;
namespace Medical.Core.Entities
{
	public class Slider:BaseEntity
	{
		public string MainTitle { get; set; }

		public string SubTitle1 { get; set; }

		public string SubTitle2 { get; set; }

		public string ImageName { get; set; }

		public int Order { get; set; }
	}
}

