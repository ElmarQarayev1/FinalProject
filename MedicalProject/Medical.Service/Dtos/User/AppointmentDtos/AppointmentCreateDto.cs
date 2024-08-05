using System;
using Medical.Core.Entities;

namespace Medical.Service.Dtos.User.AppointmentDtos
{
	public class AppointmentCreateDto
	{
        public string? AppUserId { get; set; }

        public int DoctorId { get; set; }

        public string Phone { get; set; }

        public DateTime Date { get; set; }
    }
}

