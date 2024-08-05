using System;
using Medical.Core.Entities;
using Medical.Data.Repositories.Interfaces;

namespace Medical.Data.Repositories.Implementations
{
	public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
    {
		public AppointmentRepository(AppDbContext context):base(context)
		{

		}
		
		
	}
}

