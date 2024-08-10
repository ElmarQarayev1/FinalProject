using System;
namespace Medical.UI.Models
{
	public class DepartmentListItemGetResponse
	{
        public int Id { get; set; }

        public string Name { get; set; }

        public string FileUrl { get; set; }

        public int DoctorCount { get; set; }
    }
}

