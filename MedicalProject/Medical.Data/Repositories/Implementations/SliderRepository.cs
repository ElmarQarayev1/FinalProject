using System;
using Medical.Core.Entities;
using Medical.Data.Repositories.Interfaces;

namespace Medical.Data.Repositories.Implementations
{
	public class SliderRepository : Repository<Slider>, ISliderRepository
    {
		public SliderRepository(AppDbContext context):base(context)
		{
		}
	}
}

