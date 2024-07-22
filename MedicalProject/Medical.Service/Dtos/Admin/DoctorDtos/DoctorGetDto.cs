using System;
using Medical.Core.Entities;

namespace Medical.Service.Dtos.Admin.DoctorDtos
{
	public class DoctorGetDto
	{
        public string Name { get; set; }

        public int DepartmentId { get; set; }

        public string Phone { get; set; }

        public string FileUrl { get; set; }

    }
}

