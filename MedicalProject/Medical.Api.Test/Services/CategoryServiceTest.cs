using System;
using System.Linq.Expressions;
using AutoMapper;
using Medical.Core.Entities;
using Medical.Data.Repositories.Interfaces;
using Medical.Service;
using Medical.Service.Dtos.Admin.CategoryDtos;
using Medical.Service.Dtos.User.CategoryDtos;
using Medical.Service.Exceptions;
using Medical.Service.Implementations.Admin;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Medical.Api.Test.Services
{
	public class CategoryServiceTest
	{
		public Mock<ICategoryRepository> _categoryRepository;

		public Mock<IMapper> _mapper;
        private readonly CategoryService _categoryService;

        public CategoryServiceTest()
		{
			_categoryRepository = new Mock<ICategoryRepository>();
			_mapper = new Mock<IMapper>();
            _categoryService = new CategoryService(_categoryRepository.Object, _mapper.Object);

        }

		[Fact]
		public void Create_NoExists_ThrowRestException()
		{
			//Arrange
			CategoryCreateDto categoryCreateDto = new CategoryCreateDto()
			{
				Name = "Antidepressants",

            };

            _categoryRepository.Setup(x => x.Exists(It.IsAny<Expression<Func<Category, bool>>>())).Returns(true);


            CategoryService categoryService = new CategoryService(_categoryRepository.Object, _mapper.Object);
            //Act


            var r = Assert.Throws<RestException>(() => categoryService.Create(categoryCreateDto));

            //Assert



            Assert.NotNull(r);
			Assert.Equal(r.Code, 400);
		}

        [Fact]
        public void Create_CategoryNameAlreadyExists_ThrowsRestException()
        {
            // Arrange
            var createDto = new CategoryCreateDto
            {
                Name = "ExistingCategoryName",
            };

            
            _categoryRepository.Setup(repo => repo.Exists(It.IsAny<Expression<Func<Category, bool>>>()))
                                   .Returns(true);

            // Act & Assert
            var exception = Assert.Throws<RestException>(() => _categoryService.Create(createDto));

          
            Assert.Equal(StatusCodes.Status400BadRequest, exception.Code);

          
            Assert.NotEmpty(exception.Errors); 
            var error = exception.Errors.First();
            Assert.Equal("Name", error.Key); 
            Assert.Equal("CategoryName already taken", error.Message); 
        }


        [Fact]
        public void Create_Success_ReturnId()
        {
            // Arrange
            var categoryCreateDto = new CategoryCreateDto
            {
                Name = "Antd",
            };

            var entity = new Category { Name = categoryCreateDto.Name, Id = 1 };

            _mapper.Setup(x => x.Map<Category>(categoryCreateDto)).Returns(entity);
            _categoryRepository.Setup(x => x.Exists(It.IsAny<Expression<Func<Category, bool>>>())).Returns(false);
            _categoryRepository.Setup(x => x.Add(It.IsAny<Category>())).Callback<Category>(c => c.Id = 1); 
            _categoryRepository.Setup(x => x.Save());

            var categoryService = new CategoryService(_categoryRepository.Object, _mapper.Object);

            // Act
            var result = categoryService.Create(categoryCreateDto);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void Delete_CategoryExists_Success()
        {
            // Arrange
            int categoryId = 1;
            var category = new Category { Id = categoryId };

            _categoryRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Category, bool>>>())).Returns(category);
            _categoryRepository.Setup(x => x.Delete(It.IsAny<Category>()));
            _categoryRepository.Setup(x => x.Save());

            var categoryService = new CategoryService(_categoryRepository.Object, _mapper.Object);

            // Act
            categoryService.Delete(categoryId);

            // Assert
            _categoryRepository.Verify(x => x.Delete(It.IsAny<Category>()), Times.Once);
            _categoryRepository.Verify(x => x.Save(), Times.Once);
        }
        [Fact]
        public void Delete_CategoryDoesNotExist_ThrowRestException()
        {
            // Arrange
            int categoryId = 1;

            _categoryRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Category, bool>>>())).Returns((Category)null);

            var categoryService = new CategoryService(_categoryRepository.Object, _mapper.Object);

            // Act
            var exception = Assert.Throws<RestException>(() => categoryService.Delete(categoryId));

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, exception.Code);
            Assert.Equal("Category not found", exception.Message);
        }



        [Fact]
        public void GetAll_ReturnsListOfCategories()
        {
            // Arrange
            var categories = new List<Category>
             {
                new Category { Id = 1, Name = "Category1" },
                 new Category { Id = 2, Name = "Category2" }
            }.AsQueryable();



            var categoryDtos = new List<CategoryGetDto>
             {
                   new CategoryGetDto { Id = 1, Name = "Category1" },
                   new CategoryGetDto { Id = 2, Name = "Category2" }
            };

            _categoryRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<string[]>()))
                .Returns(categories); 

            _mapper.Setup(x => x.Map<List<CategoryGetDto>>(categories)).Returns(categoryDtos);

            var categoryService = new CategoryService(_categoryRepository.Object, _mapper.Object);

            // Act
            var result = categoryService.GetAll();

            // Assert
            Assert.Equal(categoryDtos.Count, result.Count);
            Assert.Equal(categoryDtos[0].Name, result[0].Name);
        }


        [Fact]
        public void GetAllByPage_ReturnsPaginatedCategories()
        {
            // Arrange
                var categories = new List<Category>
                {
                  new Category { Id = 1, Name = "Category1" },
                  new Category { Id = 2, Name = "Category2" }
                 };

            var categoryDtos = new List<CategoryPaginatedGetDto>
                 {
                     new CategoryPaginatedGetDto { Id = 1, Name = "Category1" },
                      new CategoryPaginatedGetDto { Id = 2, Name = "Category2" }
                 };

            var paginatedList = new PaginatedList<Category>(categories, 1, 1, 10);

            _categoryRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<string>()))
                .Returns(categories.AsQueryable());

            _mapper.Setup(x => x.Map<List<CategoryPaginatedGetDto>>(categories)).Returns(categoryDtos);

            var categoryService = new CategoryService(_categoryRepository.Object, _mapper.Object);

            // Act
            var result = categoryService.GetAllByPage();

            // Assert
            Assert.Equal(categoryDtos.Count, result.Items.Count);
            Assert.Equal(1, result.TotalPages);
        }



        [Fact]
        public void GetAllUser_ReturnsListOfCategoriesForUser()
        {
            // Arrange
            var categories = new List<Category>
             {
                new Category { Id = 1, Name = "UserCategory1" },
                 new Category { Id = 2, Name = "UserCategory2" }
              }.AsQueryable(); 

            var categoryDtosForUser = new List<CategoryGetDtoForUser>
               {
                   new CategoryGetDtoForUser { Id = 1, Name = "UserCategory1" },
                   new CategoryGetDtoForUser { Id = 2, Name = "UserCategory2" }
              };

        
            _categoryRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<string[]>()))
                .Returns(categories);

        
            _mapper.Setup(x => x.Map<List<CategoryGetDtoForUser>>(categories)).Returns(categoryDtosForUser);

            var categoryService = new CategoryService(_categoryRepository.Object, _mapper.Object);

            // Act
            var result = categoryService.GetAllUser();

            // Assert
            Assert.Equal(categoryDtosForUser.Count, result.Count);
            Assert.Equal(categoryDtosForUser[0].Name, result[0].Name);
        }

        [Fact]
        public void GetById_CategoryExists_ReturnsCategoryDto()
        {
            // Arrange
            int categoryId = 1;
            var category = new Category { Id = categoryId, Name = "Category" };
            var categoryDto = new CategoryGetDto { Id = categoryId, Name = "Category" };

            _categoryRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Category, bool>>>())).Returns(category);
            _mapper.Setup(x => x.Map<CategoryGetDto>(category)).Returns(categoryDto);

            var categoryService = new CategoryService(_categoryRepository.Object, _mapper.Object);

            // Act
            var result = categoryService.GetById(categoryId);

            // Assert
            Assert.Equal(categoryDto.Name, result.Name);
        }


        [Fact]
        public void GetById_CategoryDoesNotExist_ThrowRestException()
        {
            // Arrange
            int categoryId = 1;

            _categoryRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Category, bool>>>())).Returns((Category)null);

            var categoryService = new CategoryService(_categoryRepository.Object, _mapper.Object);

            // Act
            var exception = Assert.Throws<RestException>(() => categoryService.GetById(categoryId));

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, exception.Code);
            Assert.Equal("Category not found", exception.Message);
        }


        [Fact]
        public void Update_CategoryExists_Success()
        {
            // Arrange
            int categoryId = 1;
            var category = new Category { Id = categoryId, Name = "OldName" };
            var updateDto = new CategoryUpdateDto { Name = "NewName" };

            _categoryRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Category, bool>>>())).Returns(category);
            _categoryRepository.Setup(x => x.Exists(It.IsAny<Expression<Func<Category, bool>>>())).Returns(false);

            var categoryService = new CategoryService(_categoryRepository.Object, _mapper.Object);

            // Act
            categoryService.Update(categoryId, updateDto);

            // Assert
            Assert.Equal("NewName", category.Name);
            _categoryRepository.Verify(x => x.Save(), Times.Once);
        }
        [Fact]
        public void Update_CategoryNameAlreadyExists_ThrowRestException()
        {
            // Arrange
            int categoryId = 1;
            var category = new Category { Id = categoryId, Name = "OldName" };
            var updateDto = new CategoryUpdateDto { Name = "ExistingName" };

            _categoryRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Category, bool>>>())).Returns(category);
            _categoryRepository.Setup(x => x.Exists(It.IsAny<Expression<Func<Category, bool>>>())).Returns(true);

            var categoryService = new CategoryService(_categoryRepository.Object, _mapper.Object);

            // Act
            var exception = Assert.Throws<RestException>(() => categoryService.Update(categoryId, updateDto));

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, exception.Code);
            Assert.NotEmpty(exception.Errors);
            var error = exception.Errors.First();
            Assert.Equal("Name", error.Key);
            Assert.Equal("CategoryName already taken", error.Message);
           

            
        }


        [Fact]
        public void Update_CategoryDoesNotExist_ThrowRestException()
        {
            // Arrange
            int categoryId = 1;
            var updateDto = new CategoryUpdateDto { Name = "NewName" };

            _categoryRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Category, bool>>>())).Returns((Category)null);

            var categoryService = new CategoryService(_categoryRepository.Object, _mapper.Object);

            // Act
            var exception = Assert.Throws<RestException>(() => categoryService.Update(categoryId, updateDto));

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, exception.Code);
            Assert.Equal("Category not found", exception.Message);
        }

    }
}

