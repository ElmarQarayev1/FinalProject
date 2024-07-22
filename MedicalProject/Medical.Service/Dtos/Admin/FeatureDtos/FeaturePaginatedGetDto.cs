using System;
namespace Medical.Service.Dtos.Admin.FeatureDtos
{
	public class FeaturePaginatedGetDto
	{
        public int Id { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

        public string FileUrl { get; set; }
    }
}

