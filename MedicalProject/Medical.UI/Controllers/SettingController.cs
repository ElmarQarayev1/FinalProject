using System;
using Medical.UI.Exception;
using Medical.UI.Filter;
using Medical.UI.Models;
using Medical.UI.Service;
using Microsoft.AspNetCore.Mvc;

namespace Medical.UI.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class SettingController : Controller
    {
        private HttpClient _client;
        private readonly ICrudService _crudService;

        public SettingController(ICrudService crudService)
        {
            _crudService = crudService;
            _client = new HttpClient();
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                var paginatedResponse = await _crudService.GetAllPaginated<SettingListItemDetailedGetResponse>("settings", page);

              
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


        public async Task<IActionResult> Delete(string key)
        {
            try
            {
                await _crudService.Delete("settings/" + key);
                return Ok();
            }
            catch (HttpException e)
            {
                return StatusCode((int)e.Status);
            }
        }

        public async Task<IActionResult> Edit(string key)
        {
            var setting = await _crudService.Get<SettingEditRequest>("settings/" + key);

            if (setting == null)
            {
                return NotFound();
            }

            ViewBag.Key = key;

            return View(setting);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(SettingEditRequest editRequest, string key)
        {
            if (!ModelState.IsValid) return View(editRequest);
            try
            {
                await _crudService.Update<SettingEditRequest>(editRequest, "settings/" + key);
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

    }
}

