using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Medical.Service;
using Medical.Service.Dtos.Admin.CategoryDtos;
using Medical.Service.Dtos.User.AppointmentDtos;
using Medical.Service.Dtos.User.MedicineDtos;
using Medical.Service.Exceptions;
using Medical.Service.Implementations.Admin;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Medical.Api.Controllers
{
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IMapper _mapper;

        public AppointmentsController(IAppointmentService appointmentService, IMapper mapper)
        {
            _appointmentService = appointmentService;
            _mapper = mapper;
        }


        [ApiExplorerSettings(GroupName = "user_v1")]
        [Authorize(Roles = "Member")]
        [HttpPost("api/appointments")]
        public async Task<IActionResult> Create([FromBody] AppointmentCreateDto createDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var appointmentId = await _appointmentService.Create(createDto, userId);

            return StatusCode(201, new { Id = appointmentId });
        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("api/admin/daily-count")]
        public async Task<IActionResult> GetDailyAppointmentsCount()
        {
            try
            {
                var count = await _appointmentService.GetDailyAppointmentsCountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, "Internal server error");
            }
        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("api/admin/yearly-count")]
        public async Task<IActionResult> GetYearlyAppointmentsCount()
        {
            try
            {
                var count = await _appointmentService.GetYearlyAppointmentsCountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, "Internal server error");
            }
        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("api/admin/monthly-count")]
        public async Task<IActionResult> GetMonthlyAppointmentsCount()
        {
            try
            {
                var counts = await _appointmentService.GetMonthlyAppointmentsCountAsync();

               
                var response = new
                {
                    months = counts.Keys.ToArray(),
                    appointments = counts.Values.ToArray()
                };

               return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("api/admin/appointments")]
        public ActionResult<PaginatedList<AppointmentPaginatedGetDto>> GetAll(string? search = null, int page = 1, int size = 10,int? doctorId=null)
        {
            return StatusCode(200, _appointmentService.GetAllByPage(search, page, size,doctorId));
        }




        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("api/admin/Appointments/{doctorId}")]
        public IActionResult GetByIdForAppointment(int doctorId)
        {
            var appointmentGetDtos = _appointmentService.GetByIdForFilter(doctorId);
            return Ok(appointmentGetDtos);

         }
    }
}
