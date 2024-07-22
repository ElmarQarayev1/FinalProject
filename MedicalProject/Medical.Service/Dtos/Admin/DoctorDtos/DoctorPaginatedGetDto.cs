using System;
namespace Medical.Service.Dtos.Admin.DoctorDtos
{
	public class DoctorPaginatedGetDto
	{

        public string Name { get; set; }

        public int DepartmentId { get; set; }

        public string Phone { get; set; }

        public string FileUrl { get; set; }
    }
}

