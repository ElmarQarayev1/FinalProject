using System;
using AutoMapper;
using FluentValidation;
using Medical.Core.Entities;
using Medical.Data.Repositories.Implementations;
using Medical.Data.Repositories.Interfaces;
using Medical.Service.Dtos.Admin.FeatureDtos;
using Medical.Service.Dtos.Admin.FeedDtos;
using Medical.Service.Dtos.User.FeatureDtos;
using Medical.Service.Dtos.User.FeedDtos;
using Medical.Service.Exceptions;
using Medical.Service.Helpers;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Medical.Service.Implementations.Admin
{
	public class FeedService:IFeedService
	{
        private readonly IFeedRepository _feedRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public FeedService(IFeedRepository feedRepository, IMapper mapper, IWebHostEnvironment env)
        {
            _feedRepository = feedRepository;
            _mapper = mapper;
            _env = env;
        }

        public int Create(FeedCreateDto createDto)
        {
            if (createDto.Date < DateTime.Today)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Date", "Date cannot be in the past.");
            }

            var validator = new FeedCreateDtoValidator();

            var validationResult = validator.Validate(createDto);


            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            Feed feed = new Feed()
            {
                Name = createDto.Name,
                Desc = createDto.Desc,
                Date = createDto.Date,
                ImageName = FileManager.Save(createDto.File, _env.WebRootPath, "uploads/feeds")

            };

            _feedRepository.Add(feed);
            _feedRepository.Save();

            return feed.Id;
        }

        public void Delete(int id)
        {
            Feed feed = _feedRepository.Get(x => x.Id == id);

            if (feed == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Feed not found");
            }

            _feedRepository.Delete(feed);

            _feedRepository.Save();

            FileManager.Delete(_env.WebRootPath, "uploads/feeds", feed.ImageName);
        }

        public List<FeedGetDto> GetAll(string? search = null)
        {
            var feeds = _feedRepository.GetAll(x => search == null || x.Name.Contains(search)).ToList();
            return _mapper.Map<List<FeedGetDto>>(feeds);
        }

        public List<FeedGetDtoForUser> GetAllUser(string? search = null)
        {
            var feeds = _feedRepository.GetAll(x => search == null || x.Name.Contains(search)).ToList();

            var random = new Random();
            feeds = feeds.OrderBy(x => random.Next()).ToList();

            var selectedFeatures = feeds.Take(3).ToList();

            return _mapper.Map<List<FeedGetDtoForUser>>(selectedFeatures);
        }

        public PaginatedList<FeedPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var query = _feedRepository.GetAll(x => x.Name.Contains(search) || search == null);


            var paginated = PaginatedList<Feed>.Create(query, page, size);

            var feedDtos = _mapper.Map<List<FeedPaginatedGetDto>>(paginated.Items);

            return new PaginatedList<FeedPaginatedGetDto>(feedDtos, paginated.TotalPages, page, size);
        }

        public FeedGetDto GetById(int id)
        {
            Feed feed = _feedRepository.Get(x => x.Id == id);

            if (feed == null)
                throw new RestException(StatusCodes.Status404NotFound, "Feed not found");

            return _mapper.Map<FeedGetDto>(feed);
        }

        public void Update(int id, FeedUpdateDto updateDto)
        {
            var feed = _feedRepository.Get(x => x.Id == id);
            if (feed == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Id", "Feed not found by given Id");
            }

            if (updateDto.Date < DateTime.Today)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Date", "Date cannot be in the past.");
            }

            var validator = new FeedUpdateDtoValidator();

            var validationResult = validator.Validate(updateDto);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            string deletedFile = feed.ImageName;

            if (!string.IsNullOrEmpty(updateDto.Name))
            {
                feed.Name = updateDto.Name;
            }

            if (!string.IsNullOrEmpty(updateDto.Desc))
            {

                feed.Desc = updateDto.Desc;
            }

            feed.Date = updateDto.Date;

            if (updateDto.File != null)
            {
                feed.ImageName = FileManager.Save(updateDto.File, _env.WebRootPath, "uploads/feeds");
            }

            _feedRepository.Save();


            if (deletedFile != null && deletedFile != feed.ImageName)
            {
                FileManager.Delete(_env.WebRootPath, "uploads/feeds", deletedFile);
            }
        }
    }
}

