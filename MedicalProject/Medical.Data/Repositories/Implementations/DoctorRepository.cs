using System;
using Medical.Core.Entities;
using Medical.Data.Repositories.Interfaces;

namespace Medical.Data.Repositories.Implementations
{
	public class DoctorRepository : Repository<Doctor>, IDoctorRepository
    {
		public DoctorRepository(AppDbContext context):base(context)
		{
		}
	}
}

