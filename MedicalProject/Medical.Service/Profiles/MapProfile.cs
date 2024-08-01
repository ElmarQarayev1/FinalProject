using System;
using AutoMapper;
using Medical.Core.Entities;
using Medical.Service.Dtos.Admin.AuthDtos;
using Medical.Service.Dtos.Admin.CategoryDtos;
using Medical.Service.Dtos.Admin.DepartmentDtos;
using Medical.Service.Dtos.Admin.DoctorDtos;
using Medical.Service.Dtos.Admin.FeatureDtos;
using Medical.Service.Dtos.Admin.FeedDtos;
using Medical.Service.Dtos.Admin.MedicineDtos;
using Medical.Service.Dtos.Admin.ServiceDtos;
using Medical.Service.Dtos.Admin.SettingDtos;
using Medical.Service.Dtos.Admin.SliderDtos;
using Medical.Service.Dtos.User.DepartmentDtos;
using Medical.Service.Dtos.User.DoctorDtos;
using Medical.Service.Dtos.User.FeatureDtos;
using Medical.Service.Dtos.User.FeedDtos;
using Medical.Service.Dtos.User.ServiceDtos;
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






            //sliders
            CreateMap<Slider, SliderCreateDto>().ReverseMap();
            CreateMap<Slider, SliderPaginatedGetDto>()
                 .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => baseUrl + "/uploads/sliders/" + src.ImageName));

            CreateMap<Slider, SliderGetDto>()
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => baseUrl + "/uploads/sliders/" + src.ImageName));


            //departments
            CreateMap<Department, DepartmentCreateDto>().ReverseMap();
            CreateMap<Department,DepartmentPaginatedGetDto>()
                .ForMember(dest=>dest.DoctorCount,s=>s.MapFrom(s=>s.Doctors.Count))
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => baseUrl + "/uploads/departments/" + src.ImageName));

            CreateMap<Department, DepartmentGetDto>()
                 .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => baseUrl + "/uploads/departments/" + src.ImageName));

            CreateMap<Department, DepartmentGetDtoForUser>()
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => baseUrl + "/uploads/departments/" + src.ImageName));



            //services
            CreateMap<Medical.Core.Entities.Service, ServiceCreateDto>().ReverseMap();
            CreateMap<Medical.Core.Entities.Service, ServicePaginatedGetDto>()
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => baseUrl + "/uploads/services/" + src.ImageName));

            CreateMap<Medical.Core.Entities.Service, ServiceGetDto>()
                 .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => baseUrl + "/uploads/services/" + src.ImageName));


            CreateMap<Medical.Core.Entities.Service, ServiceGetDtoForUser>()
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => baseUrl + "/uploads/services/" + src.ImageName));




            //features
            CreateMap<Feature, FeatureCreateDto>().ReverseMap();
            CreateMap<Feature, FeaturePaginatedGetDto>()
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => baseUrl + "/uploads/features/" + src.ImageName));

            CreateMap<Feature, FeatureGetDto>()
                 .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => baseUrl + "/uploads/features/" + src.ImageName));


            CreateMap<Feature, FeatureGetDtoForUser>()
                 .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => baseUrl + "/uploads/features/" + src.ImageName));



            //feeds
            CreateMap<Feed, FeedCreateDto>().ReverseMap();
            CreateMap<Feed, FeedPaginatedGetDto>()
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => baseUrl + "/uploads/feeds/" + src.ImageName))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToString("MMMM dd, yyyy")));

            CreateMap<Feed, FeedGetDto>()
                 .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => baseUrl + "/uploads/feeds/" + src.ImageName))
               .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToString("MMMM dd, yyyy")));



            CreateMap<Feed, FeedGetDtoForUser>()
               .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => baseUrl + "/uploads/feeds/" + src.ImageName))
             .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToString("MMMM dd, yyyy")));



            //settings
            CreateMap<Setting, SettingGetDto>();
            CreateMap<Setting, SettingPaginatedGetDto>();


            //doctors

            CreateMap<Doctor, DoctorCreateDto>().ReverseMap();
            CreateMap<Doctor, DoctorPaginatedGetDto>()
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => baseUrl + "/uploads/doctors/" + src.ImageName));

            CreateMap<Doctor, DoctorGetDto>()
                 .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => baseUrl + "/uploads/doctors/" + src.ImageName));



            CreateMap<Doctor, DoctorGetDtoForUser>()
                 .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => baseUrl + "/uploads/doctors/" + src.ImageName));

            CreateMap<Doctor, DoctorGetDetailDto>()
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => baseUrl + "/uploads/doctors/" + src.ImageName));


            //medicines

            CreateMap<Medicine, MedicineCreateDto>()
              .ForMember(dest => dest.Files, opt => opt.Ignore());

            CreateMap<MedicineCreateDto, Medicine>();

            CreateMap<Medicine, MedicineDetailsDto>()
     .ForMember(dest => dest.MedicineImages,
                opt => opt.MapFrom(src => src.MedicineImages.Select(rc => new MedicineImageResponseDto
                {
                    Id = rc.Id,
                    Url = baseUrl + "/uploads/medicines/" + rc.ImageName
                }).ToList()));
    

            CreateMap<Medicine, MedicineGetDto>().ReverseMap();

            CreateMap<Medicine, MedicinePaginatedGetDto>();

            //Auth

            CreateMap<AppUser, AdminGetDto>();

            CreateMap<AppUser, AdminPaginatedGetDto>();
              

        }
    }
}

