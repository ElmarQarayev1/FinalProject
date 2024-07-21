﻿using System;
using Medical.Service.Dtos.Admin.CategoryDtos;

namespace Medical.Service.Interfaces.Admin
{
	public interface ICategoryService
	{
        int Create(CategoryCreateDto createDto);
        PaginatedList<CategoryPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        List<CategoryGetDto> GetAll(string? search = null);
        CategoryGetDto GetById(int id);
        void Update(int id, CategoryUpdateDto updateDto);
        void Delete(int id);
    }
}
