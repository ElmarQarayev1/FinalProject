using System;
namespace Medical.Service.Dtos.User.ReviewDtos
{
	public class ReviewDetailDto
	{
        public int Id { get; set; }
        public string? FullName { get; set; }

        public DateTime CreatedAt { get; set; }

        public string MedicineName { get; set; }

        public string Status { get; set; }

        public string Text { get; set; }
    }
}

