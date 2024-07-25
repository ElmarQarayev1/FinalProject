using System;
using FluentValidation;
using Medical.Service.Dtos.Admin.CategoryDtos;

namespace Medical.Service.Dtos.Admin.SettingDtos
{
	public class SettingUpdateDto
	{              

        public string Value { get; set; }

    }
    public class SettingUpdateDtoValidator : AbstractValidator<SettingUpdateDto>
    {
        public SettingUpdateDtoValidator()
        {
            RuleFor(x => x.Value).NotEmpty().MinimumLength(1);

        }
    }
}

