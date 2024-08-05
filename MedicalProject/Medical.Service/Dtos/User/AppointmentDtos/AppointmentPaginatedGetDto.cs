using System;
namespace Medical.Service.Dtos.User.AppointmentDtos
{
	public class AppointmentPaginatedGetDto
	{
        public int Id { get; set; }

        public string DoctorFullName { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Date { get; set; }

    }
}

