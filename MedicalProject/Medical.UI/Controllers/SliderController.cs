using System;
using Medical.UI.Exception;
using Medical.UI.Filter;
using Medical.UI.Models;
using Medical.UI.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Medical.UI.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class SliderController:Controller
	{
        private HttpClient _client;
        private readonly ICrudService _crudService;

        public SliderController(ICrudService crudService)
        {
            _crudService = crudService;
            _client = new HttpClient();
        }

        public async Task<IActionResult> Index(int page = 1, int size = 4)
        {
            try
            {
                
                var paginatedResponse = await _crudService.GetAllPaginated<SliderListItemGetResponse>("sliders", page, size);

               
                if (page > paginatedResponse.TotalPages && paginatedResponse.TotalPages > 0)
                {
                    
                    return RedirectToAction("Index", new { page = paginatedResponse.TotalPages, size });
                }

                return View(paginatedResponse);
            }
            catch (HttpException e)
            {
                
                if (e.Status == System.Net.HttpStatusCode.Unauthorized)
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

        public async Task<IActionResult> Create()
        {
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, Request.Cookies["token"]);


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] SliderCreateRequest createRequest)
        {
            try
            {
                await _crudService.CreateFromForm(createRequest, "sliders");
                return RedirectToAction("index");
            }
            catch (ModelException ex)
            {
                foreach (var item in ex.Error.Errors)
                    ModelState.AddModelError(item.Key, item.Message);

                return View();
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _crudService.Delete("sliders/" + id);
                return Ok();
            }
            catch (HttpException e)
            {
                return StatusCode((int)e.Status);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var slider = await _crudService.Get<SliderGetResponse>("sliders/" + id);

            if (slider == null)
            {
                return NotFound();
            }

            SliderEditRequest sliderEdit = new SliderEditRequest
            {
                MainTitle = slider.MainTitle,
                SubTitle1 = slider.SubTitle1,
                SubTitle2=slider.SubTitle2,
                FileUrl = slider.FileUrl,
                Order = slider.Order
            };

            return View(sliderEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit( SliderEditRequest editRequest, int id)        {

            if (!ModelState.IsValid)
            {

                return View(editRequest);
            }
            try
            {

                await _crudService.EditFromForm(editRequest, $"sliders/{id}");

                return RedirectToAction("Index");
            }

            catch (ModelException e)
            {

                foreach (var item in e.Error.Errors)
                {
                    ModelState.AddModelError(item.Key, item.Message);
                }

                return View(editRequest);
            }
        }

    }

}

