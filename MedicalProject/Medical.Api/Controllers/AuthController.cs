﻿using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Medical.Core.Entities;
using Medical.Service;
using Medical.Service.Dtos.Admin.AuthDtos;
using Medical.Service.Dtos.User.AuthDtos;
using Medical.Service.Exceptions;
using Medical.Service.Helpers;
using Medical.Service.Implementations.Admin;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Medical.Api.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration configuration;


        public AuthController(IAuthService authService, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager,IConfiguration _configuration)
        {
            _authService = authService;
            _userManager = userManager;
            _roleManager = roleManager;
            configuration = _configuration;

        }


        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/signin-google")]
        public async Task<IActionResult> GoogleLogin()
        {
          
            var response = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (response.Principal == null)
                return BadRequest("Google authentication failed.");

           
            var email = response.Principal.FindFirstValue(ClaimTypes.Email);
            var fullName = response.Principal.FindFirstValue(ClaimTypes.Name);
            var userName = email;

           
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(userName))
                return BadRequest("Incomplete Google account information.");

          
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
              
                return BadRequest("A user with this email address already exists.");
            }
           
            user = new AppUser
            {
                Email = email,
                UserName = userName,
                FullName = fullName,
                EmailConfirmed = true,
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                return BadRequest("User creation failed.");
            }
            
            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            if (!roleResult.Succeeded)
            {
                return BadRequest("Failed to assign role to user.");
            }

            var token = await GenerateJwtToken(user);
            if (token == null)
                return BadRequest("Login failed. Please try again.");

           
            var redirectUrl = $"{configuration["Client:URL"]}/account/ExternalLoginCallback?token={token}";
            return Redirect(redirectUrl);
        }

        
      
        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpPost("api/login")]
        public async Task<IActionResult> LoginForUser([FromBody] MemberLoginDto loginDto)
        {

            var token = await _authService.LoginForUser(loginDto);


            return Ok(new { Result = token });

        }


        [ApiExplorerSettings(GroupName = "user_v1")]
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

        


        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/login-google")]
        public IActionResult Login()
        {
            var props = new AuthenticationProperties { RedirectUri = "api/signin-google" };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }

     


        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("api/admin/createAdmin")]
        public IActionResult Create(SuperAdminCreateAdminDto createDto)
        {

            return StatusCode(201, new { Id = _authService.Create(createDto) });
        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPost("api/admin/login")]
        public ActionResult Login(AdminLoginDto loginDto)
        {
            var token = _authService.Login(loginDto);
            return Ok(new { token });
        }


        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpPost("api/forgetpassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] MemberForgetPasswordDto forgetPasswordDto)
        {
            try
            {
                var resetUrl = await _authService.ForgetPassword(forgetPasswordDto);
                return Ok(new { Message = "Password reset link has been sent to your email." });
            }
            catch (RestException ex)
            {
                return StatusCode(ex.Code, ex.Message);
            }
        }



        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpPost("api/resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] MemberResetPasswordDto resetPasswordDto)
        {
            try
            {
                await _authService.ResetPassword(resetPasswordDto);
                return Ok(new { Message = "Password has been reset successfully. Please log in with your new password." });
            }
            catch (RestException ex)
            {
                return StatusCode(ex.Code, ex.Message);
            }
        }




        [ApiExplorerSettings(GroupName = "user_v1")]
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



        [ApiExplorerSettings(GroupName = "user_v1")]
        [Authorize(Roles ="Member")]
        [HttpPost("api/profile/update")]
        public async Task<IActionResult> UpdateProfile([FromBody] MemberProfileEditDto profileEditDto)
        {
            await _authService.UpdateProfile(profileEditDto);
            return Ok(new { message = "Profile updated successfully!" });
        }




        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/account/verifyemail")]
        public async Task<IActionResult> VerifyEmail(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid email verification request.");
            }

            var user = await _userManager.FindByEmailAsync(email);
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




        [ApiExplorerSettings(GroupName = "admin_v1")]
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



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("api/superadmin/adminAll")]
        public ActionResult<List<AdminGetDto>> GetAll(string? search = null)
        {
            var admins = _authService.GetAll(search);
            return Ok(admins);
        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("api/admin/adminAllByPage")]
        public ActionResult<PaginatedList<AdminPaginatedGetDto>> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var paginatedAdmins = _authService.GetAllByPage(search, page, size);
            return Ok(paginatedAdmins);
        }




        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("api/admin/getById/{id}")]
        public ActionResult<AdminGetDto> GetById(string id)
        {
            var admin = _authService.GetById(id);
            return Ok(admin);
        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("api/admin/update/{id}")]
        public IActionResult Update(string id, AdminUpdateDto updateDto)
        {
            _authService.Update(id, updateDto);
            return NoContent();
        }




        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPut("api/admin/updatePassword")]
        public async Task<IActionResult> UpdatePassword(AdminUpdateDto updatePasswordDto)
        {

            await _authService.UpdatePasswordAsync(updatePasswordDto);
            return NoContent();

        }



        [ApiExplorerSettings(GroupName = "user_v1")]
        [Authorize(Roles = "Member")]
        [HttpGet("api/GetUserProfile")]
        public ActionResult<MemberProfileGetDto> GetUserProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            try
            {
                var userProfile = _authService.GetByIdForUserProfile(userId);
                return Ok(userProfile);
            }
            catch (RestException ex)
            {
                return StatusCode(ex.Code, new { message = ex.Message });
            }
        }




        [ApiExplorerSettings(GroupName = "admin_v1")]
       // [Authorize]
        [HttpGet("api/profileLayout")]
        public ActionResult ProfileForLayout()
        {
            var userName = User.Identity.Name;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            return Ok(new UserProfileDto { UserName = userName, Role = role });
        }




        private async Task<string> GenerateJwtToken(AppUser user)
        {
            var claims = new List<Claim>
        {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim("FullName", user.FullName)
         };


            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


            var secret = configuration.GetSection("JWT:Secret").Value;
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: configuration.GetSection("JWT:Issuer").Value,
                audience: configuration.GetSection("JWT:Audience").Value,
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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

