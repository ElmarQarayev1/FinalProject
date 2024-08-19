using System;
using Medical.UI.Exception;
using System.Text.Json;
using Medical.UI.Filter;
using Medical.UI.Service;
using Microsoft.AspNetCore.Mvc;
using Medical.UI.Models;

namespace Medical.UI.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class MedicineController:Controller
	{
        private HttpClient _client;
        private readonly ICrudService _crudService;

        public MedicineController(ICrudService crudService)
        {
            _crudService = crudService;
            _client = new HttpClient();
        }

        public async Task<IActionResult> Index(int page = 1, int size = 4)
        {
            try
            {
                var paginatedResponse = await _crudService.GetAllPaginated<MedicineListItemGetResponse>("medicines", page, size);

             
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
                else
                {
                   
                    throw;
                }
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
                await _crudService.Delete("medicines/" + id);
                return Ok();
            }
            catch (HttpException e)
            {
                return StatusCode((int)e.Status);
            }
        }

        public async Task<IActionResult> Create()
        {


            ViewBag.Categories = await getCategories();

            if (ViewBag.Categories == null) return RedirectToAction("error", "home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MedicineCreateRequest createRequest)
        {
            try
            {
                await _crudService.CreateFromForm(createRequest, "medicines");
                return RedirectToAction("index");
            }
            catch (ModelException ex)
            {
                foreach (var item in ex.Error.Errors)
                    ModelState.AddModelError(item.Key, item.Message);


               ViewBag.Categories = await _crudService.Get<List<CategoryListItemGetResponse>>("categories/all");

                return View();
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var medicine = await _crudService.Get<MedicineGetResponse>("medicines/" + id);

            if (medicine == null)
            {
                return NotFound();
            }

            MedicineEditRequest medicineEdit = new MedicineEditRequest
            {
                Name = medicine.Name,
                Desc = medicine.Desc,
                Price = medicine.Price,
                
                CategoryId = medicine.CategoryId,

                FileUrls = medicine.MedicineImages
            };

            ViewBag.Categories = await _crudService.Get<List<CategoryListItemGetResponse>>("categories/all");
            ViewBag.Pictures = medicine.MedicineImages;


            return View(medicineEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id,[FromForm]MedicineEditRequest editRequest)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _crudService.Get<List<CategoryListItemGetResponse>>("categories/all");
                return View(editRequest);
            }
            try
            {
                MedicineEditRequest updateDto = new MedicineEditRequest
                {
                    Name = editRequest.Name,
                    Desc = editRequest.Desc,
                    Price = editRequest.Price,

                    CategoryId = editRequest.CategoryId,

                    Files = editRequest.Files ?? new List<IFormFile>(),
                    ExistPictureIds = editRequest.ExistPictureIds,
                };

                await _crudService.EditFromForm(updateDto, $"medicines/{id}");
                return RedirectToAction("Index");
            }
            catch (ModelException ex)
            {
                foreach (var item in ex.Error.Errors)
                    ModelState.AddModelError(item.Key, item.Message);

                ViewBag.Categories = await _crudService.Get<List<CategoryListItemGetResponse>>("categories/all");
                return View(editRequest);
            }
        }

        private async Task<List<MedicineListItemGetResponse>> getCategories()
        {
            using (var response = await _client.GetAsync("https://localhost:7061/api/admin/Categories/all"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var data = JsonSerializer.Deserialize<List<MedicineListItemGetResponse>>(await response.Content.ReadAsStringAsync(), options);

                    return data;
                }
            }
            return null;
        }

    }
}

