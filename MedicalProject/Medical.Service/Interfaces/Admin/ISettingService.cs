using System;
using Medical.Service.Dtos.Admin.ServiceDtos;
using Medical.Service.Dtos.Admin.SettingDtos;

namespace Medical.Service.Interfaces.Admin
{
	public interface ISettingService
	{
       
        PaginatedList<SettingPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        List<SettingGetDto> GetAll(string? search = null);
        SettingGetDto GetByKey(string key);
        void Update(string key, SettingUpdateDto updateDto);
        void Delete(string key);

    }
}

