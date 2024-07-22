using System;
using Medical.Service;
using Medical.Service.Dtos.Admin.FeatureDtos;
using Medical.Service.Dtos.Admin.ServiceDtos;
using Medical.Service.Implementations.Admin;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Medical.Api.Controllers
{
    [ApiController]
    public class FeaturesController:ControllerBase
	{
        public IFeatureService _featureService;
        public FeaturesController(IFeatureService featureService)
        {
            _featureService = featureService;
        }
        [HttpPost("api/admin/Features")]
        public ActionResult Create([FromForm]  FeatureCreateDto createDto)
        {
            return StatusCode(201, new { Id = _featureService.Create(createDto) });
        }

        [HttpGet("api/admin/Features")]
        public ActionResult<PaginatedList<FeaturePaginatedGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _featureService.GetAllByPage(search, page, size));
        }

        [HttpGet("api/admin/Features/all")]
        public ActionResult<List<FeatureGetDto>> GetAll()
        {
            return StatusCode(200, _featureService.GetAll());
        }

        [HttpGet("api/admin/Features/{id}")]
        public ActionResult<FeatureGetDto> GetById(int id)
        {
            return StatusCode(200, _featureService.GetById(id));
        }

        [HttpPut("api/admin/Features/{id}")]
        public void Update(int id, [FromForm] FeatureUpdateDto updateDto)
        {
            _featureService.Update(id, updateDto);
        }

        [HttpDelete("api/admin/Features/{id}")]
        public IActionResult Delete(int id)
        {
            _featureService.Delete(id);
            return NoContent();
        }
    }
}

