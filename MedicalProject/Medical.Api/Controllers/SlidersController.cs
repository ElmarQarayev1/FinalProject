using System;
using Medical.Service;
using Medical.Service.Dtos.Admin.SliderDtos;
using Medical.Service.Dtos.User.FeatureDtos;
using Medical.Service.Dtos.User.SliderDtos;
using Medical.Service.Implementations.Admin;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Medical.Api.Controllers
{
    [ApiController]
    public class SlidersController : ControllerBase
    {
        private readonly ISliderService _sliderService;
        public SlidersController(ISliderService sliderService)
        {

           _sliderService = sliderService;
        }


        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPost("api/admin/Sliders")]
        public ActionResult Create([FromForm] SliderCreateDto createDto)
        {
            return StatusCode(201, new { Id = _sliderService.Create(createDto) });
        }


        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("api/admin/Sliders")]
        public ActionResult<PaginatedList<SliderPaginatedGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _sliderService.GetAllByPage(search, page, size));
        }


        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("api/admin/Sliders/all")]
        public ActionResult<List<SliderGetDto>> GetAll()
        {
            return StatusCode(200, _sliderService.GetAll());
        }


        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/Sliders")]
        public ActionResult<List<SliderGetDtoForUser>> GetAllForUserHome()
        {
            return StatusCode(200, _sliderService.GetAllUser());
        }


        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("api/admin/Sliders/{id}")]
        public ActionResult<SliderGetDto> GetById(int id)
        {
            return StatusCode(200, _sliderService.GetById(id));
        }


        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPut("api/admin/Sliders/{id}")]
        public void Update(int id, [FromForm] SliderUpdateDto updateDto)
        {
            _sliderService.Update(id, updateDto);

        }


        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpDelete("api/admin/Sliders/{id}")]
        public IActionResult Delete(int id)
        {
            _sliderService.Delete(id);
            return NoContent();
        }


    }
}

