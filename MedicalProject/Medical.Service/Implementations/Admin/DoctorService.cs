using System;
using AutoMapper;
using FluentValidation;
using Medical.Core.Entities;
using Medical.Data.Repositories.Implementations;
using Medical.Data.Repositories.Interfaces;
using Medical.Service.Dtos.Admin.DoctorDtos;
using Medical.Service.Dtos.Admin.ServiceDtos;
using Medical.Service.Dtos.User.DoctorDtos;
using Medical.Service.Dtos.User.FeatureDtos;
using Medical.Service.Dtos.User.OrderDtos;
using Medical.Service.Exceptions;
using Medical.Service.Helpers;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medical.Service.Implementations.Admin
{
	public class DoctorService:IDoctorService
	{
		private readonly IDoctorRepository _doctorRepository;

		private readonly IWebHostEnvironment _env;

        private readonly IDepartmentRepository _departmentRepository;

		private readonly IMapper _mapper;

		public DoctorService(IDoctorRepository doctorRepository,IWebHostEnvironment env,IMapper mapper,IDepartmentRepository departmentRepository)
		{
			_doctorRepository = doctorRepository;
			_env = env;
			_mapper = mapper;
            _departmentRepository = departmentRepository;

		}

        public int Create(DoctorCreateDto createDto)
        {
            var validator = new DoctorCreateDtoValidator();

            var validationResult = validator.Validate(createDto);


            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var department = _departmentRepository.Get(x => x.Id == createDto.DepartmentId);

            if (department == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Department not found");
            }

            Doctor doctorEmail = _doctorRepository.Get(x => x.Email == createDto.Email);

            if (doctorEmail != null)
            {
                throw new RestException(StatusCodes.Status404NotFound,"Email", "Doctor already exists by given Email");
            }

            Doctor doctorPhone = _doctorRepository.Get(x => x.Phone == createDto.Phone);

            if (doctorPhone != null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Phone", "Doctor already exists by given Phone");
            }

            Doctor doctor = new Doctor()
            {
                FullName = createDto.FullName,
                Desc = createDto.Desc,
                ImageName = FileManager.Save(createDto.File, _env.WebRootPath, "uploads/doctors"),
                InstagramUrl = createDto.InstagramUrl,
                TwitterUrl=createDto.TwitterUrl,
                VimeoUrl=createDto.VimeoUrl,
                BehanceUrl=createDto.BehanceUrl,
                Position=createDto.Position,
                Address=createDto.Address,
                Phone=createDto.Phone,
                DepartmentId=createDto.DepartmentId,
                Email=createDto.Email,
                EthicSkil=createDto.EthicSkil,
                CompassionSkil=createDto.CompassionSkil,
                ResilienceSkil=createDto.ResilienceSkil

            };

            _doctorRepository.Add(doctor);
            _doctorRepository.Save();

            return doctor.Id;
        }

        public void Delete(int id)
        {
            Doctor doctor = _doctorRepository.Get(x => x.Id == id);

            if (doctor == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Doctor not found");
            }

            _doctorRepository.Delete(doctor);

            _doctorRepository.Save();

            FileManager.Delete(_env.WebRootPath, "uploads/doctors", doctor.ImageName);
        }

        public List<DoctorGetDto> GetAll(string? search = null)
        {
            var doctors = _doctorRepository.GetAll(x => search == null || x.FullName.Contains(search)).ToList();
            return _mapper.Map<List<DoctorGetDto>>(doctors);
        }

        public List<DoctorGetDtoForUser> GetForUserHome(string? search = null)
        {
           
            var doctors = _doctorRepository.GetAll(x =>
                search == null || (x.FullName != null && x.FullName.Contains(search)))
                .ToList(); 

           
            doctors = doctors.OrderByDescending(x => x.CreateAt).ToList();

            
            var random = new Random();
            doctors = doctors.OrderBy(x => random.Next()).ToList();

           
            var selectedDoctors = doctors.Take(4).ToList();

            
            return _mapper.Map<List<DoctorGetDtoForUser>>(selectedDoctors);
        }



        public List<DoctorGetDtoForUser> GetAllUser(string? search = null)
        {
            var doctors = _doctorRepository.GetAll(x => search == null || x.FullName.Contains(search)).ToList();
            return _mapper.Map<List<DoctorGetDtoForUser>>(doctors);

        }

        public PaginatedList<DoctorPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var query = _doctorRepository.GetAll(x => x.FullName.Contains(search) || search == null,"Department","Appointments");

            var paginated = PaginatedList<Doctor>.Create(query, page, size);

            var doctorDtos = _mapper.Map<List<DoctorPaginatedGetDto>>(paginated.Items);

            return new PaginatedList<DoctorPaginatedGetDto>(doctorDtos, paginated.TotalPages, page, size);
        }

        public DoctorGetDto GetById(int id)
        {
            Doctor doctor = _doctorRepository.Get(x => x.Id == id);

            if (doctor == null)
                throw new RestException(StatusCodes.Status404NotFound, "Doctor not found");

            return _mapper.Map<DoctorGetDto>(doctor);
        }

        public List<DoctorGetDtoForAppointment> GetByIdForAppointment(int departmentId)
        {
            var department = _departmentRepository.Get(x => x.Id == departmentId);

            if (department == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "departmentId", "Department not found");
            }

            var query = _doctorRepository.GetAll(o => o.DepartmentId == departmentId, "Department")
                .Select(doctor => new DoctorGetDtoForAppointment
                {
                    Id = doctor.Id,
                    FullName = doctor.FullName
                }).ToList();

            return query;
        }

        public DoctorGetDetailDto GetByIdForUser(int id)
        {
            Doctor doctor = _doctorRepository.Get(x => x.Id == id);

            if (doctor == null)
                throw new RestException(StatusCodes.Status404NotFound, "Doctor not found");

            return _mapper.Map<DoctorGetDetailDto>(doctor);
        }
        public void Update(int id, DoctorUpdateDto updateDto)
        {
            var doctor = _doctorRepository.Get(x => x.Id == id);

            if (doctor == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Id", "Doctor not found by given Id");
            }

            var validator = new DoctorUpdateDtoValidator();

            var validationResult = validator.Validate(updateDto);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var department = _departmentRepository.Get(x => x.Id == updateDto.DepartmentId);

            if (department == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Department not found");
            }
            
            if (doctor.Email != updateDto.Email)
            {
                var doctorEmail = _doctorRepository.Get(x => x.Email == updateDto.Email);
                if (doctorEmail != null)
                {
                    throw new RestException(StatusCodes.Status400BadRequest, "There cannot be a doctor with the same email");
                }
            }

            if (doctor.Phone != updateDto.Phone)
            {
                var doctorPhone = _doctorRepository.Get(x => x.Phone == updateDto.Phone);
                if (doctorPhone != null)
                {
                    throw new RestException(StatusCodes.Status400BadRequest, "There cannot be a doctor with the same phone");
                }
            }

            string deletedFile = doctor.ImageName;

            if (!string.IsNullOrEmpty(updateDto.FullName))
            {
                doctor.FullName = updateDto.FullName;
            }

            if (!string.IsNullOrEmpty(updateDto.Position))
            {
                doctor.Position = updateDto.Position;
            }

            if (!string.IsNullOrEmpty(updateDto.Email))
            {
                doctor.Email = updateDto.Email;
            }

            if (!string.IsNullOrEmpty(updateDto.Address))
            {
                doctor.Address = updateDto.Address;
            }
            if (!string.IsNullOrEmpty(updateDto.Phone))
            {
                doctor.Phone = updateDto.Phone;
            }
            if (!string.IsNullOrEmpty(updateDto.TwitterUrl))
            {
                doctor.TwitterUrl = updateDto.TwitterUrl;
            }

            if (!string.IsNullOrEmpty(updateDto.InstagramUrl))
            {
                doctor.InstagramUrl = updateDto.InstagramUrl;
            }
            if (!string.IsNullOrEmpty(updateDto.VimeoUrl))
            {
                doctor.VimeoUrl = updateDto.VimeoUrl;
            }
            if (!string.IsNullOrEmpty(updateDto.BehanceUrl))
            {
                doctor.BehanceUrl = updateDto.BehanceUrl;
            }
             
            if (!string.IsNullOrEmpty(updateDto.Desc))
            {
                doctor.Desc = updateDto.Desc;
            }

            if (updateDto.File != null)
            {
                doctor.ImageName = FileManager.Save(updateDto.File, _env.WebRootPath, "uploads/doctors");
            }

            doctor.DepartmentId = updateDto.DepartmentId;
            doctor.ResilienceSkil = updateDto.ResilienceSkil;
            doctor.CompassionSkil = updateDto.CompassionSkil;
            doctor.EthicSkil = updateDto.EthicSkil;

            _doctorRepository.Save();


            if (deletedFile != null && deletedFile != doctor.ImageName)
            {
                FileManager.Delete(_env.WebRootPath, "uploads/doctors", deletedFile);
            }
        }
    }
}

