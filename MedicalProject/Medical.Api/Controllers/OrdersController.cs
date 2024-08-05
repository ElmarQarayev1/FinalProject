using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Medical.Service.Dtos.User.OrderDtos;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Medical.Service.Exceptions;
using System.Collections.Generic;
using Medical.Core.Enum;

namespace Medical.Api.Controllers
{
    [ApiController]
    
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrdersController(IOrderService orderService, IHttpContextAccessor httpContextAccessor)
        {
            _orderService = orderService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("api/orders/checkout")]
        public IActionResult Checkout([FromBody] CheckOutDto checkoutDto)
        {
            try
            {
               
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized("User not authenticated.");
                }

                
                checkoutDto.AppUserId = userId;

               
                var orderId = _orderService.CheckOut(checkoutDto);

               
                return Ok(new { OrderId = orderId });
            }
            catch (RestException ex)
            {
                return StatusCode(ex.Code, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error.");
            }
        }


        [HttpGet("api/admin/orders/{id}")]
        public IActionResult GetOrderById(int id)
        {
            return StatusCode(200, _orderService.GetById(id));
        }

       // [Authorize(Roles ="Member")]
        [HttpGet("api/orders/{AppUserId}")]
        public IActionResult GetOrderByIdForUserProfile(string AppUserId)
        {
            var orders = _orderService.GetByIdForUserProfile(AppUserId);
            return Ok(orders);
        }
        [HttpGet("api/admin/orders")]
        public IActionResult GetAllOrders(string? search = null, int page = 1, int size = 10)
        {
            try
            {
                var orders = _orderService.GetAllByPage(search, page, size);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error.");

            }
        }

        [HttpGet("api/ordersDetails")]
        public IActionResult GetAllOrdersDetails(string? search = null)
        {
            try
            {
                var orders = _orderService.GetDetailsOrder(search);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPut("api/ordersCancelled/{id}")]
        public IActionResult CancelOrder(int id)
        {


            _orderService.UpdateOrderStatus(id, OrderStatus.Canceled);
            return NoContent(); 
            
          
        }

        [HttpPut("api/admin/ordersAccepted/{id}")]
        public IActionResult AcceptOrder(int id)
        {
           
            
                _orderService.UpdateOrderStatus(id, OrderStatus.Accepted);
                return NoContent(); 
            
           
        }

        [HttpPut("api/admin/ordersRejected/{id}")]
        public IActionResult RejectOrder(int id)
        {
            
           
                _orderService.UpdateOrderStatus(id, OrderStatus.Rejected);
                return NoContent(); 
            
                     
            
        }



    }

}
 