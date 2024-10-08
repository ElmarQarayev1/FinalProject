﻿using System;
using System.Drawing;
using AutoMapper;
using FluentValidation;
using Medical.Core.Entities;
using Medical.Data.Repositories.Implementations;
using Medical.Data.Repositories.Interfaces;
using Medical.Service.Dtos.Admin.SettingDtos;
using Medical.Service.Exceptions;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Medical.Service.Implementations.Admin
{
	public class SettingService:ISettingService
	{
		private readonly ISettingRepository _settingRepository;

		private readonly IMapper _mapper;

		private readonly IWebHostEnvironment _env;

		public SettingService(ISettingRepository settingRepository,IMapper mapper,IWebHostEnvironment env)
		{

			_settingRepository = settingRepository;

			_mapper = mapper;

            _env = env;
        }

        public void Delete(string key)
        {
            Setting entity = _settingRepository.Get(x => x.Key == key);

            if (entity == null)
                throw new RestException(StatusCodes.Status404NotFound, "Setting not found");

            _settingRepository.Delete(entity);

            _settingRepository.Save();
        }

        public List<SettingGetDto> GetAll(string? search = null)
        {
            var settings = _settingRepository.GetAll(x => search == null || x.Value.Contains(search)).ToList();
            return _mapper.Map<List<SettingGetDto>>(settings);
        }

        public PaginatedList<SettingPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var query = _settingRepository.GetAll(x => x.Value.Contains(search) || search == null);


            var paginated = PaginatedList<Setting>.Create(query, page, size);

            var settingDtos = _mapper.Map<List<SettingPaginatedGetDto>>(paginated.Items);

            return new PaginatedList<SettingPaginatedGetDto>(settingDtos, paginated.TotalPages, page, size);
        }

        public SettingGetDto GetByKey(string key)
        {
            Setting setting = _settingRepository.Get(x => x.Key == key);

            if (setting == null)
                throw new RestException(StatusCodes.Status404NotFound, "Setting not found");

            return _mapper.Map<SettingGetDto>(setting);

        }

        public void Update(string key, SettingUpdateDto updateDto)
        {
            Setting entity = _settingRepository.Get(x => x.Key == key);

            if (entity == null)
                throw new RestException(StatusCodes.Status404NotFound, "Setting not found");
            
            entity.Value = updateDto.Value;

            _settingRepository.Save();
        }
    }
}

