using System;
using Medical.Service.Dtos.Admin.DoctorDtos;
using Medical.Service.Dtos.Admin.ServiceDtos;
using Medical.Service.Dtos.User.DoctorDtos;
using Medical.Service.Dtos.User.FeatureDtos;

namespace Medical.Service.Interfaces.Admin
{
	public interface IDoctorService
	{
        int Create(DoctorCreateDto createDto);
        PaginatedList<DoctorPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        List<DoctorGetDto> GetAll(string? search = null);
        DoctorGetDto GetById(int id);
        void Update(int id, DoctorUpdateDto updateDto);
        void Delete(int id);
        List<DoctorGetDtoForUser> GetForUserHome(string? search = null);

        List<DoctorGetDtoForUser> GetAllUser(string? search = null);

        DoctorGetDetailDto GetByIdForUser(int id);

        List<DoctorGetDtoForAppointment> GetByIdForAppointment(int departmentId);
    }
}

