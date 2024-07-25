using System;
namespace Medical.UI.Models
{
	public class MedicineGetResponse
	{
        public string Name { get; set; }

        public double Price { get; set; }

        public string Desc { get; set; }

        public int CategoryId { get; set; }

        public List<PictureResponse>? MedicineImages { get; set; }
    }
}

