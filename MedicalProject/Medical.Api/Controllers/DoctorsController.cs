using System;
using Medical.Service;
using Medical.Service.Dtos.Admin.DoctorDtos;
using Medical.Service.Dtos.User.DoctorDtos;
using Medical.Service.Dtos.User.FeatureDtos;
using Medical.Service.Implementations.Admin;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Medical.Api.Controllers
{
    [ApiController]
	public class DoctorsController:ControllerBase
	{
		public IDoctorService _doctorService;

		public DoctorsController(IDoctorService doctorService)
		{
			_doctorService = doctorService;

		}

        [HttpPost("api/admin/Doctors")]
        public ActionResult Create([FromForm] DoctorCreateDto createDto)
        {
            return StatusCode(201, new { Id = _doctorService.Create(createDto) });
        }

        [HttpGet("api/admin/Doctors")]
        public ActionResult<PaginatedList<DoctorPaginatedGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _doctorService.GetAllByPage(search, page, size));
        }

        [HttpGet("api/admin/Doctors/all")]
        public ActionResult<List<DoctorGetDto>> GetAll()
        {
            return StatusCode(200, _doctorService.GetAll());
        }
        [HttpGet("api/Doctors")]
        public ActionResult<List<DoctorGetDtoForUser>> GetAllForUserHome()
        {
            return StatusCode(200, _doctorService.GetForUserHome());
        }

        [HttpGet("api/Doctors/all")]
        public ActionResult<List<DoctorGetDtoForUser>> GetAllUser()
        {
            return StatusCode(200, _doctorService.GetAll());
        }

        [HttpGet("api/admin/Doctors/{id}")]
        public ActionResult<DoctorGetDto> GetById(int id)
        {
            return StatusCode(200, _doctorService.GetById(id));
        }

        [HttpDelete("api/admin/Doctors/{id}")]
        public IActionResult Delete(int id)
        {
            _doctorService.Delete(id);
            return NoContent();
        }
        [HttpGet("api/Doctors/{id}")]
        public ActionResult<DoctorGetDetailDto> GetByIdForUser(int id)
        {
            return StatusCode(200, _doctorService.GetByIdForUser(id));
        }

        [HttpPut("api/admin/Doctors/{id}")]
        public void Update(int id, [FromForm] DoctorUpdateDto updateDto)
        {
            _doctorService.Update(id, updateDto);
        }

      

    }
}

