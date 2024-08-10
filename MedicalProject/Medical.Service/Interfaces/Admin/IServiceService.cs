using System;
using Medical.Service.Dtos.Admin.ServiceDtos;
using Medical.Service.Dtos.User.FeatureDtos;
using Medical.Service.Dtos.User.ServiceDtos;

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

        List<ServiceGetDtoForUser> GetForUserHome(string? search = null);

        List<ServiceGetDtoForUser> GetAllUser(string? search = null);

        List<ServiceForDownSideDto> GetAllUserForDownSide(string? search = null);
    }
}

