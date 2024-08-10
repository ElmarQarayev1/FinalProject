using System;
using System.ComponentModel.DataAnnotations;

namespace Medical.UI.Models
{
	public class MedicineCreateRequest
	{
        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public string Desc { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public List<IFormFile> Files { get; set; }
    }
}

