using System;
namespace Medical.Service.Dtos.Admin.DepartmentDtos
{
	public class DepartmentPaginatedGetDto
	{
        public int Id { get; set; }

        public string Name { get; set; }

        public string FileUrl { get; set; }

        public int DoctorCount { get; set; }
    }
}

