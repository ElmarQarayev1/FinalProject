using System;
using Medical.Core.Entities;
using Medical.Service.Dtos.Admin.AuthDtos;
using Medical.Service.Interfaces.Admin;
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

        [HttpPost("api/admin/login")]
        public ActionResult Login(AdminLoginDto loginDto)
        {
            var token = _authService.Login(loginDto);
            return Ok(new { token });
        }

        [Authorize]
        [HttpGet("api/admin/profile")]
        public ActionResult Profile()
        {
            return Ok(User.Identity.Name);
        }

    }
}

