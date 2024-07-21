using System;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Medical.Service.Dtos.Admin.SliderDtos
{
	public class SliderUpdateDto
	{
        public string MainTitle { get; set; }

        public string SubTitle1 { get; set; }

        public string SubTitle2 { get; set; }

        public IFormFile FileUrl { get; set; }

        public int Order { get; set; }
    }
    public class SliderUpdateDtoValidator : AbstractValidator<SliderUpdateDto>
    {
        public SliderUpdateDtoValidator()
        {
            RuleFor(x => x.MainTitle)
               .MaximumLength(35).WithMessage("MainTitle cannot be longer than 35 characters.")
               .MinimumLength(2).WithMessage("MainTitle must be at least 2 characters long.");


            RuleFor(x => x.SubTitle1)
                .MaximumLength(200).WithMessage("SubTitle1 cannot be longer than 200 characters.")
                .MinimumLength(3).WithMessage("SubTitle1 must be at least 3 characters long.");


            RuleFor(x => x.SubTitle2)
                .MaximumLength(200).WithMessage("SubTitle1 cannot be longer than 200 characters.")
                .MinimumLength(3).WithMessage("SubTitle1 must be at least 3 characters long.");


            RuleFor(x => x.Order)
                .GreaterThan(0).WithMessage("'Order' must be greater than 0.");


            RuleFor(x => x.FileUrl)
                .Must(file => file == null || file.Length <= 2 * 1024 * 1024)
                .WithMessage("File must be less than or equal to 2MB.")
                .Must(file => file == null || new[] { "image/png", "image/jpeg" }.Contains(file.ContentType))
                .When(x => x.FileUrl != null);
        }
    }
}

