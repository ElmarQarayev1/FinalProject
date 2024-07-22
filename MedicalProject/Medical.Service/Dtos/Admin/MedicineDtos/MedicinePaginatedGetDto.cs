using System;
namespace Medical.Service.Dtos.Admin.MedicineDtos
{
	public class MedicinePaginatedGetDto
	{
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public int CategoryId { get; set; }
    }
}

