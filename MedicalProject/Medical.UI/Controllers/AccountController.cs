using Medical.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Medical.UI.Exception;
using Medical.UI.Service;
using Medical.UI.Extentions;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Medical.UI.Controllers
{
    public class AccountController : Controller
    {
        private HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICrudService _crudService;
        private readonly IConfiguration _configuration;

        public AccountController(
            ICrudService service,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _client = new HttpClient();
            _crudService = service;
            _configuration = configuration;
        }

        public IActionResult GoogleLogin()
        {
            return View();
        }   

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            var content = new StringContent(JsonSerializer.Serialize(loginRequest, options), System.Text.Encoding.UTF8, "application/json");
            using (var response = await _client.PostAsync("https://localhost:7061/api/admin/login", content))
            {
                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(await response.Content.ReadAsStringAsync(), options);
                   

                    if (loginResponse.Token.PasswordResetRequired)
                    {
                        TempData["ResetUserName"] = loginRequest.UserName;
                        Response.Cookies.Append("token", "Bearer " + loginResponse.Token.Token);
                       
                        _httpContextAccessor.HttpContext.Session.SetBool("PasswordResetRequired", true);

                        return RedirectToAction("ResetPassword");
                    }
                    Response.Cookies.Append("token", "Bearer " + loginResponse.Token.Token);
                    _httpContextAccessor.HttpContext.Session.SetBool("PasswordResetRequired", false);
                    return RedirectToAction("index", "home");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError("", "UserName or Password incorrect!");
                    return View();
                }
                else
                {
                    TempData["Error"] = "Something went wrong";
                }
            }
           
            return View();
        }

        public IActionResult ResetPassword()
        {
           

            var userName = TempData["ResetUserName"] as string;
           

            if (userName == null || !_httpContextAccessor.HttpContext.Session.GetBool("PasswordResetRequired"))
            {
                return RedirectToAction("Login");
            }    


            if (userName == null)
            {
                return RedirectToAction("Login");
            }


            var model = new ResetPasswordModel
            {
                UserName = userName,
              
                  
                
            };

                  
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {   
                return View(model);
            }
                 
            try
            {

               
                await _crudService.Update<ResetPasswordModel>(model, "updatePassword");

               

                return RedirectToAction("Login");
            }
            catch (ModelException e)
            {
                foreach (var item in e.Error.Errors)
                {
                   
                    ModelState.AddModelError(item.Key, item.Message);
                }

                return View(model);
            }
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


        public async Task<IActionResult> Profile()
        {
            var token = HttpContext.Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login"); 
            }

            var user = await _crudService.Get<AdminGetResponse>("profile");

            AdminProfileEditRequest adminProfile = new AdminProfileEditRequest
            {
                UserName = user.UserName
            };
             TempData["UserId"] = user.Id;
            
            if (adminProfile == null)
            {
                return NotFound();
            }
            return View(adminProfile);
        }
        [HttpPost]
        public async Task<IActionResult> Profile(AdminProfileEditRequest editRequest, string id)
        {

            if (!ModelState.IsValid)
            {
                TempData["ProfileUpdateError"] = "Please correct the errors and try again.";

                TempData["UserId"] = id;
                return View(editRequest);
            }

            try
            {
                await _crudService.Update<AdminProfileEditRequest>(editRequest, "update/" + id);

               
                return RedirectToAction("login", "account");
            }
            catch (ModelException e)
            {
               foreach (var item in e.Error.Errors)
                {
                    TempData["ProfileUpdateError"] = "Please correct the errors and try again.";
                    ModelState.AddModelError(item.Key, item.Message);
                }
                TempData["UserId"] = id;
                return View(editRequest);
            }
        }

        public async Task<IActionResult> Logout()
        {
            
            if (Request.Cookies.ContainsKey("token"))
            {
                Response.Cookies.Delete("token");
            }        
           
            return RedirectToAction("Login", "Account");
        }

        public IActionResult ExternalLoginCallback(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }
        
            return RedirectToAction("IndexForUser", "Home");
        }
    }

}


