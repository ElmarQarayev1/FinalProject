using System;
namespace Medical.UI.Models
{
	public class DoctorListitemGetResponse
	{
        public int Id { get; set; }

        public string FullName { get; set; }

        public int DepartmentId { get; set; }

        public string Phone { get; set; }

        public string FileUrl { get; set; }
    }
}

