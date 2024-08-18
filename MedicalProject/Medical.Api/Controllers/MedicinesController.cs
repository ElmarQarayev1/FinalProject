using System;
using System.Security.Claims;
using Medical.Service;
using Medical.Service.Dtos.Admin.DepartmentDtos;
using Medical.Service.Dtos.Admin.MedicineDtos;
using Medical.Service.Dtos.User.MedicineDtos;
using Medical.Service.Exceptions;
using Medical.Service.Implementations.Admin;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
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
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPost("api/admin/Medicines")]
        public ActionResult Create([FromForm] MedicineCreateDto createDto)
        {
            return StatusCode(201, new { Id = _medicineService.Create(createDto) });
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("api/admin/Medicines")]
        public ActionResult<PaginatedList<MedicinePaginatedGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _medicineService.GetAllByPage(search, page, size));
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/Medicines/Filter")]
        public ActionResult<PaginatedList<MedicinePaginatedGetDtoForUser>> GetAllForUser(string? search = null, int page = 1, int size =9, int? categoryId = null)
        {
            return StatusCode(200, _medicineService.GetAllByPageForUser(search, page, size,categoryId));
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/Medicines/LatestMedicines")]
        public ActionResult<List<MedicineGetDtoLatest>> GetLatestMedicines()
        {
            return StatusCode(200, _medicineService.GetAllLatest());
        }


        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("api/admin/Medicines/all")]
        public ActionResult<List<MedicineGetDto>> GetAll()
        {
            return StatusCode(200, _medicineService.GetAll());
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpGet("api/admin/Medicines/{id}")]
        public ActionResult<MedicineDetailsDto> GetById(int id)
        {
            return StatusCode(200, _medicineService.GetById(id));
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/Medicines/{id}")]
        public ActionResult<MedicineGetDtoForUser> GetByIdForUser(int id)
        {
            return StatusCode(200, _medicineService.GetByIdForUser(id));
        }


        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpPut("api/admin/Medicines/{id}")]
        public void Update(int id, [FromForm] MedicineUpdateDto updateDto)
        {
            _medicineService.Update(id, updateDto);
        }
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [HttpDelete("api/admin/Medicines/{id}")]
        public IActionResult Delete(int id)
        {
            _medicineService.Delete(id);
            return NoContent();
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [Authorize(Roles ="Member")]
        [HttpPost("api/Medicines/AddToBasket")]
        public ActionResult CreateBasket (MedicineBasketItemDto createDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var basketId =  _medicineService.BasketItem(createDto, userId);

            return StatusCode(201, new { Id = basketId });
           
        }
        [ApiExplorerSettings(GroupName = "user_v1")]
        [Authorize(Roles = "Member")]
        [HttpDelete("api/Medicines/RemoveFromBasket")]
        public IActionResult RemoveFromBasket(MedicineBasketDeleteDto deleteDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            _medicineService.RemoveItemFromBasket(deleteDto,userId);
            return NoContent();
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [Authorize(Roles = "Member")]
        [HttpPut("api/Medicines/EditBasketItem")]
        public void Update( MedicineBasketItemDto updateDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                throw new RestException(StatusCodes.Status401Unauthorized, "user doesn't login");
            }
            _medicineService.UpdateItemCountInBasket(updateDto,userId);
        }
       

    }
}

