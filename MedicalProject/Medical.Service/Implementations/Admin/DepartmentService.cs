using System;
using AutoMapper;
using FluentValidation;
using Medical.Core.Entities;
using Medical.Data.Repositories.Implementations;
using Medical.Data.Repositories.Interfaces;
using Medical.Service.Dtos.Admin.DepartmentDtos;
using Medical.Service.Dtos.User.DepartmentDtos;
using Medical.Service.Dtos.User.FeatureDtos;
using Medical.Service.Exceptions;
using Medical.Service.Helpers;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Medical.Service.Implementations.Admin
{
    public class DepartmentService : IDepartmentService
    {

        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public DepartmentService(IDepartmentRepository departmentRepository, IMapper mapper, IWebHostEnvironment env)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
            _env = env;
        }

        public int Create(DepartmentCreateDto createDto)
        {
            var validator = new DepartmentCreateDtoValidator();

            var validationResult = validator.Validate(createDto);


            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            Department department = new Department()
            {
                Name = createDto.Name,
                
                ImageName = FileManager.Save(createDto.File, _env.WebRootPath, "uploads/departments")

            };

            _departmentRepository.Add(department);
            _departmentRepository.Save();

            return department.Id;
        }

        public void Delete(int id)
        {
            Department department = _departmentRepository.Get(x => x.Id == id);

            if (department == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Department not found");
            }

            _departmentRepository.Delete(department);

            _departmentRepository.Save();

            FileManager.Delete(_env.WebRootPath, "uploads/departments", department.ImageName);
        }

        public List<DepartmentGetDto> GetAll(string? search = null)
        {
            var departments = _departmentRepository.GetAll(x => search == null || x.Name.Contains(search)).ToList();
            return _mapper.Map<List<DepartmentGetDto>>(departments);
        }

        public PaginatedList<DepartmentPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var query = _departmentRepository.GetAll(x => x.Name.Contains(search) || search == null,"Doctors");


            var paginated = PaginatedList<Department>.Create(query, page, size);

            var departmentDtos = _mapper.Map<List<DepartmentPaginatedGetDto>>(paginated.Items);

            return new PaginatedList<DepartmentPaginatedGetDto>(departmentDtos, paginated.TotalPages, page, size);
        }


        public List<DepartmentGetDtoForUser> GetForUserHome(string? search = null)
        {
            var departments = _departmentRepository.GetAll(x => search == null || x.Name.Contains(search)).ToList();

            var random = new Random();
            departments = departments.OrderBy(x => random.Next()).ToList();

            var selectedDepartments = departments.Take(9).ToList();

            return _mapper.Map<List<DepartmentGetDtoForUser>>(selectedDepartments);
        }
        public List<DepartmentGetDtoForUser> GetAllUser(string? search = null)
        {
            var departments = _departmentRepository.GetAll(x => search == null || x.Name.Contains(search)).ToList();
            return _mapper.Map<List<DepartmentGetDtoForUser>>(departments);

        }

        public DepartmentGetDto GetById(int id)
        {
            Department department = _departmentRepository.Get(x => x.Id == id);

            if (department == null)
                throw new RestException(StatusCodes.Status404NotFound, "Department not found");

            return _mapper.Map<DepartmentGetDto>(department);
        }

        public void Update(int id, DepartmentUpdateDto updateDto)
        {
            var department = _departmentRepository.Get(x => x.Id == id);
            if (department == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Id", "Department not found by given Id");
            }

            var validator = new DepartmentUpdateDtoValidator();

            var validationResult = validator.Validate(updateDto);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }


            string deletedFile = department.ImageName;

            if (!string.IsNullOrEmpty(updateDto.Name))
            {
                department.Name = updateDto.Name;
            }


            if (updateDto.File != null)
            {
                department.ImageName = FileManager.Save(updateDto.File, _env.WebRootPath, "uploads/departments");
            }

            _departmentRepository.Save();


            if (deletedFile != null && deletedFile != department.ImageName)
            {
                FileManager.Delete(_env.WebRootPath, "uploads/departments", deletedFile);
            }
        }
    }
}

