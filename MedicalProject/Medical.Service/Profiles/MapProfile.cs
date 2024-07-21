using System;
using AutoMapper;
using Medical.Core.Entities;
using Medical.Service.Dtos.Admin.CategoryDtos;
using Medical.Service.Dtos.Admin.SliderDtos;
using Microsoft.AspNetCore.Http;

namespace Medical.Service.Profiles
{
    public class MapProfile : Profile
    {
        private readonly IHttpContextAccessor _context;

        public MapProfile(IHttpContextAccessor httpContextAccessor)
        {
            _context = httpContextAccessor;

            var uriBuilder = new UriBuilder(_context.HttpContext.Request.Scheme, _context.HttpContext.Request.Host.Host, _context.HttpContext.Request.Host.Port ?? -1);

            if (uriBuilder.Uri.IsDefaultPort)
            {
                uriBuilder.Port = -1;
            }
            string baseUrl = uriBuilder.Uri.AbsoluteUri;



            //categories
            CreateMap<Category, CategoryGetDto>();

            CreateMap<Category, CategoryPaginatedGetDto>()
                 .ForMember(dest => dest.MedicineCount, s => s.MapFrom(s => s.Medicines.Count));






            //Sliders
            CreateMap<Slider, SliderCreateDto>().ReverseMap();
            CreateMap<Slider, SliderPaginatedGetDto>()
                 .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => baseUrl + "/uploads/sliders/" + src.ImageName));

            CreateMap<Slider, SliderGetDto>()
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => baseUrl + "/uploads/sliders/" + src.ImageName));

        }
    }
}

