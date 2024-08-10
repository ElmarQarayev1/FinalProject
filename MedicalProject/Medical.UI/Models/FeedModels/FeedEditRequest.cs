using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Medical.UI.Models
{
	public class FeedEditRequest
	{
        [Required]
        public string Name { get; set; }

        [Required]
        [MaxLength(400)]
        public string Desc { get; set; }

        public DateTime Date { get; set; }

        public IFormFile? File { get; set; }

        [JsonIgnore]
        public string? FileUrl { get; set; }
    }
}

