using System;
using Medical.Service;
using Medical.Service.Dtos.Admin.DepartmentDtos;
using Medical.Service.Implementations.Admin;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Medical.Api.Controllers
{
    [ApiController]
    public class DepartmentsController:ControllerBase
	{
		public IDepartmentService _departmentService;
		public DepartmentsController(IDepartmentService departmentService)
		{
			_departmentService = departmentService;
		}


        [HttpPost("api/admin/Departments")]
        public ActionResult Create([FromForm] DepartmentCreateDto createDto)
        {
            return StatusCode(201, new { Id = _departmentService.Create(createDto) });
        }

        [HttpGet("api/admin/Departments")]
        public ActionResult<PaginatedList<DepartmentPaginatedGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _departmentService.GetAllByPage(search, page, size));
        }

        [HttpGet("api/admin/Departments/all")]
        public ActionResult<List<DepartmentGetDto>> GetAll()
        {
            return StatusCode(200, _departmentService.GetAll());
        }

        [HttpGet("api/admin/Departments/{id}")]
        public ActionResult<DepartmentGetDto> GetById(int id)
        {
            return StatusCode(200, _departmentService.GetById(id));
        }

        [HttpPut("api/admin/Departments/{id}")]
        public void Update(int id, [FromForm] DepartmentUpdateDto updateDto)
        {
            _departmentService.Update(id, updateDto);
        }

        [HttpDelete("api/admin/Departments/{id}")]
        public IActionResult Delete(int id)
        {
            _departmentService.Delete(id);
            return NoContent();
        }
    }
}

