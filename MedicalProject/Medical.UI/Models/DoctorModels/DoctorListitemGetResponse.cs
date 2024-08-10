using System;
namespace Medical.UI.Models
{
	public class DoctorListitemGetResponse
	{
        public int Id { get; set; }

        public string FullName { get; set; }

        public string DepartmentName { get; set; }

        public int AppointmentCount { get; set; }

        public int TodayAppointmentCount { get; set; }

        public string FileUrl { get; set; }
    }
}

