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

    public IActionResult Error()
    {
        return View();
    }


    [HttpGet("api/admin/yearly-count")]
    public async Task<IActionResult> GetYearlyAppointmentsCount()
    {
        try
        {
           
            var appointments = await _crudService.GetYearlyAppointmentsCountAsync();

           
            var labels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            var counts = new int[12]; 

               
            int totalAppointments = appointments.Count(); 
            int appointmentsPerMonth = totalAppointments / 12; 

            for (int i = 0; i < counts.Length; i++)
            {
                counts[i] = appointmentsPerMonth; 
            }

            var response = new
            {
                labels = labels,
                counts = counts
            };

            return Ok(response);
        }
        catch (System.Exception ex)
        {
            
            return StatusCode(500, "Internal server error");
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

    [HttpGet("api/admin/monthly-count")]
    public async Task<IActionResult> GetMonthlyAppointmentsCount()
    {
        try
        {
            var count = await _crudService.GetMonthlyAppointmentsCountAsync();
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

