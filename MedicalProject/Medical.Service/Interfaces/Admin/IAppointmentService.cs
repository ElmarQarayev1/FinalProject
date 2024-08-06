﻿using System;
using Medical.Service.Dtos.Admin.CategoryDtos;
using Medical.Service.Dtos.User.AppointmentDtos;
using Medical.Service.Dtos.User.CategoryDtos;
using Medical.Service.Dtos.User.DoctorDtos;

namespace Medical.Service.Interfaces.Admin
{
	public interface IAppointmentService
	{
        Task<int> Create(AppointmentCreateDto createDto);
        PaginatedList<AppointmentPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10, int? doctorId = null);
        List<AppointmentGetDtoForFilter> GetByIdForFilter(int doctorId);
    }
}
