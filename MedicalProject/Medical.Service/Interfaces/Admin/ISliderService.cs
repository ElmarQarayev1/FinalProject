﻿using System;
using Medical.Service.Dtos.Admin.SliderDtos;

namespace Medical.Service.Interfaces.Admin
{
	public interface ISliderService
	{
        int Create(SliderCreateDto createDto);
        PaginatedList<SliderPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        List<SliderGetDto> GetAll(string? search = null);
        SliderGetDto GetById(int id);
        void Update(int id, SliderUpdateDto updateDto);
        void Delete(int id);
    }
}

