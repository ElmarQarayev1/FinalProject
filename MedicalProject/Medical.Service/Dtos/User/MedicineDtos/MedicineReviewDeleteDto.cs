using System;
namespace Medical.Service.Dtos.User.MedicineDtos
{
	public class MedicineReviewDeleteDto
	{
        public string AppUserId { get; set; }

        public int MedicineId { get; set; }

        public int MedicineReviewId { get; set; }
    }
}

