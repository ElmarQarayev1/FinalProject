using System;
using System.Linq.Expressions;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Medical.Core.Entities;
using Medical.Data.Repositories.Interfaces;
using Medical.Service;
using Medical.Service.Dtos.Admin.DoctorDtos;
using Medical.Service.Dtos.User.DoctorDtos;
using Medical.Service.Exceptions;
using Medical.Service.Helpers;
using Medical.Service.Implementations.Admin;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace Medical.Api.Test.Services
{
    public class DoctorServiceTest
    {
        private readonly Mock<IDoctorRepository> _doctorRepository;
        private readonly Mock<IWebHostEnvironment> _env;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IDepartmentRepository> _departmentRepository;
        private readonly DoctorService _doctorService;
      
        public DoctorServiceTest()
        {
            _doctorRepository = new Mock<IDoctorRepository>();
            _env = new Mock<IWebHostEnvironment>();
            _mapper = new Mock<IMapper>();
            _departmentRepository = new Mock<IDepartmentRepository>();
          

            _env.Setup(e => e.WebRootPath).Returns("wwwroot");
            _mapper.Setup(m => m.Map<Doctor>(It.IsAny<DoctorCreateDto>())).Returns(new Doctor());

            _doctorService = new DoctorService(
             _doctorRepository.Object,
             _env.Object,
             _mapper.Object,
             _departmentRepository.Object 
             

         );
        }

        [Fact]
        public void Create_DepartmentNotFound_ThrowsRestException()
        {
            // Arrange
            var createDto = new DoctorCreateDto
            {
                FullName = "John Doe",
                Email = "johndoe@example.com",
                DepartmentId = 999, 
                Desc = "A skilled doctor",
                Position = "Surgeon",
                Address = "123 Medical St.",
                Phone = "123-456-7890",
                ResilienceSkil = 5,
                EthicSkil = 5,
                CompassionSkil = 5
            };

            _departmentRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Department, bool>>>())).Returns((Department)null);

            var service = new DoctorService(_doctorRepository.Object, _env.Object, _mapper.Object, _departmentRepository.Object);

            // Act & Assert
            var ex = Assert.Throws<RestException>(() => service.Create(createDto));
            Assert.Equal(StatusCodes.Status404NotFound, ex.Code);
            Assert.Equal("Department not found", ex.Message);
        }

        [Fact]
        public void Create_DoctorWithSameEmailExists_ThrowsRestException()
        {
            // Arrange
            var createDto = new DoctorCreateDto
            {
                FullName = "John Doe",
                Email = "johndoe@example.com",
                DepartmentId = 1,
                Desc = "A skilled doctor",
                Position = "Surgeon",
                Address = "123 Medical St.",
                Phone = "123-456-7890",
                ResilienceSkil = 5,
                EthicSkil = 5,
                CompassionSkil = 5
            };

            var existingDoctor = new Doctor { Email = "johndoe@example.com" };

            _doctorRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Doctor, bool>>>())).
                Returns(existingDoctor);

            _departmentRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Department, bool>>>())).
                Returns(new Department { Id = createDto.DepartmentId });

            var service = new DoctorService(_doctorRepository.Object, _env.Object, _mapper.Object, _departmentRepository.Object);

            // Act & Assert
            var ex = Assert.Throws<RestException>(() => service.Create(createDto));
            Assert.Equal(StatusCodes.Status400BadRequest, ex.Code);
            Assert.NotEmpty(ex.Errors);
            var error = ex.Errors.First();
            Assert.Equal("Email", error.Key);
            Assert.Equal("Doctor already exists by given Email", error.Message);
        }

        [Fact]
        public void Delete_DoctorNotFound_ThrowsRestException()
        {
            // Arrange
            int doctorId = 1;
            _doctorRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Doctor, bool>>>())).Returns((Doctor)null);

            // Act & Assert
            var ex = Assert.Throws<RestException>(() => _doctorService.Delete(doctorId));
            Assert.Equal(StatusCodes.Status404NotFound, ex.Code);
            Assert.Equal("Doctor not found", ex.Message);
        }

    }
}
