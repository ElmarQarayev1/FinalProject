using System;
namespace Medical.UI.Models
{
	public class FeedListItemGetResponse
	{
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string Desc { get; set; }

        public string FileUrl { get; set; }
    }
}

