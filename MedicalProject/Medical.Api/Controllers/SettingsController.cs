using System;
using Medical.Service;
using Medical.Service.Dtos.Admin.ServiceDtos;
using Medical.Service.Dtos.Admin.SettingDtos;
using Medical.Service.Implementations.Admin;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Medical.Api.Controllers
{
    [ApiController]
	public class SettingsController:ControllerBase
	{
		public ISettingService _settingService;

		public SettingsController(ISettingService settingService)
		{
			_settingService = settingService;
		}
 
        [HttpGet("api/admin/Settings")]
        public ActionResult<PaginatedList<SettingPaginatedGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _settingService.GetAllByPage(search, page, size));
        }

        [HttpGet("api/admin/Settings/all")]
        public ActionResult<List<SettingGetDto>> GetAll()
        {
            return StatusCode(200, _settingService.GetAll());
        }

        [HttpGet("api/admin/Settings/{key}")]
        public ActionResult<ServiceGetDto> GetByKey(string key)
        {
            return StatusCode(200, _settingService.GetByKey(key));
        }

        [HttpPut("api/admin/Settings/{key}")]
        public void Update(string key, [FromForm] SettingUpdateDto updateDto)
        {
            _settingService.Update(key, updateDto);
        }

        [HttpDelete("api/admin/Settings/{key}")]
        public IActionResult Delete(string key)
        {
            _settingService.Delete(key);
            return NoContent();
        }
    }
}

