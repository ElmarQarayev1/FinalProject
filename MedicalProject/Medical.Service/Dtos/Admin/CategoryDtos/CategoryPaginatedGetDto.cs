using System;
namespace Medical.Service.Dtos.Admin.CategoryDtos
{
	public class CategoryPaginatedGetDto
	{
        public int Id { get; set; }

        public string Name { get; set; }

        public int MedicineCount { get; set; }
    }
}

