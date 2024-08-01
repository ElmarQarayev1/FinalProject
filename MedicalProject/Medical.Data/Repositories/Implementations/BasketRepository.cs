using System;
using Medical.Core.Entities;
using Medical.Data.Repositories.Interfaces;

namespace Medical.Data.Repositories.Implementations
{
	public class BasketRepository : Repository<BasketItem>, IBasketRepository
    {
        public BasketRepository(AppDbContext context) : base(context)
        {

        }
    }
}

