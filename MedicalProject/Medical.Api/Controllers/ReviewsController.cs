using System;
using Medical.Service.Dtos.User.MedicineDtos;
using Medical.Service.Implementations.Admin;
using System.Data;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Medical.Core.Enum;

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

        [HttpGet("api/admin/Reviews")]
        public IActionResult GetAllOrders(string? search = null, int page = 1, int size = 10)
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

        [Authorize(Roles = "Member")]
        [HttpPost("api/Reviews/CreateReview")]
        public ActionResult CreateReview(MedicineReviewItemDto createDto)
        {
            return StatusCode(201, new { Id = _reviewService.CreateReview(createDto) });
        }

        [Authorize(Roles = "Member")]
        [HttpDelete("api/Reviews/DeleteReview")]
        public IActionResult DeleteReview(MedicineReviewDeleteDto deleteDto)
        {
            _reviewService.DeleteReview(deleteDto);
            return NoContent();
        }

        [HttpGet("api/admin/reviews/{id}")]
        public IActionResult GetReviewById(int id)
        {
            return StatusCode(200, _reviewService.GetById(id));
        }


        [HttpPut("api/admin/reviewsAccepted/{id}")]
        public IActionResult AcceptReview(int id)
        {

            _reviewService.UpdateOrderStatus(id, ReviewStatus.Accepted);
            return NoContent();
        }      
        [HttpPut("api/admin/reviewsRejected/{id}")]
        public IActionResult RejectOrder(int id)
        {

            _reviewService.UpdateOrderStatus(id, ReviewStatus.Rejected);
            return NoContent();

        }

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

