using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Medical.Core.Entities;
using Medical.Data.Repositories.Interfaces;
using Medical.Service.Dtos.Admin.MedicineDtos;
using Medical.Service.Exceptions;
using Medical.Service.Implementations.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Xunit;
using Medical.Service.Dtos.User.MedicineDtos;
using Medical.Service;

namespace Medical.Api.Test.Services
{
    public class MedicineServiceTest
    {
        private readonly Mock<IMedicineRepository> _medicineRepository;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IWebHostEnvironment> _env;
        private readonly Mock<ICategoryRepository> _categoryRepository;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
        private readonly MedicineService _medicineService;

        public MedicineServiceTest()
        {
            _medicineRepository = new Mock<IMedicineRepository>();
            _mapper = new Mock<IMapper>();
            _env = new Mock<IWebHostEnvironment>();
            _categoryRepository = new Mock<ICategoryRepository>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();

            _medicineService = new MedicineService(
                _medicineRepository.Object,
                _httpContextAccessor.Object,
                null, 
                _mapper.Object,
                _categoryRepository.Object,
                _env.Object,
                null 
            );
        }
       
        [Fact]
        public void Create_CategoryDoesNotExist_ThrowRestException()
        {
            // Arrange
            var createDto = new MedicineCreateDto
            {
                Name = "MedicineName",
                CategoryId = 1,
                Price = 10.0,
                Desc = "Description"
            };

            _categoryRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Category, bool>>>())).Returns((Category)null);

