using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Medical.UI.Models
{
	public class DepartmentEditRequest
	{

        [Required]
        public string Name { get; set; }

        public IFormFile? File { get; set; }

        [JsonIgnore]
        public string? FileUrl { get; set; }
    }
}

