using System;
using AutoMapper;
using Medical.Core.Entities;
using Medical.Data.Repositories.Interfaces;
using Medical.Service.Dtos.Admin.CategoryDtos;
using Medical.Service.Exceptions;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Http;

namespace Medical.Service.Implementations.Admin
{
    public class CategoryService : ICategoryService
    {

        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;


        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public int Create(CategoryCreateDto createDto)
        {
            if (_categoryRepository.Exists(x => x.Name == createDto.Name))
                throw new RestException(StatusCodes.Status400BadRequest, "Name", "CategoryName already taken");
     
            var entity = new Category
            {
                Name = createDto.Name,          
            };

            _categoryRepository.Add(entity);
            _categoryRepository.Save();

            return entity.Id;
        }

        public void Delete(int id)
        {
            Category entity = _categoryRepository.Get(x => x.Id == id);

            if (entity == null)
                throw new RestException(StatusCodes.Status404NotFound, "Category not found");

            _categoryRepository.Delete(entity);

            _categoryRepository.Save();
        }

        public List<CategoryGetDto> GetAll(string? search = null)
        {
            var categories = _categoryRepository.GetAll(x => search == null || x.Name.Contains(search)).ToList();
            return _mapper.Map<List<CategoryGetDto>>(categories);
        }

        public PaginatedList<CategoryPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var query = _categoryRepository.GetAll(x => x.Name.Contains(search) || search == null, "Medicines");


            var paginated = PaginatedList<Category>.Create(query, page, size);

            var categoryDtos = _mapper.Map<List<CategoryPaginatedGetDto>>(paginated.Items);

            return new PaginatedList<CategoryPaginatedGetDto>(categoryDtos, paginated.TotalPages, page, size);
        }

        public CategoryGetDto GetById(int id)
        {
            Category category = _categoryRepository.Get(x => x.Id == id);

            if (category == null)
                throw new RestException(StatusCodes.Status404NotFound, "Category not found");

            return _mapper.Map<CategoryGetDto>(category);
        }

        public void Update(int id, CategoryUpdateDto updateDto)
        {
            Category entity = _categoryRepository.Get(x => x.Id == id);

            if (entity == null)
                throw new RestException(StatusCodes.Status404NotFound, "Category not found");

            if (entity.Name != updateDto.Name && _categoryRepository.Exists(x => x.Name == updateDto.Name))
                throw new RestException(StatusCodes.Status400BadRequest, "Name", "CategoryName already taken");

            entity.Name = updateDto.Name;

            _categoryRepository.Save();
        }
    }
}

