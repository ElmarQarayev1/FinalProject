using System;
using FluentValidation;
using Medical.Service.Dtos.Admin.FeatureDtos;
using Microsoft.AspNetCore.Http;

namespace Medical.Service.Dtos.Admin.FeedDtos
{
	public class FeedUpdateDto
	{
        public string Name { get; set; }

        public string Desc { get; set; }

        public DateTime Date { get; set; }

        public IFormFile? File { get; set; }
    }
    public class FeedUpdateDtoValidator : AbstractValidator<FeedUpdateDto>
    {
        public FeedUpdateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(65).MinimumLength(2);

            RuleFor(x => x.Desc).NotEmpty().MaximumLength(200).MinimumLength(3);


            RuleFor(x => x.File)
                .Must(file => file == null || file.Length <= 2 * 1024 * 1024)
                .WithMessage("File must be less than or equal to 2MB.")
                .Must(file => file == null || new[] { "image/png", "image/jpeg" }.Contains(file.ContentType))
                .WithMessage("File type must be png, jpeg, or jpg.");

            RuleFor(x => x.Date)
               .Must(date => date >= DateTime.Today)
               .WithMessage("Date cannot be in the past.");
        }
    }
}

