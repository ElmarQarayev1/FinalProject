using System;
namespace Medical.UI.Models
{
	public class FeedGetResponse
	{
        public string Name { get; set; }

        public string Desc { get; set; }

        public DateTime Date { get; set; }

        public string? FileUrl { get; set; }
    }
}

