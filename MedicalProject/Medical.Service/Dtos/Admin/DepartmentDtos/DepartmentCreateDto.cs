using System;
using FluentValidation;
using Medical.Service.Dtos.Admin.SliderDtos;
using Microsoft.AspNetCore.Http;

namespace Medical.Service.Dtos.Admin.DepartmentDtos
{
	public class DepartmentCreateDto
	{
        public string Name { get; set; }

        public IFormFile File { get; set; }

    }
    public class DepartmentCreateDtoValidator : AbstractValidator<DepartmentCreateDto>
    {
        public DepartmentCreateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(35).MinimumLength(2);
            

            RuleFor(x => x.File)
                .Must(file => file == null || file.Length <= 2 * 1024 * 1024)
                .WithMessage("File must be less than or equal to 2MB.")
                .Must(file => file == null || new[] { "image/png", "image/jpeg" }.Contains(file.ContentType))
                .WithMessage("File type must be png, jpeg, or jpg.");
        }
    }


}

