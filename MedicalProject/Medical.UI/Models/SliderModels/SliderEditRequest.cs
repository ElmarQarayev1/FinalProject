using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Medical.UI.Models
{
	public class SliderEditRequest
	{

        [Required]
        public string MainTitle { get; set; }

        [Required]
        public string SubTitle1 { get; set; }

        [Required]
        public string SubTitle2 { get; set; }


        public IFormFile? File { get; set; }

        [JsonIgnore]
        public string? FileUrl { get; set; }

        [Required]
        public int Order { get; set; }
    }
}

