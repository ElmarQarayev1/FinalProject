using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Medical.UI.Models
{
	public class FeatureEditRequest
	{
        [Required]
        public string Name { get; set; }

        [Required]
        [MaxLength(400)]
        public string Desc { get; set; }

        public IFormFile? File { get; set; }

        [JsonIgnore]
        public string? FileUrl { get; set; }
    }
}

