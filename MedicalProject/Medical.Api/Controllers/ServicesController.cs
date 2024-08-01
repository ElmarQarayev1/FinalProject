using System;
using Medical.Service;
using Medical.Service.Dtos.Admin.ServiceDtos;
using Medical.Service.Dtos.User.FeatureDtos;
using Medical.Service.Dtos.User.ServiceDtos;
using Medical.Service.Implementations.Admin;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Medical.Api.Controllers
{
	[ApiController]
	public class ServicesController:ControllerBase
	{
		public IServiceService _serviceService;
		public ServicesController(IServiceService serviceService)
		{
			_serviceService = serviceService;
		}

        [HttpPost("api/admin/Services")]
        public ActionResult Create([FromForm] ServiceCreateDto createDto)
        {
            return StatusCode(201, new { Id = _serviceService.Create(createDto) });
        }

        [HttpGet("api/admin/Services")]
        public ActionResult<PaginatedList<ServicePaginatedGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _serviceService.GetAllByPage(search, page, size));
        }

        [HttpGet("api/admin/Services/all")]
        public ActionResult<List<ServiceGetDto>> GetAll()
        {
            return StatusCode(200, _serviceService.GetAll());
        }

        [HttpGet("api/Services")]
        public ActionResult<List<ServiceGetDtoForUser>> GetAllForUserHome()
        {
            return StatusCode(200, _serviceService.GetForUserHome());
        }

        [HttpGet("api/Services/all")]
        public ActionResult<List<ServiceGetDtoForUser>> GetAllUser()
        {
            return StatusCode(200, _serviceService.GetAll());
        }

        [HttpGet("api/admin/Services/{id}")]
        public ActionResult<ServiceGetDto> GetById(int id)
        {
            return StatusCode(200, _serviceService.GetById(id));
        }

        [HttpPut("api/admin/Services/{id}")]
        public void Update(int id, [FromForm] ServiceUpdateDto updateDto)
        {
            _serviceService.Update(id, updateDto);
        }

        [HttpDelete("api/admin/Services/{id}")]
        public IActionResult Delete(int id)
        {
            _serviceService.Delete(id);
            return NoContent();
        }
    }
}

