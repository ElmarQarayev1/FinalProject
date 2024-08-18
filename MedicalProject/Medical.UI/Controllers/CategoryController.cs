using System;
using Medical.UI.Exception;
using Medical.UI.Filter;
using Medical.UI.Models;
using Medical.UI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medical.UI.Controllers
{
  
    [ServiceFilter(typeof(AuthFilter))]
	public class CategoryController:Controller
	{

		private HttpClient _client;
		private readonly ICrudService _crudService;

		public CategoryController(HttpClient httpClient,ICrudService crudService)
		{
			_client = httpClient;
			_crudService = crudService;
		}

		public async  Task<IActionResult> Index(int page=1)
		{
			try
			{
				var paginatedResponse = await _crudService.GetAllPaginated<CategoryListItemDetailedGetResponse>("categories", page);

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

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _crudService.Delete("categories/" + id);
                return Ok();
            }
            catch (HttpException e)
            {
                return StatusCode((int)e.Status);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _crudService.Get<CategoryEditRequest>("categories/" + id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryEditRequest editRequest, int id)
        {
            if (!ModelState.IsValid) return View(editRequest);

            try
            {
                await _crudService.Update<CategoryEditRequest>(editRequest, "categories/" + id);
                return RedirectToAction("index");
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
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateRequest createRequest)
        {
            if (!ModelState.IsValid)
                return View(createRequest);

            try
            {
                await _crudService.Create(createRequest, "categories");
                return RedirectToAction("Index");
            }
            catch (ModelException e)
            {
                foreach (var item in e.Error.Errors)
                    ModelState.AddModelError(item.Key, item.Message);
                return View(createRequest);
            }
        }

    }
}

