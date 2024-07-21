using System;
using Microsoft.AspNetCore.Http;

namespace Medical.Service.Dtos.Admin.SliderDtos
{
	public class SliderPaginatedGetDto
	{
        public int Id { get; set; }

        public string MainTitle { get; set; }

        public string SubTitle1 { get; set; }

        public string SubTitle2 { get; set; }

        public string FileUrl { get; set; }

        public int Order { get; set; }
    }
}

