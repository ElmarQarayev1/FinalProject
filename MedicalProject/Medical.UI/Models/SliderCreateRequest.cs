using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Medical.UI.Models
{
	public class SliderCreateRequest
	{
        [Required]
        public string MainTitle { get; set; }

        [Required]
        public string SubTitle1 { get; set; }

        [Required]
        public string SubTitle2 { get; set; }


        public IFormFile? FileUrl { get; set; }

        [Required]
        public int Order { get; set; }
    }
}

