﻿using System;
using Medical.Core.Entities;
using Medical.Core.Enum;
using Medical.Service.Dtos.Admin.DoctorDtos;
using Medical.Service.Dtos.Admin.OrderDtos;
using Medical.Service.Dtos.User.OrderDtos;

namespace Medical.Service.Interfaces.Admin
{
	public interface IOrderService
	{
        int CheckOut(CheckOutDto createDto,string userId);
        PaginatedList<OrderPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        List<OrderGetDto> GetAll(string? search = null);
        OrderGetDto GetById(int id);
        List<OrderDetailDto> GetDetailsOrder(string? search = null);
        void UpdateOrderStatus(int id, OrderStatus newStatus);
      
        Task<int> GetTodayOrdersCountAsync();
        Task<double> GetTodayOrdersTotalPriceAsync();
        Task<int> GetMonthlyOrdersCountAsync();
        Task<double> GetMonthlyOrdersTotalPriceAsync();
        Task<OrderStatusCountsDto> GetOrderStatusCountsAsync();
        Task<OrdersPricePerYearDto> GetOrdersPricePerYearAsync();

    }
}

