using System;
namespace Medical.UI.Models
{
	public class DoctorGetResponse
	{
        public string FullName { get; set; }

        public string Position { get; set; }

        public int DepartmentId { get; set; }

        public string Desc { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string? FileUrl { get; set; }

        public string? TwitterUrl { get; set; }

        public string? InstagramUrl { get; set; }

        public string? VimeoUrl { get; set; }

        public string? BehanceUrl { get; set; }

        public double ResilienceSkil { get; set; }

        public double EthicSkil { get; set; }

        public double CompassionSkil { get; set; }
    }
}

