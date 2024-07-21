using System;
using Medical.Core.Entities;
using Medical.Data.Repositories.Interfaces;

namespace Medical.Data.Repositories.Implementations
{
	public class FeatureRepository : Repository<Feature>, IFeatureRepository
    {
		public FeatureRepository(AppDbContext context):base(context)
		{
		}
	}
}

