using System;
using Medical.Service;
using Medical.Service.Dtos.Admin.DoctorDtos;
using Medical.Service.Dtos.User.DoctorDtos;
using Medical.Service.Dtos.User.FeatureDtos;
using Medical.Service.Exceptions;
using Medical.Service.Implementations.Admin;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("api/admin/Doctors")]
        public ActionResult Create([FromForm] DoctorCreateDto createDto)
        {
          
            var newDoctorId = _doctorService.Create(createDto);
     
          
            return StatusCode(201, new { Id = newDoctorId });
        }

       

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("api/admin/Doctors")]
        public ActionResult<PaginatedList<DoctorPaginatedGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
           

            var result = _doctorService.GetAllByPage(search, page, size);
            return Ok(result);
        }

      
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("api/admin/Doctors/all")]
        public ActionResult<List<DoctorGetDto>> GetAll()
        {
          
            var result = _doctorService.GetAll();
           

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
           
            var result = _doctorService.GetByIdForUser(id);
           

            return Ok(result);
        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("api/admin/Doctors/{id}")]
        public ActionResult<DoctorGetDto> GetById(int id)
        {
          

            var result = _doctorService.GetById(id);           

            return Ok(result);
        }


        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete("api/admin/Doctors/{id}")]
        public IActionResult Delete(int id)
        {
            _doctorService.Delete(id);

            return NoContent();
        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut("api/admin/Doctors/{id}")]
        public void Update(int id, [FromForm] DoctorUpdateDto updateDto)
        {
            _doctorService.Update(id, updateDto);

             
        }

    }


}