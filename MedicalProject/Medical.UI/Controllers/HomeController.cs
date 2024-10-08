﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Medical.UI.Models;
using Medical.UI.Filter;
using Medical.UI.Exception;
using Medical.UI.Service;

namespace Medical.UI.Controllers;

[ServiceFilter(typeof(AuthFilter))]
public class HomeController : Controller
{
    private HttpClient _client;
    private readonly ICrudService _crudService;

    public HomeController(ICrudService crudService)
    {
        _crudService = crudService;
        _client = new HttpClient();
    }

    public IActionResult Index()
    {
        return View();
    }
    public IActionResult IndexForUser()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }

    [HttpGet("api/admin/orders-price-per-year")]
    public async Task<IActionResult> GetOrdersPricePerYear()
    {
        try
        {
            var ordersPricePerYear = await _crudService.GetOrdersPricePerYearAsync();
            return Ok(ordersPricePerYear);
        }
        catch (HttpException ex)
        {
            if (ex.Status == System.Net.HttpStatusCode.Unauthorized)
            {
                return Unauthorized("You are not authorized to access this resource.");
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
        }
        catch (System.Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
        }
    }

    [HttpGet("api/admin/Reviews/PendingCount")]
    public async Task<IActionResult> GetPendingReviewCount()
    {
        try
        {
            var pendingReviewCount = await _crudService.GetPendingReviewCountAsync();
            return Ok(pendingReviewCount);
        }
        catch (HttpException ex)
        {
            if (ex.Status == System.Net.HttpStatusCode.Unauthorized)
            {
                return Unauthorized("You are not authorized to access this resource.");
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
        }
        catch (System.Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
        }
    }

    [HttpGet("api/admin/order-status-counts")]
    public async Task<IActionResult> OrderStatusCounts()
    {
        try
        {
            var statusCounts = await _crudService.GetOrderStatusCountsAsync();
            return Ok(statusCounts);
        }
        catch (HttpException ex)
        {
            if (ex.Status == System.Net.HttpStatusCode.Unauthorized)
            {
                return Unauthorized("You are not authorized to access this resource.");
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
        }
        catch (System.Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
        }
    }

    [HttpGet("api/admin/monthly-count")]
    public async Task<IActionResult> GetMonthlyAppointmentsCount()
    {
        try
        {
            var counts = await _crudService.GetMonthlyAppointmentsCountAsync();

            if (counts == null || counts.Months == null || counts.Appointments == null)
            {
               
                return BadRequest("Data is missing or in an unexpected format.");
            }

            return Ok(counts);
        }
        catch (HttpException ex)
        {
            if (ex.Status == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Auth");
            }

            return RedirectToAction("Error", "Home");
        }
        catch (System.Exception)
        {
            return RedirectToAction("Error", "Home");
        }
    }
  


    [HttpGet("api/admin/daily-count")]
    public async Task<IActionResult> GetDailyAppointmentsCount()
    {
        try
        {
            var count = await _crudService.GetDailyAppointmentsCountAsync();
            return Ok(count);
        }
        catch (HttpException ex)
        {
            if (ex.Status == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Auth");
            }

            return RedirectToAction("Error", "Home");
        }
        catch (System.Exception)
        {
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpGet("api/admin/yearly-count")]
    public async Task<IActionResult> GetYearlyAppointmentsCount()
    {
        try
        {
            var count = await _crudService.GetYearlyAppointmentsCountAsync();
            return Ok(count);
        }
        catch (HttpException ex)
        {
            if (ex.Status == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Auth");
            }

            return RedirectToAction("Error", "Home");
        }
        catch (System.Exception)
        {
            return RedirectToAction("Error", "Home");
        }
    }


    [HttpGet]
    public async Task<IActionResult> ExportToExcel()
    {
        try
        {
            var fileContent = await _crudService.ExportAllTablesAsync();
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Tables.xlsx");
        }
        catch (HttpException ex)
        {
            if (ex.Status == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Auth");
            }

            return RedirectToAction("Error", "Home");
        }
        catch (System.Exception)
        {
            return RedirectToAction("Error", "Home");
        }
    }
    [HttpGet]
    public async Task<IActionResult> GetTodayOrdersCount()
    {
        try
        {
            var count = await _crudService.GetTodayOrdersCountAsync();
            return Ok(count);
        }
        catch (HttpException ex)
        {
            if (ex.Status == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Auth");
            }

            return RedirectToAction("Error", "Home");
        }
        catch (System.Exception)
        {
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetTodayOrdersTotalPrice()
    {
        try
        {
            var totalPrice = await _crudService.GetTodayOrdersTotalPriceAsync();
            return Ok(totalPrice);
        }
        catch (HttpException ex)
        {
            if (ex.Status == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Auth");
            }

            return RedirectToAction("Error", "Home");
        }
        catch (System.Exception)
        {
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetMonthlyOrdersCount()
    {
        try
        {
            var count = await _crudService.GetMonthlyOrdersCountAsync();
            return Ok(count);
        }
        catch (HttpException ex)
        {
            if (ex.Status == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Auth");
            }

            return RedirectToAction("Error", "Home");
        }
        catch (System.Exception)
        {
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetMonthlyOrdersTotalPrice()
    {
        try
        {
            var totalPrice = await _crudService.GetMonthlyOrdersTotalPriceAsync();
            return Ok(totalPrice);
        }
        catch (HttpException ex)
        {
            if (ex.Status == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Auth");
            }

            return RedirectToAction("Error", "Home");
        }
        catch (System.Exception)
        {
            return RedirectToAction("Error", "Home");
        }
    }



}

