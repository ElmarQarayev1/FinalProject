using System;
using Medical.Core.Entities;
using Medical.Data.Repositories.Interfaces;

namespace Medical.Data.Repositories.Implementations
{
	public class SettingRepository : Repository<Setting>, ISettingRepository
    {
		public SettingRepository(AppDbContext context):base(context)
		{
		}
	}
}

