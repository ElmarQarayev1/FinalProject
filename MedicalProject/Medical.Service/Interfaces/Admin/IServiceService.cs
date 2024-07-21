using System;
using Medical.Service.Dtos.Admin.DepartmentDtos;
using Medical.Service.Dtos.Admin.ServiceDtos;

namespace Medical.Service.Interfaces.Admin
{
	public interface IServiceService
	{
        int Create(ServiceCreateDto createDto);
        PaginatedList<ServicePaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        List<ServiceGetDto> GetAll(string? search = null);
        ServiceGetDto GetById(int id);
        void Update(int id, ServiceUpdateDto updateDto);
        void Delete(int id);
    }
}

