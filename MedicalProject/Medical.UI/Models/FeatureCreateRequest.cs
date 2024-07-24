using System;
using System.ComponentModel.DataAnnotations;

namespace Medical.UI.Models
{
	public class FeatureCreateRequest
	{
        [Required]
        public string Name { get; set; }

        [Required]
        [MaxLength(400)]
        public string Desc { get; set; }

        public IFormFile? File { get; set; }
    }
}

