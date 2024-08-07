using System;
using Medical.Core.Entities;
using Medical.Service;
using Medical.Service.Dtos.Admin.AuthDtos;
using Medical.Service.Dtos.User.AuthDtos;
using Medical.Service.Exceptions;
using Medical.Service.Implementations.Admin;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Medical.Api.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AuthController(IAuthService authService, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _authService = authService;
            _userManager = userManager;
            _roleManager = roleManager;

        }

    

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("api/admin/createAdmin")]
        public IActionResult Create(SuperAdminCreateAdminDto createDto)
        {

            return StatusCode(201, new { Id = _authService.Create(createDto) });
        }

        [HttpPost("api/admin/login")]
        public ActionResult Login(AdminLoginDto loginDto)
        {
            var token = _authService.Login(loginDto);
            return Ok(new { token });
        }




        [HttpPost("api/login")]
        public async Task<IActionResult> LoginForUser([FromBody] MemberLoginDto loginDto)
        {
                     
                var token = await _authService.LoginForUser(loginDto);

                
                return Ok(new {  Result =token});
                     
        }


        [HttpPost("api/register")]
        public async Task<ActionResult> RegisterForUser([FromBody] MemberRegisterDto registerDto)
        {
            try
            {
                var Id = await _authService.Register(registerDto);
                return Ok(new { Result = Id });
            }
            catch (RestException ex)
            {
                var errorResponse = new
                {
                    message = ex.Message,
                    errors = ex.Errors
                };
                return StatusCode(ex.Code, errorResponse);
            }
        }


        [HttpPost("api/forgetpassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] MemberForgetPasswordDto forgetPasswordDto)
        {
            try
            {
                var resetUrl = await _authService.ForgetPassword(forgetPasswordDto);
                return Ok(new { Message = "Password reset link has been sent to your email.", ResetUrl = resetUrl });
            }
            catch (RestException ex)
            {
                return StatusCode(ex.Code, ex.Message);
            }
        }

        [HttpPost("api/resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] MemberResetPasswordDto resetPasswordDto)
        {
            try
            {
                await _authService.ResetPassword(resetPasswordDto);
                return Ok("Password has been reset successfully. Please log in with your new password.");
            }
            catch (RestException ex)
            {
                return StatusCode(ex.Code, ex.Message);
            }
        }

        [HttpPost("api/verify")]
        public async Task<IActionResult> Verify([FromBody] MemberVerifyDto verifyDto)
        {
            try
            {
                bool isValid = await _authService.VerifyEmailAndToken(verifyDto.Email, verifyDto.Token);
                if (isValid)
                {
                    return Ok("Email and token are valid. You can now reset your password.");
                }
                else
                {
                    return BadRequest("Invalid email or token.");
                }
            }
            catch (RestException ex)
            {
                return StatusCode(ex.Code, ex.Message);
            }
        }

        
        [HttpPost("api/profile/update")]
        public async Task<IActionResult> UpdateProfile([FromBody] MemberProfileEditDto profileEditDto)
        {
            await _authService.UpdateProfile(profileEditDto);
            return Ok(new { message = "Profile updated successfully!" });
        }

       [Authorize(Roles ="Member")]
        [HttpPost("api/CheckEmailConfirmationStatus")]
        public async Task<IActionResult> CheckConfirmation(string userId)
        {
            try
            {
                bool isConfirmed = await _authService.IsEmailConfirmedAsync(userId);
                if (isConfirmed)
                {
                    return Ok("Email confirmed successfully!");
                }
                else
                {
                    return BadRequest("Email is not confirmed.");
                }
            }
            catch (RestException ex)
            {
                return StatusCode(ex.Code, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("api/account/verifyemail")]
        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid email verification request.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok("Email confirmed successfully!");
            }

            return BadRequest("Failed to confirm email.");
        }



        [Authorize(Roles ="Admin,SuperAdmin")]
        [HttpGet("api/admin/profile")]
        public ActionResult Profile()
        {
            var userName = User.Identity.Name;


            var user = _userManager.FindByNameAsync(userName).Result;

            if (user == null)
            {
                return NotFound("User not found.");
            }
            var userDto = new AdminGetDto
            {
                Id = user.Id,
                UserName = user.UserName
            };

            return Ok(userDto);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("api/superadmin/adminAll")]
        public ActionResult<List<AdminGetDto>> GetAll(string? search = null)
        {
            var admins = _authService.GetAll(search);
            return Ok(admins);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("api/admin/adminAllByPage")]
        public ActionResult<PaginatedList<AdminPaginatedGetDto>> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var paginatedAdmins = _authService.GetAllByPage(search, page, size);
            return Ok(paginatedAdmins);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("api/admin/getById/{id}")]
        public ActionResult<AdminGetDto> GetById(string id)
        {
            var admin = _authService.GetById(id);
            return Ok(admin);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("api/admin/update/{id}")]
        public IActionResult Update(string id, AdminUpdateDto updateDto)
        {
            _authService.Update(id, updateDto);
            return NoContent();
        }

       
        [HttpPut("api/admin/updatePassword")]
        public async Task<IActionResult> UpdatePassword(AdminUpdateDto updatePasswordDto)
        {

            await _authService.UpdatePasswordAsync(updatePasswordDto);
            return NoContent();

        }
        [Authorize(Roles = "Member")]
        [HttpGet("api/GetUserProfile/{appUserId}")]
        public ActionResult<MemberProfileGetDto> GetUserProfile(string appUserId)
        {
            try
            {
                var userProfile = _authService.GetByIdForUserProfile(appUserId);
                return Ok(userProfile);
            }
            catch (RestException ex)
            {
                return StatusCode(ex.Code, new { message = ex.Message });
            }
        }



        //[HttpGet("api/admin/createUser")]
        //public async Task<IActionResult> CreateUser()
        //{

        //    await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
        //    await _roleManager.CreateAsync(new IdentityRole("Admin"));
        //    await _roleManager.CreateAsync(new IdentityRole("Member"));


        //    AppUser user1 = new AppUser
        //    {
        //        FullName = "Admin",
        //        UserName = "admin",
        //    };

        //    await _userManager.CreateAsync(user1, "Admin123");

        //    AppUser user2 = new AppUser
        //    {
        //        FullName = "Member",
        //        UserName = "member",
        //    };

        //    await _userManager.CreateAsync(user2, "Member123");


        //    AppUser user3 = new AppUser
        //    {
        //        FullName = "SuperAdmin",
        //        UserName = "superadmin",
        //    };

        //    await _userManager.CreateAsync(user3, "SuperAdmin123");


        //    await _userManager.AddToRoleAsync(user3, "SuperAdmin");
        //    await _userManager.AddToRoleAsync(user1, "Admin");
        //    await _userManager.AddToRoleAsync(user2, "Member");

        //    return Ok(user1.Id);
        //}
    }
}

