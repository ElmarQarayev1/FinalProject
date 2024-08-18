using System;
using Medical.Core.Enum;

namespace Medical.Service.Dtos.User.ReviewDtos
{
	public class ReviewPaginatedGetDto
	{
        public int Id { get; set; }
        public string? FullName { get; set; }       
              
        public DateTime CreatedAt { get; set; } 

        public string Status { get; set; } 

        public string Text { get; set; }

      
     
    }
}

