﻿using System;
using Medical.UI.Exception;
using Medical.UI.Filter;
using Medical.UI.Models;
using Medical.UI.Service;
using Microsoft.AspNetCore.Mvc;

namespace Medical.UI.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class OrderController:Controller
	{

        private HttpClient _client;
        private readonly ICrudService _crudService;

        public OrderController(HttpClient httpClient, ICrudService crudService)
        {
            _client = httpClient;
            _crudService = crudService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
               
                var paginatedResponse = await _crudService.GetAllPaginated<OrderPaginatedGetResponse>("orders", page);

                if (page > paginatedResponse.TotalPages && paginatedResponse.TotalPages > 0)
                {
                    
                    return RedirectToAction("Index", new { page = paginatedResponse.TotalPages });
                }

             
                return View(paginatedResponse);
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
                var fileContent = await _crudService.ExportOrdersAsync();
                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders.xlsx");
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
        public async Task<IActionResult> Details(int id)
        {
            try
            {
              
                var orderDetails = await _crudService.Get<OrderGetResponse>($"orders/{id}");

                
                return View(orderDetails);
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


        [HttpPost]
        public async Task<IActionResult> Accept(int id)
        {
            try
            {
                await _crudService.Status($"ordersAccepted/{id}");
                return RedirectToAction("Index");
            }
            catch (HttpException e)
            {
                return StatusCode((int)e.Status);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                await _crudService.Status($"ordersRejected/{id}");
                return RedirectToAction("Index");
            }
            catch (HttpException e)
            {
                return StatusCode((int)e.Status);
            }
        }
    
    }
}



    


