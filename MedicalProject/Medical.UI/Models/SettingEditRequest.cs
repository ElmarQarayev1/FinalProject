using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Medical.UI.Models
{
    public class SettingEditRequest
    {
        [Required]
        [MinLength(2)]
        public string Value { get; set; }
       
    }
}
