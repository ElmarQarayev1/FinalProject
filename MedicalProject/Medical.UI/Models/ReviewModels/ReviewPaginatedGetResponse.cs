using System;
namespace Medical.UI.Models
{
	public class ReviewPaginatedGetResponse
	{
        public int Id { get; set; }
        public string? FullName { get; set; }

        public DateTime? CreatedAt { get; set; }
        public string CreatedAtFormatted => CreatedAt?.ToString("MM/dd/yyyy HH:mm");

        public string Status { get; set; }

        public string Text { get; set; }

       
    }
}

