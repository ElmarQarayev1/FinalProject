using System;
namespace Medical.Service.Dtos.Admin.DoctorDtos
{
	public class DoctorPaginatedGetDto
	{
        public int Id { get; set; }

        public string FullName { get; set; }

        public string DepartmentName { get; set; }

        public string Phone { get; set; }

        public string FileUrl { get; set; }
    }
}

