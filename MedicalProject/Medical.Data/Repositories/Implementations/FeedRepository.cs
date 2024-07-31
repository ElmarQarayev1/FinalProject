using System;
using Medical.Core.Entities;
using Medical.Data.Repositories.Interfaces;

namespace Medical.Data.Repositories.Implementations
{
	public class FeedRepository : Repository<Feed>, IFeedRepository
    {
        public FeedRepository(AppDbContext context) : base(context)
        {

        }
    }
}

