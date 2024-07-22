using System;
namespace Medical.Service.Dtos.Admin.MedicineDtos
{
	public class MedicineDetailsDto
	{
    
        public string Name { get; set; }

        public double Price { get; set; }

        public string Desc { get; set; }

        public int CategoryId { get; set; }

        public List<MedicineImageResponseDto> MedicineImages { get; set; }
    }
}

