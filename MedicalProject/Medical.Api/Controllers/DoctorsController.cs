using System;
using Medical.Service;
using Medical.Service.Dtos.Admin.DoctorDtos;
using Medical.Service.Dtos.User.DoctorDtos;
using Medical.Service.Dtos.User.FeatureDtos;
using Medical.Service.Exceptions;
using Medical.Service.Implementations.Admin;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Medical.Api.Controllers
{
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        public IDoctorService _doctorService;

        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromSeconds(10);

        public DoctorsController(IDoctorService doctorService, IMemoryCache cache)
        {
            _doctorService = doctorService;
            _cache = cache;

        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPost("api/admin/Doctors")]
        public ActionResult Create([FromForm] DoctorCreateDto createDto)
        {
          
            var newDoctorId = _doctorService.Create(createDto);

           
            _cache.Remove("Doctors_GetAll_Admin"); 
            _cache.Remove("Doctors_GetAllForUserHome"); 
            _cache.Remove("Doctors_GetAllUser"); 
            _cache.Remove("Doctors_GetAllUserForDownSide"); 

          
            _cache.Remove($"Doctor_GetById_{newDoctorId}");

            return StatusCode(201, new { Id = newDoctorId });
        }

       

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("api/admin/Doctors")]
        public ActionResult<PaginatedList<DoctorPaginatedGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
           

            var result = _doctorService.GetAllByPage(search, page, size);
            return Ok(result);
        }


        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("api/admin/Doctors/all")]
        public ActionResult<List<DoctorGetDto>> GetAll()
        {
            var cacheKey = "Doctors_GetAll_Admin";
            if (_cache.TryGetValue(cacheKey, out List<DoctorGetDto> cachedResult))
            {
                return Ok(cachedResult);
            }

            var result = _doctorService.GetAll();
            _cache.Set(cacheKey, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            });


            return Ok(result);
        }


        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/Doctors")]
        public ActionResult<List<DoctorGetDtoForUser>> GetAllForUserHome()
        {
            var cacheKey = "Doctors_GetAllForUserHome";
            if (_cache.TryGetValue(cacheKey, out List<DoctorGetDtoForUser> cachedResult))
            {
                return Ok(cachedResult);
            }

            var result = _doctorService.GetForUserHome();
            _cache.Set(cacheKey, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            });

            return Ok(result);
        }




        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/Doctors/all")]
        public ActionResult<List<DoctorGetDtoForUser>> GetAllUser()
        {
            var cacheKey = "Doctors_GetAllUser";
            if (_cache.TryGetValue(cacheKey, out List<DoctorGetDtoForUser> cachedResult))
            {
                return Ok(cachedResult);
            }

            var result = _doctorService.GetAll();
            _cache.Set(cacheKey, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            });

            return Ok(result);
        }


        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/Doctors/ForDownSide")]
        public async Task<ActionResult<List<DoctorForDownSideDto>>> GetAllUserForDownSide()
        {
           

            var cacheKey = "Doctors_GetAllUserForDownSide";
            if (_cache.TryGetValue(cacheKey, out List<DoctorForDownSideDto> cachedResult))
            {
                return Ok(cachedResult);
            }

            var result = _doctorService.GetAllUserForDownSide();
            _cache.Set(cacheKey, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            });

            await Task.Delay(TimeSpan.FromSeconds(3));

            return Ok(result);
        }


        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/ForAppointment/{departmentId}")]
        public IActionResult GetByIdForAppointment(int departmentId)
        {
            var cacheKey = $"Doctors_GetByIdForAppointment_{departmentId}";
            if (_cache.TryGetValue(cacheKey, out List<DoctorGetDtoForUser> cachedResult))
            {
                return Ok(cachedResult);
            }

            var doctors = _doctorService.GetByIdForAppointment(departmentId);
            _cache.Set(cacheKey, doctors, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            });

            return Ok(doctors);
        }



        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/Doctors/{id}")]
        public ActionResult<DoctorGetDetailDto> GetByIdForUser(int id)
        {
            var cacheKey = $"Doctors_GetByIdForUser_{id}";
            if (_cache.TryGetValue(cacheKey, out DoctorGetDetailDto cachedResult))
            {
                return Ok(cachedResult);
            }

            var result = _doctorService.GetByIdForUser(id);
            _cache.Set(cacheKey, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            });

            return Ok(result);
        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("api/admin/Doctors/{id}")]
        public ActionResult<DoctorGetDto> GetById(int id)
        {
            var cacheKey = $"Doctor_GetById_{id}";
            if (_cache.TryGetValue(cacheKey, out DoctorGetDto cachedResult))
            {
                return Ok(cachedResult);
            }

            var result = _doctorService.GetById(id);

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            };

            _cache.Set(cacheKey, result, cacheEntryOptions);

            return Ok(result);
        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpDelete("api/admin/Doctors/{id}")]
        public IActionResult Delete(int id)
        {
            _doctorService.Delete(id);


            _cache.Remove($"Doctor_GetById_{id}");


            _cache.Remove("Doctors_GetAll");

            return NoContent();
        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPut("api/admin/Doctors/{id}")]
        public void Update(int id, [FromForm] DoctorUpdateDto updateDto)
        {
            _doctorService.Update(id, updateDto);

           
            _cache.Remove($"Doctor_GetById_{id}");
            _cache.Remove($"Doctors_GetByIdForUser_{id}");
            _cache.Remove("Doctors_GetAll_Admin");
            _cache.Remove("Doctors_GetAllForUserHome");
            _cache.Remove("Doctors_GetAllUser");
            _cache.Remove("Doctors_GetAllUserForDownSide");
        }


    }


}