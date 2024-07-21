using System;
using Medical.Core.Entities;
using Medical.Data.Repositories.Interfaces;

namespace Medical.Data.Repositories.Implementations
{
	public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
		public DepartmentRepository(AppDbContext context):base(context)
		{
		}
	}
}

