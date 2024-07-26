using System;
using Medical.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Medical.UI.Exception;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Medical.UI.Service;

namespace Medical.UI.Controllers
{
    public class AccountController : Controller
    {
        private HttpClient _client;
        private readonly ICrudService _crudService;
        public AccountController(ICrudService service)
        {
            _client = new HttpClient();
            _crudService = service;
        }
        public IActionResult Login()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Login(LoginRequest loginRequest)
        //{
        //    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        //    var content = new StringContent(JsonSerializer.Serialize(loginRequest, options), System.Text.Encoding.UTF8, "application/json");
        //    using (var response = await _client.PostAsync("https://localhost:7061/api/admin/login", content))
        //    {
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(await response.Content.ReadAsStringAsync(), options);
        //            Response.Cookies.Append("token", "Bearer " + loginResponse.Token);
        //            return RedirectToAction("index", "home");
        //        }
        //        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        //        {
        //            ModelState.AddModelError("", "UserName or Password incorrect!");
        //            return View();
        //        }
        //        else
        //        {
        //            TempData["Error"] = "Something went wrong";
        //        }
        //    }

        //    return View();
        //}
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            // Configure JSON serializer options
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // Create the content to send in the POST request
            var content = new StringContent(
                JsonSerializer.Serialize(loginRequest, options),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            // Make the POST request to the login API
            using (var response = await _client.PostAsync("https://localhost:7061/api/admin/login", content))
            {
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response content
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(
                        await response.Content.ReadAsStringAsync(),
                        options
                    );

                    // Set the authentication token in a cookie
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true, // Helps mitigate XSS attacks
                        Secure = true,   // Ensures the cookie is only sent over HTTPS
                        SameSite = SameSiteMode.Strict, // Helps prevent CSRF attacks
                        Expires = DateTime.UtcNow.AddMinutes(60) // Adjust the expiration as needed
                    };
                    Response.Cookies.Append("token", "Bearer " + loginResponse.Token, cookieOptions);

                    // Redirect to the home page after successful login
                    return RedirectToAction("Index", "Home");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Handle unauthorized login attempt
                    ModelState.AddModelError("", "UserName or Password incorrect!");
                    return View();
                }
                else
                {
                    // Handle other errors
                    TempData["Error"] = "Something went wrong";
                }
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdminCreateByS(AdminCreateRequest createRequest)
        {
            if (!ModelState.IsValid)
                return View(createRequest);

            try
            {
                await _crudService.CreateForAdmins(createRequest, "createAdmin");
                return RedirectToAction("ShowAdmin");
            }
            catch (ModelException e)
            {
                foreach (var item in e.Error.Errors)
                    ModelState.AddModelError(item.Key, item.Message);
                return View(createRequest);
            }
        }

       // [Authorize(Roles ="superadmin")]
        public async Task<IActionResult> ShowAdmin(int page = 1)
        {
            try
            {
                var paginatedResponse = await _crudService.GetAllPaginated<AdminPaginatedGetResponse>("adminAllByPage", page);

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

       
        public IActionResult AdminCreateByS()
        {
            return View();
        }


        public async Task<IActionResult> Profile(string id)
        {
            var user = await _crudService.Get<AdminGetResponse>("getById/" + id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AdminProfileEditRequest editRequest, string id)
        {
            if (!ModelState.IsValid) return View(editRequest);

            try
            {
                await _crudService.Update<AdminProfileEditRequest>(editRequest, "update/" + id);
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

