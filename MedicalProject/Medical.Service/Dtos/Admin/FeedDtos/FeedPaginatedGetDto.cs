using System;
namespace Medical.Service.Dtos.Admin.FeedDtos
{
	public class FeedPaginatedGetDto
	{
        public int Id { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

        public DateTime Date { get; set; }

        public string FileUrl { get; set; }
    }
}

