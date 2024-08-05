using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Medical.Service;
using Medical.Service.Dtos.Admin.CategoryDtos;
using Medical.Service.Dtos.User.AppointmentDtos;
using Medical.Service.Exceptions;
using Medical.Service.Implementations.Admin;
using Medical.Service.Interfaces.Admin;
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

        [HttpPost("api/appointments")]
        public async Task<IActionResult> Create([FromBody] AppointmentCreateDto createDto)
        {
            var id = await _appointmentService.Create(createDto);
            return StatusCode(201, new { Id = id });
        }

        [HttpGet("api/admin/appointments")]
        public ActionResult<PaginatedList<CategoryPaginatedGetDto>> GetAll(string? search = null, int page = 1, int size = 10,int? doctorId=null)
        {
            return StatusCode(200, _appointmentService.GetAllByPage(search, page, size,doctorId));
        }

        [HttpGet("api/Appointments/{doctorId}")]
        public IActionResult GetByIdForAppointment(int doctorId)
        {
            var appointmentGetDtos = _appointmentService.GetByIdForFilter(doctorId);
            return Ok(appointmentGetDtos);

        }
    }
}
