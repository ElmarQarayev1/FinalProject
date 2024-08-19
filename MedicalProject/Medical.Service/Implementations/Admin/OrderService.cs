using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Medical.Core.Entities;
using Medical.Core.Enum;
using Medical.Data;
using Medical.Data.Repositories.Interfaces;
using Medical.Service.Dtos.Admin.OrderDtos;
using Medical.Service.Dtos.User.OrderDtos;
using Medical.Service.Exceptions;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medical.Service.Implementations.Admin
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMedicineRepository _medicineRepository;
        private readonly IBasketRepository _basketRepository;
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public OrderService(IOrderRepository orderRepository, IBasketRepository basketRepository, AppDbContext context,IMedicineRepository medicineRepository,EmailService emailService)
        {
            _orderRepository = orderRepository;
            _basketRepository = basketRepository;
            _context = context;
            _medicineRepository = medicineRepository;
            _emailService = emailService;
        }


        public async Task<OrdersPricePerYearDto> GetOrdersPricePerYearAsync()
        {
            var startYear = 2021;
            var currentYear = DateTime.Now.Year;
            var years = Enumerable.Range(startYear, currentYear - startYear + 1).Select(y => y.ToString()).ToList();

            var ordersPrice = new List<double>();

            foreach (var year in years)
            {
                var yearInt = int.Parse(year);

                var totalSales = await _context.Orders
                    .Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Year == yearInt)
                    .SelectMany(o => o.OrderItems)
                    .SumAsync(oi => oi.SalePrice * oi.Count);

                ordersPrice.Add(totalSales);
            }

            return new OrdersPricePerYearDto
            {
                Years = years,
                OrdersPrice = ordersPrice
            };
        }


        public async Task<OrderStatusCountsDto> GetOrderStatusCountsAsync()
        {
            var counts = await _context.Orders
                .Where(o => o.Status == OrderStatus.Accepted ||
                            o.Status == OrderStatus.Rejected ||
                            o.Status == OrderStatus.Pending)
                .GroupBy(o => o.Status)
                .Select(group => new
                {
                    Status = group.Key,
                    Count = group.Count()
                })
                .ToListAsync();

            return new OrderStatusCountsDto
            {
                AcceptedCount = counts.FirstOrDefault(c => c.Status == OrderStatus.Accepted)?.Count ?? 0,
                RejectedCount = counts.FirstOrDefault(c => c.Status == OrderStatus.Rejected)?.Count ?? 0,
                PendingCount = counts.FirstOrDefault(c => c.Status == OrderStatus.Pending)?.Count ?? 0
            };
        }

        public async Task<int> GetTodayOrdersCountAsync()
        {
            var today = DateTime.Today;
            return await _context.Orders.CountAsync(o => o.CreatedAt >= today
                && o.CreatedAt < today.AddDays(1)
                && o.Status!=OrderStatus.Canceled);
        }

        public async Task<int> GetMonthlyOrdersCountAsync()
        {
            var firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);

            return await _context.Orders.CountAsync(o => o.CreatedAt >= firstDayOfMonth
                && o.CreatedAt < firstDayOfNextMonth
                && o.Status != OrderStatus.Canceled);
        }

        public async Task<double> GetTodayOrdersTotalPriceAsync()
        {
            var today = DateTime.Today;

            var totalPrice = await _context.Orders
                .Where(o => o.CreatedAt >= today
                    && o.CreatedAt < today.AddDays(1)
                    && o.Status != OrderStatus.Accepted)
                .SelectMany(o => o.OrderItems)
                .SumAsync(oi => oi.SalePrice * oi.Count);

            return totalPrice;
        }

        public async Task<double> GetMonthlyOrdersTotalPriceAsync()
        {
            var firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);

            var totalPrice = await _context.Orders
                .Where(o => o.CreatedAt >= firstDayOfMonth
                    && o.CreatedAt < firstDayOfNextMonth
                    && o.Status != OrderStatus.Accepted)
                .SelectMany(o => o.OrderItems)
                .SumAsync(oi => oi.SalePrice * oi.Count);

            return totalPrice;
        }


        public int CheckOut(CheckOutDto createDto,string userId)
           {
            var user = _context.AppUsers.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "User not found");
            }

            var basketItems = _basketRepository.GetAll(b => b.AppUserId == userId)
                .Include(b => b.Medicine).Include(x => x.AppUser)
                .ToList();

            if (!basketItems.Any())
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Basket is empty");
            }

            foreach (var dtoItem in createDto.BasketItems)
            {
                var basketItem = basketItems.FirstOrDefault(b => b.MedicineId == dtoItem.MedicineId);
                if (basketItem == null)
                {
                    throw new RestException(StatusCodes.Status404NotFound, $"Basket item not found for medicine ID {dtoItem.MedicineId}");
                }

                if (basketItem.Count != dtoItem.Count)
                {
                    throw new RestException(StatusCodes.Status400BadRequest, $"Mismatched count for medicine ID {dtoItem.MedicineId}. Basket has {basketItem.Count}, but checkout DTO has {dtoItem.Count}");
                }
            }

            var order = new Order
            {
                AppUserId = userId,
                FullName = user.FullName,
                Email = user.Email,
                Address = createDto.Address,
                Phone = createDto.Phone,
                CreatedAt = DateTime.Now,
                Status = OrderStatus.Pending,
                OrderItems = basketItems.Select(item => new OrderItem
                {
                    MedicineId = item.MedicineId,
                    Count = item.Count,
                    SalePrice = item.Medicine.Price
                }).ToList()
            };

            _orderRepository.Add(order);
            _basketRepository.DeleteRange(basketItems);
            _orderRepository.Save();

            return order.Id;
        }

        public PaginatedList<OrderPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var query = _orderRepository.GetAll(o =>
                (search == null || o.FullName.Contains(search) || o.Email.Contains(search))
                && o.Status != OrderStatus.Canceled) 
                .Select(order => new OrderPaginatedGetDto
                {
                    Id = order.Id,
                    FullName = order.FullName,
                    Email = order.Email,
                    Phone = order.Phone,
                    CreatedAt = order.CreatedAt,
                    TotalPrice = order.OrderItems.Sum(oi => oi.SalePrice * oi.Count),
                    Status = order.Status.ToString()
                });

            return PaginatedList<OrderPaginatedGetDto>.Create(query, page, size);
        }

        public List<OrderGetDto> GetAll(string? search = null)
        {
            var orders = _orderRepository.GetAll(o => search == null || o.FullName.Contains(search) || o.Email.Contains(search))
                .Select(order => new OrderGetDto
                {
                    Id = order.Id,
                    FullName = order.FullName,
                    Email = order.Email,
                    Phone = order.Phone,
                    CreatedAt = order.CreatedAt,
                    TotalPrice = order.OrderItems.Sum(oi => oi.SalePrice * oi.Count),
                    OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                    {
                        MedicineName = oi.Medicine != null ? oi.Medicine.Name : "Unknown",
                        Count = oi.Count,
                        Price = oi.SalePrice
                    }).ToList()
                })
                .ToList();

            return orders;
        }

        public OrderGetDto GetById(int id)
        {
            var order = _orderRepository.Get(o => o.Id == id, "OrderItems.Medicine");

            if (order == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Order not found");
            }

            return new OrderGetDto
            {
                Id = order.Id,
                FullName = order.FullName,
                Email = order.Email,
                Phone = order.Phone,
                CreatedAt = order.CreatedAt,
                TotalPrice = order.OrderItems.Sum(oi => oi.SalePrice * oi.Count),
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    MedicineName = oi.Medicine != null ? oi.Medicine.Name : "Unknown",
                    Count = oi.Count,
                    Price = oi.SalePrice
                }).ToList()
            };
        }
       

        public List<OrderDetailDto> GetDetailsOrder(string? search = null)
        {
            var query = _orderRepository.GetAll(o => search == null || o.FullName.Contains(search) || o.Email.Contains(search),"Medicines")
                .Select(order => new OrderDetailDto
                {
                    Id = order.Id,
                    FullName = order.FullName,
                    Email = order.Email,
                    Phone = order.Phone,
                    CreatedAt = order.CreatedAt,
                    TotalPrice = order.OrderItems.Sum(oi => oi.SalePrice * oi.Count),
                    TotalItemCount = order.OrderItems.Sum(oi => oi.Count),
                    OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                    {
                        MedicineName = oi.Medicine != null ? oi.Medicine.Name : "Unknown",
                        Count = oi.Count,
                        Price = oi.SalePrice
                    }).ToList(),
                    Status = order.Status.ToString()
                }).ToList();

            return query;
        }

        public void UpdateOrderStatus(int id, OrderStatus newStatus)
        {
            var order = _orderRepository.Get(o => o.Id == id);

            if (order == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Order not found");
            }

            if (order.Status == newStatus)
            {
                throw new RestException(StatusCodes.Status400BadRequest, $"Order is already {newStatus}");
            }

            order.Status = newStatus;

            var subject = "Order Status Update";
            var recipientEmail = order.AppUser?.Email ?? order.Email;

            var bodyTemplate = @"
    <!DOCTYPE html>
    <html lang='en'>
    <head>
        <meta charset='UTF-8'>
        <title>Order Status Update</title>
        <style>
            body {
                font-family: Arial, sans-serif;
                background-color: #f4f4f4;
                margin: 0;
                padding: 0;
            }
            .container {
                background-color: #ffffff;
                padding: 20px;
                margin: 20px;
                border-radius: 5px;
                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            }
            .header {
                background-color: #4CAF50;
                color: white;
                padding: 10px 0;
                text-align: center;
                border-radius: 5px 5px 0 0;
            }
            .content {
                margin: 20px 0;
            }
            .footer {
                text-align: center;
                color: #888;
                font-size: 12px;
                margin-top: 20px;
            }
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'>
                <h1>Order Status Update</h1>
            </div>
            <div class='content'>
                <p>Dear {{UserName}},</p>
                <p>Your order status has been updated to <strong>{{OrderStatus}}</strong>.</p>
                <p>Thank you for shopping with us.</p>
            </div>
            <div class='footer'>
                <p>&copy; 2024 Medical Project. All rights reserved.</p>
            </div>
        </div>
    </body>
    </html>";

            var body = bodyTemplate
                .Replace("{{UserName}}", order.AppUser?.FullName ?? "Customer")
                .Replace("{{OrderStatus}}", newStatus.ToString());

            _emailService.Send(recipientEmail, subject, body);
            _context.Orders.Update(order);
            _context.SaveChanges();
        }


    }
}
