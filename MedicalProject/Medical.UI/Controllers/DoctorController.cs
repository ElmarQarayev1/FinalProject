using System;
using System.Text.Json;
using Medical.UI.Exception;
using Medical.UI.Filter;
using Medical.UI.Models;
using Medical.UI.Service;
using Microsoft.AspNetCore.Mvc;

namespace Medical.UI.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class DoctorController:Controller
	{

        private HttpClient _client;
        private readonly ICrudService _crudService;

        public DoctorController(ICrudService crudService)
        {
            _crudService = crudService;
            _client = new HttpClient();
        }


        public async Task<IActionResult> Index(int page = 1, int size = 4)
        {
            try
            {
               
                var paginatedData = await _crudService.GetAllPaginated<DoctorListitemGetResponse>("doctors", page, size);

               
                if (page > paginatedData.TotalPages)
                {
                    return RedirectToAction("Index", new { page = paginatedData.TotalPages, size });
                }

              
                return View(paginatedData);
            }
            catch (HttpException e)
            {
               
                if (e.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Auth");
                }
                
                return RedirectToAction("Error", "Home");
            }
            catch (System.Exception e)
            {
               
                return RedirectToAction("Error", "Home");
            }
        }


        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _crudService.Delete("doctors/" + id);
                return Ok();
            }
            catch (HttpException e)
            {
                return StatusCode((int)e.Status);
            }
        }

        public async Task<IActionResult> Create()
        {


            ViewBag.Departments = await getDepartments();

            if (ViewBag.Departments == null) return RedirectToAction("error", "home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] DoctorCreateRequest createRequest)
        {
            try
            {
                await _crudService.CreateFromForm(createRequest, "doctors");
                return RedirectToAction("index");
            }
            catch (ModelException ex)
            {
                foreach (var item in ex.Error.Errors)
                    ModelState.AddModelError(item.Key, item.Message);

                ViewBag.Departments = await _crudService.Get<List<DepartmentListItemGetResponseForDoctor>>("departments/all");

                return View();
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _crudService.Get<DoctorGetResponse>("doctors/" + id);

            if (doctor == null)
            {
                return NotFound();
            }

            DoctorEditRequest doctorEdit = new DoctorEditRequest
            {
                FullName = doctor.FullName,
                FileUrl = doctor.FileUrl,
                 Address = doctor.Address,
                 CompassionSkil=doctor.CompassionSkil,
                 EthicSkil=doctor.EthicSkil,
                 ResilienceSkil=doctor.EthicSkil,
                 BehanceUrl=doctor.BehanceUrl,
                 Email=doctor.Email,
                 Desc=doctor.Desc,
                 InstagramUrl=doctor.InstagramUrl,
                 TwitterUrl=doctor.TwitterUrl,
                 VimeoUrl=doctor.VimeoUrl,
                 Phone=doctor.Phone,
                 Position=doctor.Position,
                DepartmentId = doctor.DepartmentId,
  
            };

            ViewBag.Departments = await _crudService.Get<List<DepartmentListItemGetResponseForDoctor>>("departments/all");
            
            return View(doctorEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [FromForm] DoctorEditRequest editRequest)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await _crudService.Get<List<DepartmentListItemGetResponseForDoctor>>("departments/all");
                return View(editRequest);
            }
            try
            {
                
                await _crudService.EditFromForm(editRequest, $"doctors/{id}");
                return RedirectToAction("Index");
            }
            catch (ModelException ex)
            {
                foreach (var item in ex.Error.Errors)
                    ModelState.AddModelError(item.Key, item.Message);

                ViewBag.Departments = await _crudService.Get<List<DepartmentListItemGetResponseForDoctor>>("departments/all");
                return View(editRequest);
            }
        }

        private async Task<List<DepartmentListItemGetResponseForDoctor>> getDepartments()
        {
            using (var response = await _client.GetAsync("https://localhost:7061/api/admin/Departments/all"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var data = JsonSerializer.Deserialize<List<DepartmentListItemGetResponseForDoctor>>(await response.Content.ReadAsStringAsync(), options);

                    return data;
                }
            }
            return null;
        }
    }
}

