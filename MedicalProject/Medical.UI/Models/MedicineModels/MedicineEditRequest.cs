using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Medical.UI.Models
{
	public class MedicineEditRequest
	{
        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public string Desc { get; set; }

        [Required]
        public int CategoryId { get; set; }

       
        public List<IFormFile>? Files { get; set; }

        [JsonIgnore]
        public List<PictureResponse>? FileUrls { get; set; }

        public List<int>? ExistPictureIds { get; set; } = new List<int>();
    }
}

