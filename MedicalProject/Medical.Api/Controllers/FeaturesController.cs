﻿using System;
using Medical.Service;
using Medical.Service.Dtos.Admin.FeatureDtos;
using Medical.Service.Dtos.Admin.ServiceDtos;
using Medical.Service.Dtos.User.FeatureDtos;
using Medical.Service.Implementations.Admin;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medical.Api.Controllers
{
    [ApiController]
    public class FeaturesController:ControllerBase
	{
        public IFeatureService _featureService;
        public FeaturesController(IFeatureService featureService)
        {
            _featureService = featureService;
        }


        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("api/admin/Features")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Create([FromForm]  FeatureCreateDto createDto)
        {
            return StatusCode(201, new { Id = _featureService.Create(createDto) });
        }


        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("api/admin/Features")]
        public ActionResult<PaginatedList<FeaturePaginatedGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _featureService.GetAllByPage(search, page, size));
        }




        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("api/admin/Features/all")]
        public ActionResult<List<FeatureGetDto>> GetAll()
        {
            return StatusCode(200, _featureService.GetAll());
        }



        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/Features")]
        public ActionResult<List<FeatureGetDtoForUser>> GetAllForUserHome()
        {
            return StatusCode(200, _featureService.GetForUserHome());
        }



        [ApiExplorerSettings(GroupName = "user_v1")]
        [HttpGet("api/Features/all")]
        public ActionResult<List<FeatureGetDtoForUser>> GetAllUser()
        {
            return StatusCode(200, _featureService.GetAll());
        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("api/admin/Features/{id}")]
        public ActionResult<FeatureGetDto> GetById(int id)
        {
            return StatusCode(200, _featureService.GetById(id));
        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut("api/admin/Features/{id}")]
        public void Update(int id, [FromForm] FeatureUpdateDto updateDto)
        {
            _featureService.Update(id, updateDto);
        }



        [ApiExplorerSettings(GroupName = "admin_v1")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete("api/admin/Features/{id}")]
        public IActionResult Delete(int id)
        {
            _featureService.Delete(id);
            return NoContent();
        }
    }
}

