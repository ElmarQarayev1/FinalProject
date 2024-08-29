using System;
using Medical.Service;
using Medical.Service.Dtos.Admin.DepartmentDtos;
using Medical.Service.Dtos.User.DepartmentDtos;
using Medical.Service.Dtos.User.FeatureDtos;
using Medical.Service.Implementations.Admin;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
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



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("api/admin/Departments")]
        public ActionResult Create([FromForm] DepartmentCreateDto createDto)
        {
            return StatusCode(201, new { Id = _departmentService.Create(createDto) });
        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("api/admin/Departments")]
        public ActionResult<PaginatedList<DepartmentPaginatedGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _departmentService.GetAllByPage(search, page, size));
        }

        

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("api/admin/Departments/all")]
        public ActionResult<List<DepartmentGetDto>> GetAll()
        {
            return StatusCode(200, _departmentService.GetAll());
        }




        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/Departments")]
        public ActionResult<List<DepartmentGetDtoForUser>> GetAllForUserHome()
        {
            return StatusCode(200, _departmentService.GetForUserHome());
        }




        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/Departments/all")]
        public ActionResult<List<DepartmentGetDtoForUser>> GetAllUser()
        {
            return StatusCode(200, _departmentService.GetAll());
        }

   

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("api/admin/Departments/{id}")]
        public ActionResult<DepartmentGetDto> GetById(int id)
        {
            return StatusCode(200, _departmentService.GetById(id));
        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut("api/admin/Departments/{id}")]
        public void Update(int id, [FromForm] DepartmentUpdateDto updateDto)
        {
            _departmentService.Update(id, updateDto);
        }


        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete("api/admin/Departments/{id}")]
        public IActionResult Delete(int id)
        {
            _departmentService.Delete(id);
            return NoContent();
        }

    }
}

