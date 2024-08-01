using System;
using Medical.Service.Dtos.Admin.DepartmentDtos;
using Medical.Service.Dtos.User.DepartmentDtos;

namespace Medical.Service.Interfaces.Admin
{
	public interface IDepartmentService
	{
        int Create(DepartmentCreateDto createDto);
        PaginatedList<DepartmentPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        List<DepartmentGetDto> GetAll(string? search = null);
        DepartmentGetDto GetById(int id);
        void Update(int id, DepartmentUpdateDto updateDto);
        void Delete(int id);

        List<DepartmentGetDtoForUser> GetForUserHome(string? search = null);

        List<DepartmentGetDtoForUser> GetAllUser(string? search = null);
    }

}

