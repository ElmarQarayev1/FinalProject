using System;
using System.ComponentModel.DataAnnotations;

namespace Medical.UI.Models
{
	public class DepartmentCreateRequest
	{
        [Required]
        public string Name { get; set; }

		public IFormFile? File { get; set; }
	}
}

