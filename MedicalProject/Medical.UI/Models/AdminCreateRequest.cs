using System;
using System.ComponentModel.DataAnnotations;

namespace Medical.UI.Models
{
	public class AdminCreateRequest
	{
        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(40)]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

