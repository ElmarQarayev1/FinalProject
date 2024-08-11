using System;
using Medical.Core.Enum;

namespace Medical.Service.Dtos.User.MedicineDtos
{
	public class MedicineReviewItemDto
	{
       

        public int? MedicineId { get; set; }

        public string Text { get; set; }

        public byte Rate { get; set; }
    }
}

