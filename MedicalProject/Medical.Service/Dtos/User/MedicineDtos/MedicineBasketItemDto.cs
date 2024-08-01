using System;
namespace Medical.Service.Dtos.User.MedicineDtos
{
	public class MedicineBasketItemDto
	{
        public string AppUserId { get; set; }

        public int MedicineId { get; set; }

        public int Count { get; set; }
    }
}

