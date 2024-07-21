using System;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Medical.Service.Dtos.Admin.SliderDtos
{
	public class SliderCreateDto
	{
		public string MainTitle { get; set; }

		public string SubTitle1 { get; set; }

		public string SubTitle2 { get; set; }

		public IFormFile FileUrl { get; set; }

		public int Order { get; set; }
	}
    public class SliderCreateDtoValidator : AbstractValidator<SliderCreateDto>
    {
        public SliderCreateDtoValidator()
        {
            RuleFor(x => x.MainTitle).NotEmpty().MaximumLength(35).MinimumLength(2);
            RuleFor(x => x.SubTitle1).NotEmpty().MaximumLength(200).MinimumLength(2);
            RuleFor(x => x.SubTitle2).NotEmpty().MaximumLength(200).MinimumLength(2);

            RuleFor(x => x.Order).NotEmpty().GreaterThan(0);


            RuleFor(x => x.FileUrl)
                .Must(file => file == null || file.Length <= 2 * 1024 * 1024)
                .WithMessage("File must be less than or equal to 2MB.")
                .Must(file => file == null || new[] { "image/png", "image/jpeg" }.Contains(file.ContentType))
                .WithMessage("File type must be png, jpeg, or jpg.");
        }
    }
}

