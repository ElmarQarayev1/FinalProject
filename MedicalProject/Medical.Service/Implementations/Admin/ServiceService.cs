using System;
using AutoMapper;
using FluentValidation;
using Medical.Core.Entities;
using Medical.Data.Repositories.Implementations;
using Medical.Data.Repositories.Interfaces;

using Medical.Service.Dtos.Admin.ServiceDtos;
using Medical.Service.Exceptions;
using Medical.Service.Helpers;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Medical.Service.Implementations.Admin
{
	public class ServiceService:IServiceService
	{
        private readonly IServiceRepository _serviceRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public ServiceService(IServiceRepository serviceRepository, IMapper mapper, IWebHostEnvironment env)
        {
            _serviceRepository =serviceRepository;
            _mapper = mapper;
            _env = env;
        }

        public int Create(ServiceCreateDto createDto)
        {
            var validator = new ServiceCreateDtoValidator();

            var validationResult = validator.Validate(createDto);


            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            Medical.Core.Entities.Service service = new Medical.Core.Entities.Service()
            {
                Name = createDto.Name,
                Desc=createDto.Desc,
                ImageName = FileManager.Save(createDto.FileUrl, _env.WebRootPath, "uploads/services")

            };

            _serviceRepository.Add(service);
            _serviceRepository.Save();

            return service.Id;
        }


        public void Delete(int id)
        {
            Medical.Core.Entities.Service service = _serviceRepository.Get(x => x.Id == id);

            if (service == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Service not found");
            }

            _serviceRepository.Delete(service);

            _serviceRepository.Save();

            FileManager.Delete(_env.WebRootPath, "uploads/services", service.ImageName);
        }


        public List<ServiceGetDto> GetAll(string? search = null)
        {
            var services = _serviceRepository.GetAll(x => search == null || x.Name.Contains(search)).ToList();
            return _mapper.Map<List<ServiceGetDto>>(services);
        }


        public PaginatedList<ServicePaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var query = _serviceRepository.GetAll(x => x.Name.Contains(search) || search == null);


            var paginated = PaginatedList<Medical.Core.Entities.Service>.Create(query, page, size);

            var serviceDtos = _mapper.Map<List<ServicePaginatedGetDto>>(paginated.Items);

            return new PaginatedList<ServicePaginatedGetDto>(serviceDtos, paginated.TotalPages, page, size);
        }


        public ServiceGetDto GetById(int id)
        {
            Medical.Core.Entities.Service service = _serviceRepository.Get(x => x.Id == id);

            if (service == null)
                throw new RestException(StatusCodes.Status404NotFound, "Service not found");

            return _mapper.Map<ServiceGetDto>(service);
        }


        public void Update(int id, ServiceUpdateDto updateDto)
        {
            var service = _serviceRepository.Get(x => x.Id == id);
            if (service == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Id", "Service not found by given Id");
            }

            var validator = new ServiceUpdateDtoValidator();

            var validationResult = validator.Validate(updateDto);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }


            string deletedFile = service.ImageName;

            if (!string.IsNullOrEmpty(updateDto.Name))
            {
                service.Name = updateDto.Name;
            }

            if (!string.IsNullOrEmpty(updateDto.Desc))
            {
                service.Desc = updateDto.Desc;
            }

            if (updateDto.FileUrl != null)
            {
                service.ImageName = FileManager.Save(updateDto.FileUrl, _env.WebRootPath, "uploads/services");
            }

            _serviceRepository.Save();


            if (deletedFile != null && deletedFile != service.ImageName)
            {
                FileManager.Delete(_env.WebRootPath, "uploads/services", deletedFile);
            }
        }
    }
}

