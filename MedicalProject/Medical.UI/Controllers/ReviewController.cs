using System;
using Medical.UI.Exception;
using Medical.UI.Filter;
using Medical.UI.Models;
using Medical.UI.Service;
using Microsoft.AspNetCore.Mvc;

namespace Medical.UI.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class ReviewController:Controller
	{
        private HttpClient _client;
        private readonly ICrudService _crudService;

        public ReviewController(HttpClient httpClient, ICrudService crudService)
        {
            _client = httpClient;
            _crudService = crudService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                var paginatedResponse = await _crudService.GetAllPaginated<ReviewPaginatedGetResponse>("reviews", page);

              
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
        public async Task<IActionResult> Details(int id)
        {
            try
            {

                var reviewDetails = await _crudService.Get<ReviewDetailsDto>($"reviews/{id}");


                return View(reviewDetails);
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
                await _crudService.Status($"reviewsAccepted/{id}");
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
                await _crudService.Status($"reviewsRejected/{id}");
                return RedirectToAction("Index");
            }
            catch (HttpException e)
            {
                return StatusCode((int)e.Status);
            }
        }
    }
}