            // Act
            var exception = Assert.Throws<RestException>(() => _medicineService.Create(createDto));

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, exception.Code);
            Assert.Equal("Category not found", exception.Message);
        }

        [Fact]
        public void Create_MedicineNameAlreadyExists_ThrowRestException()
        {
            // Arrange
            var createDto = new MedicineCreateDto
            {
                Name = "ExistingMedicineName",
                CategoryId = 1,
                Price = 10.0,
                Desc = "Description"
            };

            _categoryRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Category, bool>>>())).
                Returns(new Category { Id = createDto.CategoryId });

            _medicineRepository.Setup(x => x.Exists(It.IsAny<Expression<Func<Medicine, bool>>>())).Returns(true);

            // Act
            var exception = Assert.Throws<RestException>(() => _medicineService.Create(createDto));

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, exception.Code);
            Assert.NotEmpty(exception.Errors);
            var error = exception.Errors.First();
            Assert.Equal("Name", error.Key);
            Assert.Equal("MedicineName already exists", error.Message);
        }

        [Fact]
        public void Create_Success_ReturnId()
        {
            // Arrange
            var createDto = new MedicineCreateDto
            {
                Name = "NewMedicineName",
                CategoryId = 1,
                Price = 10.0,
                Desc = "Description",
                Files = new List<IFormFile>()
            };

            var medicine = new Medicine { Id = 1, Name = createDto.Name };

            _categoryRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Category, bool>>>())).
                Returns(new Category { Id = createDto.CategoryId });

            _medicineRepository.Setup(x => x.Exists(It.IsAny<Expression<Func<Medicine, bool>>>())).Returns(false);
            _medicineRepository.Setup(x => x.Add(It.IsAny<Medicine>())).Callback<Medicine>(m => m.Id = 1);
            _medicineRepository.Setup(x => x.Save());

            // Act
            var result = _medicineService.Create(createDto);

            // Assert
            Assert.Equal(1, result);
        }
        [Fact]
        public void Delete_MedicineExists_Success()
        {
            // Arrange
            int medicineId = 1;
            var medicine = new Medicine
            {
                Id = medicineId,
                MedicineImages = new List<MedicineImage> { new MedicineImage { ImageName = "image.jpg" } }
            };

           
            var webRootPath = "mockedPath";
            _env.Setup(x => x.WebRootPath).Returns(webRootPath);

            _medicineRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Medicine, bool>>>(), "MedicineImages")).Returns(medicine);
            _medicineRepository.Setup(x => x.Delete(It.IsAny<Medicine>()));
            _medicineRepository.Setup(x => x.Save());

            // Act
            _medicineService.Delete(medicineId);

            // Assert
            _medicineRepository.Verify(x => x.Delete(It.IsAny<Medicine>()), Times.Once);
            _medicineRepository.Verify(x => x.Save(), Times.Once);
        }


        [Fact]
        public void Delete_MedicineDoesNotExist_ThrowRestException()
        {
            // Arrange
            int medicineId = 1;

            _medicineRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Medicine, bool>>>(), "MedicineImages")).Returns((Medicine)null);

            // Act
            var exception = Assert.Throws<RestException>(() => _medicineService.Delete(medicineId));

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, exception.Code);
            Assert.Equal("Medicine not found", exception.Message);
        }
        [Fact]
        public void GetAll_ReturnsMappedMedicines()
        {
            // Arrange
            var medicines = new List<Medicine> { new Medicine { Name = "Medicine1" } };
            var medicineDtos = new List<MedicineGetDto> { new MedicineGetDto { Name = "Medicine1" } };

            _medicineRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Medicine, bool>>>())).Returns(medicines.AsQueryable());
            _mapper.Setup(m => m.Map<List<MedicineGetDto>>(medicines)).Returns(medicineDtos);

            // Act
            var result = _medicineService.GetAll("Medicine");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Medicine1", result.First().Name);
        }

        [Fact]
        public void GetAllLatest_ReturnsLatestMappedMedicines()
        {
            // Arrange
            var medicines = new List<Medicine>
         {
        new Medicine { Name = "Medicine1", CreateAt = DateTime.Now.AddDays(-1) },
        new Medicine { Name = "Medicine2", CreateAt = DateTime.Now }
          };

            var medicineDtos = new List<MedicineGetDtoLatest>
         {
        new MedicineGetDtoLatest { Name = "Medicine2" },
        new MedicineGetDtoLatest { Name = "Medicine1" }
         };

            _medicineRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Medicine, bool>>>(), "MedicineReviews", "MedicineImages"))
                .Returns(medicines.AsQueryable().OrderByDescending(x => x.CreateAt));

            _mapper.Setup(m => m.Map<List<MedicineGetDtoLatest>>(It.IsAny<List<Medicine>>())).Returns(medicineDtos);

            // Act
            var result = _medicineService.GetAllLatest();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Medicine2", result.First().Name);
        }
        [Fact]
        public void GetAllByPage_ReturnsPaginatedMappedMedicines()
        {
            // Arrange
            var medicines = new List<Medicine> { new Medicine { Name = "Medicine1" } };
            var paginatedMedicines = PaginatedList<Medicine>.Create(medicines.AsQueryable(), 1, 10);
            var medicineDtos = new List<MedicinePaginatedGetDto> { new MedicinePaginatedGetDto { Name = "Medicine1" } };

            _medicineRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Medicine, bool>>>(), "Category"))
                .Returns(medicines.AsQueryable());

            _mapper.Setup(m => m.Map<List<MedicinePaginatedGetDto>>(It.IsAny<List<Medicine>>())).Returns(medicineDtos);

            // Act
            var result = _medicineService.GetAllByPage("Medicine", 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalPages);
            Assert.Single(result.Items);
            Assert.Equal("Medicine1", result.Items.First().Name);
        }


        [Fact]
        public void GetAllByPageForUser_ReturnsPaginatedMappedMedicinesForUser()
        {
            // Arrange
            var categoryId = 1;
            var category = new Category { Id = categoryId, Name = "Category1" };

            _categoryRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Category, bool>>>()))
                .Returns(category);

            var medicines = new List<Medicine> { new Medicine { Name = "Medicine1", CategoryId = categoryId } };
            var paginatedMedicines = PaginatedList<Medicine>.Create(medicines.AsQueryable(), 1, 9);
            var medicineDtos = new List<MedicinePaginatedGetDtoForUser> { new MedicinePaginatedGetDtoForUser { Name = "Medicine1" } };

            _medicineRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Medicine, bool>>>(), "MedicineImages", "MedicineReviews"))
                .Returns(medicines.AsQueryable());

            _mapper.Setup(m => m.Map<List<MedicinePaginatedGetDtoForUser>>(It.IsAny<List<Medicine>>())).Returns(medicineDtos);

            // Act
            var result = _medicineService.GetAllByPageForUser("Medicine", 1, 9, categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalPages);
            Assert.Single(result.Items);
            Assert.Equal("Medicine1", result.Items.First().Name);
        }
        [Fact]
        public void GetAllByPageForUser_NoCategoryId_ReturnsPaginatedMappedMedicinesForUser()
        {
            // Arrange
            int? categoryId = null;  
            var medicines = new List<Medicine> { new Medicine { Name = "Medicine1", CategoryId = 1 } };
            var paginatedMedicines = PaginatedList<Medicine>.Create(medicines.AsQueryable(), 1, 9);
            var medicineDtos = new List<MedicinePaginatedGetDtoForUser> { new MedicinePaginatedGetDtoForUser { Name = "Medicine1" } };

            _medicineRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Medicine, bool>>>(), "MedicineImages", "MedicineReviews"))
                .Returns(medicines.AsQueryable());

            _mapper.Setup(m => m.Map<List<MedicinePaginatedGetDtoForUser>>(It.IsAny<List<Medicine>>())).Returns(medicineDtos);

            // Act
            var result = _medicineService.GetAllByPageForUser("Medicine", 1, 9, categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalPages);
            Assert.Single(result.Items);
            Assert.Equal("Medicine1", result.Items.First().Name);
        }


        [Fact]
        public void GetById_ExistingMedicine_ReturnsMappedMedicine()
        {
            // Arrange
            var medicine = new Medicine { Id = 1, Name = "Medicine1" };
            var medicineDto = new MedicineDetailsDto { Name = "Medicine1" };

            _medicineRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Medicine, bool>>>(), "MedicineImages"))
                .Returns(medicine);

            _mapper.Setup(m => m.Map<MedicineDetailsDto>(medicine)).Returns(medicineDto);

            // Act
            var result = _medicineService.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Medicine1", result.Name);
        }
        [Fact]
        public void GetByIdForUser_ExistingMedicine_ReturnsMappedMedicineForUser()
        {
            // Arrange
            var medicine = new Medicine { Id = 1, Name = "Medicine1" };
            var medicineDto = new MedicineGetDtoForUser { Name = "Medicine1" };

            _medicineRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Medicine, bool>>>(), "MedicineImages", "MedicineReviews", "MedicineReviews.AppUser"))
                .Returns(medicine);

            _mapper.Setup(m => m.Map<MedicineGetDtoForUser>(medicine)).Returns(medicineDto);

            // Act
            var result = _medicineService.GetByIdForUser(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Medicine1", result.Name);
        }
   
        [Fact]
        public void Update_CategoryNotFound_ThrowsRestException()
        {
            // Arrange
            int medicineId = 1;
            var updateDto = new MedicineUpdateDto
            {
                CategoryId = 99 
            };

            _categoryRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Category, bool>>>())).Returns((Category)null);

            // Act & Assert
            var ex = Assert.Throws<RestException>(() => _medicineService.Update(medicineId, updateDto));
            Assert.Equal(StatusCodes.Status404NotFound, ex.Code);
            Assert.Equal("Category not found", ex.Message);
        }
        [Fact]
        public void Update_MedicineNotFound_ThrowsRestException()
        {
            // Arrange
            int medicineId = 99; 
            var updateDto = new MedicineUpdateDto
            {
                CategoryId = 1
            };

            var category = new Category { Id = updateDto.CategoryId };
            _categoryRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Category, bool>>>())).Returns(category);
            _medicineRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Medicine, bool>>>(), "MedicineImages")).Returns((Medicine)null);

            // Act & Assert
            var ex = Assert.Throws<RestException>(() => _medicineService.Update(medicineId, updateDto));
            Assert.Equal(StatusCodes.Status404NotFound, ex.Code);
            Assert.NotEmpty(ex.Errors);
            var error = ex.Errors.First();
            Assert.Equal("Id", error.Key); 
            Assert.Equal("Medicine not found by given Id", error.Message); 
        }

        [Fact]
        public void Update_MedicineNameExists_ThrowsRestException()
        {
            // Arrange
            int medicineId = 1;
            var updateDto = new MedicineUpdateDto
            {
                Name = "Existing Medicine",
                CategoryId = 1
            };

            var category = new Category { Id = updateDto.CategoryId };
            var medicine = new Medicine { Id = medicineId, Name = "Original Medicine" };

            _categoryRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Category, bool>>>())).Returns(category);
            _medicineRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Medicine, bool>>>(), "MedicineImages")).Returns(medicine);
            _medicineRepository.Setup(x => x.Exists(It.IsAny<Expression<Func<Medicine, bool>>>())).Returns(true);

            // Act & Assert
            var ex = Assert.Throws<RestException>(() => _medicineService.Update(medicineId, updateDto));

            Assert.NotEmpty(ex.Errors);
            var error = ex.Errors.First();
            Assert.Equal("Name", error.Key);
           
            Assert.Equal(StatusCodes.Status400BadRequest, ex.Code);
            Assert.Equal("MedicineName already exists",error.Message);
        }


    }
}
