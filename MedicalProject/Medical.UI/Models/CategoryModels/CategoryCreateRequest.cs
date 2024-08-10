using System;
using System.ComponentModel.DataAnnotations;

namespace Medical.UI.Models
{
	public class CategoryCreateRequest
	{
        [Required]
        [MinLength(2)]
        public string Name { get; set; }
    }
}

