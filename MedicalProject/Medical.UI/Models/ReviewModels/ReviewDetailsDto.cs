using System;
namespace Medical.UI.Models
{
	public class ReviewDetailsDto
	{
        public int Id { get; set; }

        public string? FullName { get; set; }

        public DateTime? CreatedAt { get; set; }
        public string CreatedAtFormatted => CreatedAt?.ToString("MM/dd/yyyy HH:mm");

        public string MedicineName { get; set; }

        public string Status { get; set; }

        public string Text { get; set; }

        public byte Rate { get; set; }
    }
}

