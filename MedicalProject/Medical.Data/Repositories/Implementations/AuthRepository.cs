using System;
using Medical.Core.Entities;
using Medical.Data.Repositories.Interfaces;

namespace Medical.Data.Repositories.Implementations
{
	public class AuthRepository : Repository<AppUser>, IAuthRepository
    {
        public AuthRepository(AppDbContext context) : base(context)
        {

        }
    }
}
