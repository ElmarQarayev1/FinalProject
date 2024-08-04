using System;
namespace Medical.Service.Dtos.User.MedicineDtos
{
	public class MedicineReviewForDetails
	{
        public string AppUserName { get; set; }

        public int? MedicineId { get; set; }

        public string Text { get; set; }

        public string Date { get; set; }

        public byte Rate { get; set; }
    }
}

