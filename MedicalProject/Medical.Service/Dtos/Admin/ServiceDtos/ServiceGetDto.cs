using System;
using Microsoft.AspNetCore.Http;

namespace Medical.Service.Dtos.Admin.ServiceDtos
{
	public class ServiceGetDto
	{
       
        public string Name { get; set; }

        public string Desc { get; set; }

        public string FileUrl { get; set; }
    }
}

