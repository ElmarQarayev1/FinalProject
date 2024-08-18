using System;
using Medical.Service.Dtos.User.MedicineDtos;
using Medical.Service.Implementations.Admin;
using System.Data;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Medical.Core.Enum;
using System.Security.Claims;

namespace Medical.Api.Controllers
{
    [ApiController]
	public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReviewsController(IReviewService reviewService, IHttpContextAccessor httpContextAccessor)
        {
            _reviewService = reviewService;
            _httpContextAccessor = httpContextAccessor;
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("api/admin/Reviews")]
        public IActionResult GetAllRewiews(string? search = null, int page = 1, int size = 10)
        {
            try
            {
                var reviews = _reviewService.GetAllByPage(search, page, size);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error.");

            }
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [Authorize(Roles = "Member")]
        [HttpPost("api/Reviews/CreateReview")]
        public async Task<IActionResult> CreateReview([FromBody] MedicineReviewItemDto createDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var reviewId = await _reviewService.CreateReviewAsync(createDto, userId);

            return StatusCode(201, new { Id = reviewId });
        }
        [ApiExplorerSettings(GroupName = "user_v1")]
        [Authorize(Roles = "Member")]
        [HttpDelete("api/Reviews/DeleteReview")]
        public IActionResult DeleteReview(MedicineReviewDeleteDto deleteDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            _reviewService.DeleteReview(deleteDto,userId);
            return NoContent();
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("api/admin/reviews/{id}")]
        public IActionResult GetReviewById(int id)
        {
            return StatusCode(200, _reviewService.GetById(id));
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPut("api/admin/reviewsAccepted/{id}")]
        public IActionResult AcceptReview(int id)
        {

            _reviewService.UpdateReviewStatus(id, ReviewStatus.Accepted);
            return NoContent();
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPut("api/admin/reviewsRejected/{id}")]
        public IActionResult RejectOrder(int id)
        {

            _reviewService.UpdateReviewStatus(id, ReviewStatus.Rejected);
            return NoContent();

        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("api/admin/Reviews/PendingCount")]
        public IActionResult GetPendingReviewCount()
        {
            try
            {
                var pendingCount = _reviewService.GetPendingReviewCount();
                return Ok(pendingCount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error.");
            }
        }


    }
}

