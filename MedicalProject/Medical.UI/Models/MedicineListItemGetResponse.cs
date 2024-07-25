using System;
namespace Medical.UI.Models
{
	public class MedicineListItemGetResponse
	{
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public int CategoryId { get; set; }
    }
}


