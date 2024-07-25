using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Medical.UI.Models
{
	public class DoctorCreateRequest
	{

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Position { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public string Desc { get; set; }

        [Required]
        public string Address { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        public IFormFile? File { get; set; }

        public string? TwitterUrl { get; set; }

        
        public string? InstagramUrl { get; set; }

    
        public string? VimeoUrl { get; set; }

    
        public string? BehanceUrl { get; set; }

        [Required]
        public double ResilienceSkil { get; set; }

        [Required]
        public double EthicSkil { get; set; }

        [Required]
        public double CompassionSkil { get; set; }
    }
}

