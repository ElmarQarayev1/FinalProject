using System;
using Medical.Service;
using Medical.Service.Dtos.Admin.CategoryDtos;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Medical.Api.Controllers
{
	public class CategoriesController:ControllerBase
	{
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("api/admin/categories")]
        public ActionResult Create(CategoryCreateDto createDto)
        {
            return StatusCode(201, new { Id = _categoryService.Create(createDto) });
        }

        [HttpGet("api/admin/categories")]
        public ActionResult<PaginatedList<CategoryPaginatedGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _categoryService.GetAllByPage(search, page, size));
        }

        [HttpGet("api/admin/categories/all")]
        public ActionResult<List<CategoryGetDto>> GetAll()
        {
            return StatusCode(200, _categoryService.GetAll());
        }

        [HttpGet("api/admin/categories/{id}")]
        public ActionResult<CategoryGetDto> GetById(int id)
        {
            return StatusCode(200, _categoryService.GetById(id));
        }

        [HttpDelete("api/admin/categories/{id}")]
        public IActionResult Delete(int id)
        {
            _categoryService.Delete(id);
            return NoContent();
        }

    }
}

