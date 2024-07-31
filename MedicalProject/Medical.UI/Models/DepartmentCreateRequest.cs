using System;
using System.ComponentModel.DataAnnotations;

namespace Medical.UI.Models
{
	public class DepartmentCreateRequest
	{
        [Required]
        public string Name { get; set; }

        [Required]
        public IFormFile File { get; set; }
	}
}

