﻿using System;
namespace Medical.Service.Dtos.Admin.FeatureDtos
{
	public class FeatureGetDto
	{
        public int Id { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

        public string FileUrl { get; set; }
    }
}

