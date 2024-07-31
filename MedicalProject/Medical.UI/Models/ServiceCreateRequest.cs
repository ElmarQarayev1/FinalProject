using System;
using System.ComponentModel.DataAnnotations;

namespace Medical.UI.Models
{
	public class ServiceCreateRequest
	{
        [Required]
        public string Name { get; set; }

        [Required]
        [MaxLength(400)]
        public string Desc { get; set; }

        [Required]
        public IFormFile File { get; set; }
    }
}

