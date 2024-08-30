using System;
using Medical.Service;
using Medical.Service.Dtos.Admin.FeatureDtos;
using Medical.Service.Dtos.Admin.FeedDtos;
using Medical.Service.Dtos.User.FeedDtos;
using Medical.Service.Implementations.Admin;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medical.Api.Controllers
{
	public class FeedsController:ControllerBase
	{
        public IFeedService _feedService;

        public FeedsController(IFeedService feedService)
        {
            _feedService = feedService;
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("api/admin/Feeds")]
        public ActionResult Create([FromForm] FeedCreateDto createDto)
        {
            return StatusCode(201, new { Id = _feedService.Create(createDto) });
        }

       
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("api/admin/Feeds")]
        public ActionResult<PaginatedList<FeedPaginatedGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _feedService.GetAllByPage(search, page, size));
        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("api/admin/Feeds/all")]
        public ActionResult<List<FeedGetDto>> GetAll()
        {
            return StatusCode(200, _feedService.GetAll());
        }



        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/Feeds")]
        public ActionResult<List<FeedGetDtoForUser>> GetAllUser()
        {
            return StatusCode(200, _feedService.GetAllUser());
        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("api/admin/Feeds/{id}")]
        public ActionResult<FeedGetDto> GetById(int id)
        {
            return StatusCode(200, _feedService.GetById(id));
        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut("api/admin/Feeds/{id}")]
        public void Update(int id, [FromForm] FeedUpdateDto updateDto)
        {
            _feedService.Update(id, updateDto);
        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete("api/admin/Feeds/{id}")]
        public IActionResult Delete(int id)
        {
            _feedService.Delete(id);
            return NoContent();
        }
    }
}

