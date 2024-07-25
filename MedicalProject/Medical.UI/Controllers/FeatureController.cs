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
    public class FeatureController:Controller
	{
        private HttpClient _client;
        private readonly ICrudService _crudService;

        public FeatureController(ICrudService crudService)
        {
            _crudService = crudService;
            _client = new HttpClient();
        }

        public async Task<IActionResult> Index(int page = 1, int size = 4)
        {
            try
            {
                return View(await _crudService.GetAllPaginated<FeatureListItemGetResponse>("features", page, size));
            }
            catch (HttpException e)
            {
                if (e.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("login", "auth");
                }
                else
                {
                    throw;
                }
            }
            catch (System.Exception e)
            {
                throw;
            }
        }

        public async Task<IActionResult> Create()
        {
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, Request.Cookies["token"]);


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] FeatureCreateRequest createRequest)
        {
            try
            {
                await _crudService.CreateFromForm(createRequest, "features");
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
                await _crudService.Delete("features/" + id);
                return Ok();
            }
            catch (HttpException e)
            {
                return StatusCode((int)e.Status);
            }
        }


        public async Task<IActionResult> Edit(int id)
        {
            var feature = await _crudService.Get<FeatureGetResponse>("features/" + id);

            if (feature == null)
            {
                return NotFound();
            }

            FeatureEditRequest featureEdit = new FeatureEditRequest
            {
                Name = feature.Name,
                Desc=feature.Desc,
                FileUrl = feature.FileUrl,

            };

            return View(featureEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(FeatureEditRequest editRequest, int id)
        {

            if (!ModelState.IsValid)
            {

                return View(editRequest);
            }
            try
            {

                await _crudService.EditFromForm(editRequest, $"features/{id}");

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

