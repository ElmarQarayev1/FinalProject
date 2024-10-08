﻿using System;
using Medical.Service.Dtos.Admin.FeatureDtos;
using Medical.Service.Dtos.User.FeatureDtos;

namespace Medical.Service.Interfaces.Admin
{
	public interface IFeatureService
	{
        int Create(FeatureCreateDto createDto);
        PaginatedList<FeaturePaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        List<FeatureGetDto> GetAll(string? search = null);
        FeatureGetDto GetById(int id);
        void Update(int id, FeatureUpdateDto updateDto);
        void Delete(int id);
        List<FeatureGetDtoForUser> GetForUserHome(string? search = null);

        List<FeatureGetDtoForUser> GetAllUser(string? search = null);

    }
}

