using System;
using Medical.Service;
using Medical.Service.Dtos.Admin.CategoryDtos;
using Medical.Service.Dtos.User.CategoryDtos;
using Medical.Service.Dtos.User.FeatureDtos;
using Medical.Service.Implementations.Admin;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Medical.Api.Controllers
{
    [ApiController]
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

        [HttpGet("api/Categories/all")]
        public ActionResult<List<CategoryGetDtoForUser>> GetAllUser()
        {
            return StatusCode(200, _categoryService.GetAll());
        }

        [HttpGet("api/admin/categories/{id}")]
        public ActionResult<CategoryGetDto> GetById(int id)
        {
            return StatusCode(200, _categoryService.GetById(id));
        }

        [HttpPut("api/admin/Categories/{id}")]
        public IActionResult Update(int id, CategoryUpdateDto updateDto)
        {
            _categoryService.Update(id, updateDto);
            return NoContent();
        }

        [HttpDelete("api/admin/categories/{id}")]
        public IActionResult Delete(int id)
        {
            _categoryService.Delete(id);
            return NoContent();
        }

    }
}

