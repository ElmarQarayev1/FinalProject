using System;
using System.ComponentModel.DataAnnotations;

namespace Medical.UI.Models
{
	public class FeedCreateRequest
	{
        [Required]
        [MaxLength(65)]
        public string Name { get; set; }

        [Required]
        [MaxLength(400)]
        public string Desc { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public IFormFile File { get; set; }
    }
}

