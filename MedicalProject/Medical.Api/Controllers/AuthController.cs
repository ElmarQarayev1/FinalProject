using System;
using Medical.Core.Entities;
using Medical.Service;
using Medical.Service.Dtos.Admin.AuthDtos;
using Medical.Service.Exceptions;
using Medical.Service.Implementations.Admin;
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

        [Authorize]
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

        [Authorize(Roles = "SuperAdmin")]
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
    }
}

