using System;
using Medical.Service.Dtos.Admin.MedicineDtos;

namespace Medical.Service.Dtos.User.MedicineDtos
{
	public class MedicineGetDtoForUser
	{
        public string Name { get; set; }

        public double Price { get; set; }

        public string Desc { get; set; }

        public int TotalReviewsCount { get; set; }

        public int AvgRate { get; set; }

        public List<MedicineImageResponseDto> MedicineImages { get; set; }

        public List<MedicineReviewForDetails> Reviews { get; set; }

    }
}

