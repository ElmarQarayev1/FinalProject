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
        public void GetAllUser_WithSearch_ReturnsFilteredDoctors()
        {
            // Arrange
            string search = "John";
            var doctors = new List<Doctor>
    {
        new Doctor { Id = 1, FullName = "John Doe" },
        new Doctor { Id = 2, FullName = "Jane Smith" }
    };

            _doctorRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Doctor, bool>>>())).Returns(doctors.AsQueryable());
            _mapper.Setup(m => m.Map<List<DoctorGetDtoForUser>>(doctors)).Returns(new List<DoctorGetDtoForUser>());

            // Act
            var result = _doctorService.GetAllUser(search);

            // Assert
            _doctorRepository.Verify(x => x.GetAll(It.IsAny<Expression<Func<Doctor, bool>>>()), Times.Once);
            _mapper.Verify(m => m.Map<List<DoctorGetDtoForUser>>(doctors), Times.Once);
            Assert.NotNull(result);
        }

        [Fact]
        public void GetAllUser_NoSearch_ReturnsAllDoctors()
        {
            // Arrange
            var doctors = new List<Doctor>
            {
                new Doctor { Id = 1, FullName = "John Doe" },
                new Doctor { Id = 2, FullName = "Jane Smith" }
            };

            _doctorRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Doctor, bool>>>())).Returns(doctors.AsQueryable());
            _mapper.Setup(m => m.Map<List<DoctorGetDtoForUser>>(doctors)).Returns(new List<DoctorGetDtoForUser>());

            // Act
            var result = _doctorService.GetAllUser();

            // Assert
            _doctorRepository.Verify(x => x.GetAll(It.IsAny<Expression<Func<Doctor, bool>>>()), Times.Once);
            _mapper.Verify(m => m.Map<List<DoctorGetDtoForUser>>(doctors), Times.Once);
            Assert.NotNull(result);
        }
        [Fact]
        public void GetAllUserForDownSide_WithSearch_ReturnsFilteredAndSortedDoctors()
        {
            // Arrange
            string search = "John";
            var doctors = new List<Doctor>
            {
                new Doctor { Id = 1, FullName = "John Doe", CreateAt = DateTime.Now },
                new Doctor { Id = 2, FullName = "Jane Smith", CreateAt = DateTime.Now.AddDays(-1) },
                new Doctor { Id = 3, FullName = "John Smith", CreateAt = DateTime.Now.AddDays(-2) }
            };

            _doctorRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Doctor, bool>>>())).Returns(doctors.AsQueryable());
            _mapper.Setup(m => m.Map<List<DoctorForDownSideDto>>(It.IsAny<List<Doctor>>())).Returns(new List<DoctorForDownSideDto>());

            // Act
            var result = _doctorService.GetAllUserForDownSide(search);

            // Assert
            _doctorRepository.Verify(x => x.GetAll(It.IsAny<Expression<Func<Doctor, bool>>>()), Times.Once);
            _mapper.Verify(m => m.Map<List<DoctorForDownSideDto>>(It.IsAny<List<Doctor>>()), Times.Once);
            Assert.NotNull(result);
            Assert.True(result.Count <= 6);
        }

        [Fact]
        public void GetAllUserForDownSide_NoSearch_ReturnsSortedDoctors()
        {
            // Arrange
            var doctors = new List<Doctor>
            {
                new Doctor { Id = 1, FullName = "John Doe", CreateAt = DateTime.Now },
                new Doctor { Id = 2, FullName = "Jane Smith", CreateAt = DateTime.Now.AddDays(-1) },
                new Doctor { Id = 3, FullName = "John Smith", CreateAt = DateTime.Now.AddDays(-2) },
                new Doctor { Id = 4, FullName = "Alice Johnson", CreateAt = DateTime.Now.AddDays(-3) },
                new Doctor { Id = 5, FullName = "Michael Brown", CreateAt = DateTime.Now.AddDays(-4) },
                new Doctor { Id = 6, FullName = "Chris Green", CreateAt = DateTime.Now.AddDays(-5) }
            };

            _doctorRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Doctor, bool>>>())).Returns(doctors.AsQueryable());
            _mapper.Setup(m => m.Map<List<DoctorForDownSideDto>>(It.IsAny<List<Doctor>>())).Returns(new List<DoctorForDownSideDto>());

            // Act
            var result = _doctorService.GetAllUserForDownSide();

            // Assert
            _doctorRepository.Verify(x => x.GetAll(It.IsAny<Expression<Func<Doctor, bool>>>()), Times.Once);
            _mapper.Verify(m => m.Map<List<DoctorForDownSideDto>>(It.IsAny<List<Doctor>>()), Times.Once);
            Assert.NotNull(result);
            Assert.True(result.Count <= 6);
         }




        [Fact]
        public void GetById_DoctorNotFound_ThrowsRestException()
        {
            // Arrange
            int doctorId = 1;
            _doctorRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Doctor, bool>>>())).Returns((Doctor)null);

            // Act & Assert
            var ex = Assert.Throws<RestException>(() => _doctorService.GetById(doctorId));
            Assert.Equal(StatusCodes.Status404NotFound, ex.Code);
            Assert.Equal("Doctor not found", ex.Message);
        }
        [Fact]
        public void GetByIdForUser_DoctorNotFound_ThrowsRestException()
        {
            // Arrange
            int doctorId = 1;
            _doctorRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Doctor, bool>>>())).Returns((Doctor)null);

            // Act & Assert
            var ex = Assert.Throws<RestException>(() => _doctorService.GetByIdForUser(doctorId));

            Assert.Equal(StatusCodes.Status404NotFound, ex.Code);
            Assert.Equal("Doctor not found", ex.Message); 
        }


        [Fact]
        public void GetAll_WithSearch_ReturnsFilteredDoctors()
        {
           // Arrange
            string search = "John";
            var doctors = new List<Doctor>
                {
                    new Doctor { Id = 1, FullName = "John Doe", CreateAt = DateTime.Now },
                    new Doctor { Id = 2, FullName = "Jane Smith", CreateAt = DateTime.Now }
                };

            _doctorRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Doctor, bool>>>())).Returns(doctors.AsQueryable());
            _mapper.Setup(m => m.Map<List<DoctorGetDto>>(doctors)).Returns(new List<DoctorGetDto>());

            // Act
            var result = _doctorService.GetAll(search);

            // Assert
            _doctorRepository.Verify(x => x.GetAll(It.IsAny<Expression<Func<Doctor, bool>>>()), Times.Once);
            _mapper.Verify(m => m.Map<List<DoctorGetDto>>(doctors), Times.Once);
            Assert.NotNull(result);
        }

        [Fact]
        public void GetForUserHome_WithSearch_ReturnsFilteredAndRandomDoctors()
        {
            // Arrange
            string search = "John";
                    var doctors = new List<Doctor>
            {
                new Doctor { Id = 1, FullName = "John Doe", CreateAt = DateTime.Now },
                new Doctor { Id = 2, FullName = "Jane Smith", CreateAt = DateTime.Now.AddDays(-1) },
                new Doctor { Id = 3, FullName = "John Smith", CreateAt = DateTime.Now.AddDays(-2) },
                new Doctor { Id = 4, FullName = "Alice Johnson", CreateAt = DateTime.Now.AddDays(-3) },
                new Doctor { Id = 5, FullName = "Michael Brown", CreateAt = DateTime.Now.AddDays(-4) }
            };

            _doctorRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Doctor, bool>>>())).Returns(doctors.AsQueryable());
            _mapper.Setup(m => m.Map<List<DoctorGetDtoForUser>>(It.IsAny<List<Doctor>>())).Returns(new List<DoctorGetDtoForUser>());

            // Act
            var result = _doctorService.GetForUserHome(search);

            // Assert
            _doctorRepository.Verify(x => x.GetAll(It.IsAny<Expression<Func<Doctor, bool>>>()), Times.Once);
            _mapper.Verify(m => m.Map<List<DoctorGetDtoForUser>>(It.IsAny<List<Doctor>>()), Times.Once);
            Assert.NotNull(result);
            Assert.True(result.Count <= 4);
        }

        [Fact]
        public void GetForUserHome_NoSearch_ReturnsRandomDoctors()
        {
            // Arrange
            var doctors = new List<Doctor>
            {
                new Doctor { Id = 1, FullName = "John Doe", CreateAt = DateTime.Now },
                new Doctor { Id = 2, FullName = "Jane Smith", CreateAt = DateTime.Now.AddDays(-1) },
                new Doctor { Id = 3, FullName = "John Smith", CreateAt = DateTime.Now.AddDays(-2) },
                new Doctor { Id = 4, FullName = "Alice Johnson", CreateAt = DateTime.Now.AddDays(-3) },
                new Doctor { Id = 5, FullName = "Michael Brown", CreateAt = DateTime.Now.AddDays(-4) }
            };

            _doctorRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Doctor, bool>>>())).Returns(doctors.AsQueryable());
            _mapper.Setup(m => m.Map<List<DoctorGetDtoForUser>>(It.IsAny<List<Doctor>>())).Returns(new List<DoctorGetDtoForUser>());

            // Act
            var result = _doctorService.GetForUserHome();

            // Assert
            _doctorRepository.Verify(x => x.GetAll(It.IsAny<Expression<Func<Doctor, bool>>>()), Times.Once);
            _mapper.Verify(m => m.Map<List<DoctorGetDtoForUser>>(It.IsAny<List<Doctor>>()), Times.Once);
            Assert.NotNull(result);
            Assert.True(result.Count <= 4);
        }

        [Fact]
        public void GetAll_NoSearch_ReturnsAllDoctors()
        {
            // Arrange
                var doctors = new List<Doctor>
        {
            new Doctor { Id = 1, FullName = "John Doe", CreateAt = DateTime.Now },
            new Doctor { Id = 2, FullName = "Jane Smith", CreateAt = DateTime.Now }
        };

            _doctorRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Doctor, bool>>>())).Returns(doctors.AsQueryable());
            _mapper.Setup(m => m.Map<List<DoctorGetDto>>(doctors)).Returns(new List<DoctorGetDto>());

            // Act
            var result = _doctorService.GetAll();

            // Assert
            _doctorRepository.Verify(x => x.GetAll(It.IsAny<Expression<Func<Doctor, bool>>>()), Times.Once);
            _mapper.Verify(m => m.Map<List<DoctorGetDto>>(doctors), Times.Once);
            Assert.NotNull(result);
        }

        [Fact]
        public void GetById_DoctorFound_ReturnsDoctorGetDto()
        {
            // Arrange
            int doctorId = 1;
            var doctor = new Doctor { Id = doctorId, FullName = "John Doe" };
            _doctorRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Doctor, bool>>>())).Returns(doctor);

            var expectedDto = new DoctorGetDto { Id = doctorId, FullName = "John Doe" };
            _mapper.Setup(m => m.Map<DoctorGetDto>(It.IsAny<Doctor>())).Returns(expectedDto);

            // Act
            var result = _doctorService.GetById(doctorId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.Id, result.Id);
            Assert.Equal(expectedDto.FullName, result.FullName);
        }

        [Fact]
        public void GetByIdForAppointment_DepartmentNotFound_ThrowsRestException()
        {
            // Arrange
            int departmentId = 1;
            _departmentRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Department, bool>>>())).Returns((Department)null);

            // Act & Assert
            var ex = Assert.Throws<RestException>(() => _doctorService.GetByIdForAppointment(departmentId));

            Assert.Equal(StatusCodes.Status404NotFound, ex.Code);

           
            Assert.NotEmpty(ex.Errors);
            var error = ex.Errors.First();
            Assert.Equal("departmentId", error.Key);
            Assert.Equal("Department not found", error.Message); 
        }


        [Fact]
        public void GetByIdForAppointment_DepartmentFound_ReturnsDoctorGetDtoForAppointmentList()
        {
            // Arrange
            int departmentId = 1;
            var department = new Department { Id = departmentId };
            _departmentRepository.Setup(x => x.Get(It.IsAny<Expression<Func<Department, bool>>>())).Returns(department);

            var doctors = new List<Doctor>
            {
                new Doctor { Id = 1, FullName = "John Doe", DepartmentId = departmentId },
                new Doctor { Id = 2, FullName = "Jane Smith", DepartmentId = departmentId }
            };

            _doctorRepository.Setup(x => x.GetAll(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<string[]>()))
                .Returns(doctors.AsQueryable());

            // Act
            var result = _doctorService.GetByIdForAppointment(departmentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("John Doe", result[0].FullName);
            Assert.Equal("Jane Smith", result[1].FullName);
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
