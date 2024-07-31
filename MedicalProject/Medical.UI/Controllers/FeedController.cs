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
    public class FeedController:Controller
	{
        private HttpClient _client;
        private readonly ICrudService _crudService;

        public FeedController(ICrudService crudService)
        {
            _crudService = crudService;
            _client = new HttpClient();
        }

        public async Task<IActionResult> Index(int page = 1, int size = 4)
        {
            try
            {
                return View(await _crudService.GetAllPaginated<FeedListItemGetResponse>("feeds", page, size));
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
        public async Task<IActionResult> Create([FromForm] FeedCreateRequest createRequest)
        {
            try
            {
                await _crudService.CreateFromForm(createRequest, "feeds");
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
                await _crudService.Delete("feeds/" + id);
                return Ok();
            }
            catch (HttpException e)
            {
                return StatusCode((int)e.Status);
            }
        }


        public async Task<IActionResult> Edit(int id)
        {
            var feed = await _crudService.Get<FeedGetResponse>("feeds/" + id);

            if (feed == null)
            {
                return NotFound();
            }

            FeedEditRequest feedEdit = new FeedEditRequest
            {
                Name = feed.Name,
                Desc = feed.Desc,
                Date=feed.Date,
                
                FileUrl = feed.FileUrl,

            };

            return View(feedEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(FeedEditRequest editRequest, int id)
        {

            if (!ModelState.IsValid)
            {

                return View(editRequest);
            }
            try
            {

                await _crudService.EditFromForm(editRequest, $"feeds/{id}");

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

