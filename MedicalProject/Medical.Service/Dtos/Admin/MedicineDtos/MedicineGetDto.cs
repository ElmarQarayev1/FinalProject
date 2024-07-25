using System;
namespace Medical.Service.Dtos.Admin.MedicineDtos
{
	public class MedicineGetDto
	{
        public string Name { get; set; }

        public double Price { get; set; }

        public string Desc { get; set; }

        public int CategoryName { get; set; }
    }
}

