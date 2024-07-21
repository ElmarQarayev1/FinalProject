using System;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Medical.Service.Dtos.Admin.ServiceDtos
{
	public class ServiceUpdateDto
	{

        public string Name { get; set; }

        public string Desc { get; set; }

        public IFormFile FileUrl { get; set; }
    }
    public class ServiceUpdateDtoValidator : AbstractValidator<ServiceUpdateDto>
    {
        public ServiceUpdateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(35).MinimumLength(2);

            RuleFor(x => x.Desc).NotEmpty().MaximumLength(200).MinimumLength(3);


            RuleFor(x => x.FileUrl)
                .Must(file => file == null || file.Length <= 2 * 1024 * 1024)
                .WithMessage("File must be less than or equal to 2MB.")
                .Must(file => file == null || new[] { "image/png", "image/jpeg" }.Contains(file.ContentType))
                .WithMessage("File type must be png, jpeg, or jpg.");
        }
    }
}

