using System;
using Medical.Core.Entities;
using Medical.Data.Repositories.Interfaces;

namespace Medical.Data.Repositories.Implementations
{
	public class ReviewRepository : Repository<MedicineReview>, IReviewRepository
    {
		public ReviewRepository(AppDbContext context):base(context)
		{
		}
	}
}

