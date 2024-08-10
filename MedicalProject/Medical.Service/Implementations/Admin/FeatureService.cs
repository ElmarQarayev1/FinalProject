using System;
using AutoMapper;
using FluentValidation;
using Medical.Core.Entities;
using Medical.Data.Repositories.Implementations;
using Medical.Data.Repositories.Interfaces;
using Medical.Service.Dtos.Admin.FeatureDtos;
using Medical.Service.Dtos.Admin.ServiceDtos;
using Medical.Service.Dtos.User.FeatureDtos;
using Medical.Service.Exceptions;
using Medical.Service.Helpers;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Medical.Service.Implementations.Admin
{
    public class FeatureService : IFeatureService
    {
        private readonly IFeatureRepository _featureRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public FeatureService(IFeatureRepository featureRepository,IMapper mapper,IWebHostEnvironment env)
        {
            _featureRepository = featureRepository;
            _mapper = mapper;
            _env = env;
        }

        public int Create(FeatureCreateDto createDto)
        {
            var validator = new FeatureCreateDtoValidator();

            var validationResult = validator.Validate(createDto);


            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            if (_featureRepository.Exists(x => x.Name == createDto.Name))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Name", "Feature already exists");
            }

            Feature feature = new Feature()
            {
                Name = createDto.Name,
                Desc = createDto.Desc,
                ImageName = FileManager.Save(createDto.File, _env.WebRootPath, "uploads/features")

            };

            _featureRepository.Add(feature);
            _featureRepository.Save();

            return feature.Id;
        }

        public void Delete(int id)
        {
            Feature feature = _featureRepository.Get(x => x.Id == id);

            if (feature == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Feature not found");
            }

            _featureRepository.Delete(feature);

            _featureRepository.Save();

            FileManager.Delete(_env.WebRootPath, "uploads/features", feature.ImageName);
        }

        public List<FeatureGetDto> GetAll(string? search = null)
        {
            var features = _featureRepository.GetAll(x => search == null || x.Name.Contains(search)).ToList();
            return _mapper.Map<List<FeatureGetDto>>(features);
        }

        public PaginatedList<FeaturePaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var query = _featureRepository.GetAll(x => x.Name.Contains(search) || search == null);


            var paginated = PaginatedList<Feature>.Create(query, page, size);

            var featureDtos = _mapper.Map<List<FeaturePaginatedGetDto>>(paginated.Items);

            return new PaginatedList<FeaturePaginatedGetDto>(featureDtos, paginated.TotalPages, page, size);
        }

        public List<FeatureGetDtoForUser> GetAllUser(string? search = null)
        {
            var features = _featureRepository.GetAll(x => search == null || x.Name.Contains(search)).ToList();
            return _mapper.Map<List<FeatureGetDtoForUser>>(features);

        }

        public FeatureGetDto GetById(int id)
        {
            Feature feature = _featureRepository.Get(x => x.Id == id);

            if (feature == null)
                throw new RestException(StatusCodes.Status404NotFound, "Feature not found");

            return _mapper.Map<FeatureGetDto>(feature);
        }


        public List<FeatureGetDtoForUser> GetForUserHome(string? search=null)
        {
            var features = _featureRepository.GetAll(x => search == null || x.Name.Contains(search)).ToList();

            var random = new Random();
            features = features.OrderBy(x => random.Next()).ToList();
          
            var selectedFeatures = features.Take(4).ToList();
         
            return _mapper.Map<List<FeatureGetDtoForUser>>(selectedFeatures);
        }



        public void Update(int id, FeatureUpdateDto updateDto)
        {
            var feature = _featureRepository.Get(x => x.Id == id);
            if (feature == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Id", "Feature not found by given Id");
            }

            if (feature.Name != updateDto.Name && _featureRepository.Exists(x => x.Name == updateDto.Name))
                throw new RestException(StatusCodes.Status400BadRequest, "Name", "Feature already taken");


            var validator = new FeatureUpdateDtoValidator();

            var validationResult = validator.Validate(updateDto);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }


            string deletedFile = feature.ImageName;

            if (!string.IsNullOrEmpty(updateDto.Name))
            {
                feature.Name = updateDto.Name;
            }

            if (!string.IsNullOrEmpty(updateDto.Desc))
            {
                feature.Desc = updateDto.Desc;
            }

            if (updateDto.File!= null)
            {
                feature.ImageName = FileManager.Save(updateDto.File, _env.WebRootPath, "uploads/features");
            }

            _featureRepository.Save();


            if (deletedFile != null && deletedFile != feature.ImageName)
            {
                FileManager.Delete(_env.WebRootPath, "uploads/features", deletedFile);
            }
        }
    }
}

