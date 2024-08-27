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
using Microsoft.Extensions.Caching.Memory;

namespace Medical.Api.Controllers
{
	[ApiController]
	public class MedicinesController:ControllerBase
	{
		public IMedicineService _medicineService;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromSeconds(5);

        public MedicinesController(IMedicineService medicineService,IMemoryCache cache)
		{
			_medicineService = medicineService;
            _cache = cache;
		}


        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("api/admin/Medicines")]
        public ActionResult<PaginatedList<MedicinePaginatedGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            
            var result = _medicineService.GetAllByPage(search, page, size);
          
            return Ok(result);
        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles ="SuperAdmin,Admin")]
        [HttpPost("api/admin/Medicines")]
        public ActionResult Create([FromForm] MedicineCreateDto createDto)
        {
            var newMedicineId = _medicineService.Create(createDto);
         

            return StatusCode(201, new { Id = newMedicineId });
        }

       


        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/Medicines/Filter")]
        public ActionResult<PaginatedList<MedicinePaginatedGetDtoForUser>> GetAllForUser(string? search = null, int page = 1, int size = 9, int? categoryId = null)
        {
            var cacheKey = $"Medicines_GetAllForUser_{search}_{page}_{size}_{categoryId}";
            if (_cache.TryGetValue(cacheKey, out PaginatedList<MedicinePaginatedGetDtoForUser> cachedResult))
            {
                return Ok(cachedResult);
            }

            var result = _medicineService.GetAllByPageForUser(search, page, size, categoryId);
            _cache.Set(cacheKey, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            });


            return Ok(result);
        }

        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/Medicines/LatestMedicines")]
        public ActionResult<List<MedicineGetDtoLatest>> GetLatestMedicines()
        {
            var cacheKey = "Medicines_GetLatestMedicines";
            if (_cache.TryGetValue(cacheKey, out List<MedicineGetDtoLatest> cachedResult))
            {
                return Ok(cachedResult);
            }

            var result = _medicineService.GetAllLatest();
            _cache.Set(cacheKey, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            });

            return Ok(result);
        }


        [ApiExplorerSettings(GroupName = "user_v1")]
        [Authorize(Roles = "Member")]
        [HttpGet("api/Medicines/BasketItems")]
        public ActionResult<List<MedicineBasketItemDtoForView>> GetAllBasktemItem()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = _medicineService.GetAllBasketItem(userId);

            return Ok(result);
        }
     
        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("api/admin/Medicines/all")]
        public ActionResult<List<MedicineGetDto>> GetAllAdmin()
        {
           
            var result = _medicineService.GetAll();
           

            return Ok(result);
        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("api/admin/Medicines/{id}")]
        public ActionResult<MedicineDetailsDto> GetById(int id)
        {
            

            var result = _medicineService.GetById(id);
         

            return Ok(result);
        }


        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/Medicines/{id}")]
        public ActionResult<MedicineGetDtoForUser> GetByIdForUser(int id)
        {
            var cacheKey = $"Medicine_GetByIdForUser_{id}";
            if (_cache.TryGetValue(cacheKey, out MedicineGetDtoForUser cachedResult))
            {
                return Ok(cachedResult);
            }

            var result = _medicineService.GetByIdForUser(id);
            _cache.Set(cacheKey, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            });

            return Ok(result);
        }


        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut("api/admin/Medicines/{id}")]
        public void Update(int id, [FromForm] MedicineUpdateDto updateDto)
        {
            _medicineService.Update(id, updateDto);        
           
        }

        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
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

