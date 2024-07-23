using System;
using AutoMapper;
using FluentValidation;
using Medical.Core.Entities;
using Medical.Data.Repositories.Interfaces;
using Medical.Service.Dtos.Admin.SliderDtos;
using Medical.Service.Exceptions;
using Medical.Service.Helpers;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Medical.Service.Implementations.Admin
{
	public class SliderService:ISliderService
	{
        public ISliderRepository _sliderRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public SliderService(ISliderRepository sliderRepository, IMapper mapper, IWebHostEnvironment env)
        {
            _sliderRepository = sliderRepository;
            _mapper = mapper;
            _env = env;
        }

        public int Create(SliderCreateDto createDto)
        {
            var validator = new SliderCreateDtoValidator();

            var validationResult = validator.Validate(createDto);


            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            Slider slider = new Slider()
            {
                MainTitle=createDto.MainTitle,
                SubTitle1=createDto.SubTitle1,
                SubTitle2=createDto.SubTitle2,
                Order=createDto.Order,
                ImageName=FileManager.Save(createDto.FileUrl,_env.WebRootPath,"uploads/sliders")

            };

            _sliderRepository.Add(slider);
            _sliderRepository.Save();

            return slider.Id;

        }

        public void Delete(int id)
        {
            Slider slider = _sliderRepository.Get(x=>x.Id==id);

            if (slider == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Slider not found");
            }

            _sliderRepository.Delete(slider);

            _sliderRepository.Save();

            FileManager.Delete(_env.WebRootPath, "uploads/sliders", slider.ImageName);
        }


        public List<SliderGetDto> GetAll(string? search = null)
        {
            var sliders = _sliderRepository.GetAll(x => search == null || x.MainTitle.Contains(search)).ToList();
            return _mapper.Map<List<SliderGetDto>>(sliders);
        }

        public PaginatedList<SliderPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var query = _sliderRepository.GetAll(x => x.MainTitle.Contains(search) || search == null);


            var paginated = PaginatedList<Slider>.Create(query, page, size);

            var sliderDtos = _mapper.Map<List<SliderPaginatedGetDto>>(paginated.Items);

            return new PaginatedList<SliderPaginatedGetDto>(sliderDtos, paginated.TotalPages, page, size);
        }

        public SliderGetDto GetById(int id)
        {
            Slider slider = _sliderRepository.Get(x => x.Id == id);

            if (slider == null)
                throw new RestException(StatusCodes.Status404NotFound, "Slider not found");

            return _mapper.Map<SliderGetDto>(slider);
        }

        public void Update(int id, SliderUpdateDto updateDto)
        {
            var slider = _sliderRepository.Get(x => x.Id == id);
            if (slider == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Id", "Slider not found by given Id");
            }

            var validator = new SliderUpdateDtoValidator();
            var validationResult = validator.Validate(updateDto);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }


            string deletedFile = slider.ImageName;

            if (!string.IsNullOrEmpty(updateDto.MainTitle))
            {
                slider.MainTitle = updateDto.MainTitle;
            }

            if (!string.IsNullOrEmpty(updateDto.SubTitle1))
            {
                slider.SubTitle1 = updateDto.SubTitle1;
            }
            if (!string.IsNullOrEmpty(updateDto.SubTitle2))
            {
                slider.SubTitle2 = updateDto.SubTitle2;
            }

            if (updateDto.Order > 0)
            {
                slider.Order = updateDto.Order;
            }


            if (updateDto.File != null)
            {
                slider.ImageName = FileManager.Save(updateDto.File, _env.WebRootPath, "uploads/sliders");
            }

            _sliderRepository.Save();


            if (deletedFile != null && deletedFile != slider.ImageName)
            {
                FileManager.Delete(_env.WebRootPath, "uploads/sliders", deletedFile);
            }
        }
    }
}

