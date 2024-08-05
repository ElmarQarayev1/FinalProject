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
    public class AppointmentController:Controller
	{
        private HttpClient _client;
        private readonly ICrudService _crudService;

        public AppointmentController(HttpClient httpClient, ICrudService crudService)
        {
            _client = httpClient;
            _crudService = crudService;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                var paginatedResponse = await _crudService.GetAllPaginated<AppointmentListItemGetDto>("appointments", page);

                ViewBag.Doctors = await getDoctors();

                if (ViewBag.Doctors == null) return RedirectToAction("error", "home");
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

        [HttpPost]
        public async Task<IActionResult> FilterByDoctor(int doctorId, int page = 1)
        {
            try
            {
                var paginatedResponse = await _crudService.GetAllPaginatedForAppointment<AppointmentListItemGetDto>("appointments", page, size: 10, doctorId: doctorId);

                ViewBag.Doctors = await getDoctors();

                if (ViewBag.Doctors == null) return RedirectToAction("error", "home");
                return View("Index", paginatedResponse);
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
        private async Task<List<DoctorGetResponseForAppointmentResponse>> getDoctors()
        {
            using (var response = await _client.GetAsync("https://localhost:7061/api/admin/Doctors/all"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var data = JsonSerializer.Deserialize<List<DoctorGetResponseForAppointmentResponse>>(await response.Content.ReadAsStringAsync(), options);

                    return data;
                }
            }
            return null;
        }



    }
}

