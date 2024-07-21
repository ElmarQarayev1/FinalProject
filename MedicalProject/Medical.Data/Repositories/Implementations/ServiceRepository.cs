using System;
using Medical.Core.Entities;
using Medical.Data.Repositories.Interfaces;

namespace Medical.Data.Repositories.Implementations
{
	public class ServiceRepository : Repository<Service>, IServiceRepository
    {
		public ServiceRepository(AppDbContext context):base(context)
		{
		}
	}
}

