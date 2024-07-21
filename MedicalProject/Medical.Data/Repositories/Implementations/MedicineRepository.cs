using System;
using Medical.Core.Entities;
using Medical.Data.Repositories.Interfaces;

namespace Medical.Data.Repositories.Implementations
{
	public class MedicineRepository : Repository<Medicine>, IMedicineRepository
    {
		public MedicineRepository(AppDbContext context):base(context)
		{
		}
	}
}

