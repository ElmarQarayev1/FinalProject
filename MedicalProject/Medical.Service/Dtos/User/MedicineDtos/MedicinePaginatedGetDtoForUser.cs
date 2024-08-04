using System;
namespace Medical.Service.Dtos.User.MedicineDtos
{
	public class MedicinePaginatedGetDtoForUser
	{
		public int Id { get; set; }

		public string ImagaUrl { get; set; }

		public string Name { get; set; }

        public int TotalReviewsCount { get; set; }

        public int AvgRate { get; set; }

		public double Price { get; set; }

    }
}

