using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Medical.Core.Entities;
using Medical.Data;
using Medical.Data.Repositories.Implementations;
using Medical.Data.Repositories.Interfaces;
using Medical.Service.Dtos.Admin.CategoryDtos;
using Medical.Service.Dtos.User.AppointmentDtos;
using Medical.Service.Exceptions;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Medical.Service.Implementations.Admin
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;

        public AppointmentService(AppDbContext context,UserManager<AppUser> userManager,IAppointmentRepository appointmentRepository,IDoctorRepository doctorRepository,IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _appointmentRepository = appointmentRepository;
            _doctorRepository = doctorRepository;
            _mapper = mapper;
        }


        public async Task<int> Create(AppointmentCreateDto createDto)
        {
            var doctor = _doctorRepository.Get(x => x.Id == createDto.DoctorId);

            if (doctor == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "doctorId", "Doctor Not Found");
            }
            var user = await _userManager.FindByIdAsync(createDto.AppUserId);
            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "AppUserId", "User not found.");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Email", "Email is not confirmed.");
            }

            var doctorAppointments = await _appointmentRepository.GetAll(
                a => a.DoctorId == createDto.DoctorId && a.Date.Date == createDto.Date.Date, "Doctor").ToListAsync();

            doctorAppointments = doctorAppointments ?? new List<Appointment>();

            foreach (var appointment in doctorAppointments)
            {
                if (appointment.Date.AddMinutes(30) > createDto.Date && appointment.Date < createDto.Date.AddMinutes(30))
                {
                    throw new RestException(StatusCodes.Status400BadRequest, "The doctor already has an appointment at this time.");
                }
            }

            var appointmentEntity = new Appointment
            {
                AppUserId = user.Id,
                DoctorId = createDto.DoctorId,
                FullName = user.FullName,
                Phone = createDto.Phone,
                Date = createDto.Date,
                CreatedAt=DateTime.Now
            };

            await _context.Appointments.AddAsync(appointmentEntity);
            await _context.SaveChangesAsync();

            return appointmentEntity.Id;
        }

        public PaginatedList<AppointmentPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10, int? doctorId = null)
        {
            if (doctorId!=null)
            {
                var doctor = _doctorRepository.Get(x => x.Id == doctorId.Value);
                if (doctor == null)
                {
                    throw new RestException(StatusCodes.Status404NotFound, "doctorId", "Doctor Not Found");
                }
            }

            var query = _appointmentRepository.GetAll(
                x => (x.FullName.Contains(search) || search == null) &&
                     (doctorId == null || x.DoctorId == doctorId),
                "Doctor"
            );

            var paginated = PaginatedList<Appointment>.Create(query, page, size);

            var appointmentDtos = _mapper.Map<List<AppointmentPaginatedGetDto>>(paginated.Items);

            return new PaginatedList<AppointmentPaginatedGetDto>(appointmentDtos, paginated.TotalPages, page, size);
        }

        public List<AppointmentGetDtoForFilter> GetByIdForFilter(int doctorId)
        {
            var doctor = _doctorRepository.Get(x => x.Id == doctorId);

            if (doctor == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "doctorId", "Doctor Not Found");
            }
            return _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Select(a => new AppointmentGetDtoForFilter
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    Phone = a.Phone,
                    Date = a.Date
                }).ToList();
        }
    }
}
