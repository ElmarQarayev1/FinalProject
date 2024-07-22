using System;
using Medical.Service;
using Medical.Service.Dtos.Admin.DepartmentDtos;
using Medical.Service.Dtos.Admin.MedicineDtos;
using Medical.Service.Implementations.Admin;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Medical.Api.Controllers
{
	[ApiController]
	public class MedicinesController:ControllerBase
	{
		public IMedicineService _medicineService;

		public MedicinesController(IMedicineService medicineService)
		{
			_medicineService = medicineService;
		}
        [HttpPost("api/admin/Medicines")]
        public ActionResult Create([FromForm] MedicineCreateDto createDto)
        {
            return StatusCode(201, new { Id = _medicineService.Create(createDto) });
        }

        [HttpGet("api/admin/Medicines")]
        public ActionResult<PaginatedList<MedicinePaginatedGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _medicineService.GetAllByPage(search, page, size));
        }

        [HttpGet("api/admin/Medicines/all")]
        public ActionResult<List<MedicineGetDto>> GetAll()
        {
            return StatusCode(200, _medicineService.GetAll());
        }

        [HttpGet("api/admin/Medicines/{id}")]
        public ActionResult<MedicineDetailsDto> GetById(int id)
        {
            return StatusCode(200, _medicineService.GetById(id));
        }

        [HttpPut("api/admin/Medicines/{id}")]
        public void Update(int id, [FromForm] MedicineUpdateDto updateDto)
        {
            _medicineService.Update(id, updateDto);
        }

        [HttpDelete("api/admin/Medicines/{id}")]
        public IActionResult Delete(int id)
        {
            _medicineService.Delete(id);
            return NoContent();
        }
    }
}

