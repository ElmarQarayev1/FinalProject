using System;
using FluentValidation;
using Medical.Service.Dtos.Admin.ServiceDtos;
using Microsoft.AspNetCore.Http;

namespace Medical.Service.Dtos.Admin.DoctorDtos
{
	public class DoctorCreateDto
	{
        public string FullName { get; set; }

        public string Position { get; set; }

        public int DepartmentId { get; set; }

        public string Desc { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public IFormFile File { get; set; }

        public string? TwitterUrl { get; set; }

        public string? InstagramUrl { get; set; }

        public string? VimeoUrl { get; set; }

        public string? BehanceUrl { get; set; }

        public double ResilienceSkil { get; set; }

        public double EthicSkil { get; set; }

        public double CompassionSkil { get; set; }
    }
    public class DoctorCreateDtoValidator : AbstractValidator<DoctorCreateDto>
    {
        private static readonly string UrlPattern = @"^(https?:\/\/)?(www\.)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}(\/\S*)?$";

        public DoctorCreateDtoValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(40).MinimumLength(2);

            RuleFor(x => x.Desc).NotEmpty().MaximumLength(400).MinimumLength(3);

            RuleFor(x => x.Position).NotEmpty().MaximumLength(100).MinimumLength(1);

            RuleFor(x => x.Address).NotEmpty().MaximumLength(400).MinimumLength(3);

            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(60).MinimumLength(5);

            RuleFor(x => x.Phone).NotEmpty().MaximumLength(30).MinimumLength(3);

            RuleFor(x => x.ResilienceSkil)
            .NotEmpty()
            .GreaterThan(0)
            .LessThan(100);

            RuleFor(x => x.EthicSkil)
                .NotEmpty()
                .GreaterThan(0)
                .LessThan(100);

            RuleFor(x => x.CompassionSkil)
                .NotEmpty()
                .GreaterThan(0)
                .LessThan(100);

            RuleFor(x => x.TwitterUrl)
               .Matches(UrlPattern).When(x => !string.IsNullOrEmpty(x.TwitterUrl))
               .WithMessage("Twitter URL must be a valid URL.");

            RuleFor(x => x.InstagramUrl)
              .Matches(UrlPattern).When(x => !string.IsNullOrEmpty(x.InstagramUrl))
              .WithMessage("Instagram URL must be a valid URL.");

            RuleFor(x => x.VimeoUrl)
                .Matches(UrlPattern).When(x => !string.IsNullOrEmpty(x.VimeoUrl))
                .WithMessage("Vimeo URL must be a valid URL.");

            RuleFor(x => x.BehanceUrl)
                .Matches(UrlPattern).When(x => !string.IsNullOrEmpty(x.BehanceUrl))
                .WithMessage("Behance URL must be a valid URL.");

            RuleFor(x => x.File)
                .Must(file => file == null || file.Length <= 2 * 1024 * 1024)
                .WithMessage("File must be less than or equal to 2MB.")
                .Must(file => file == null || new[] { "image/png", "image/jpeg" }.Contains(file.ContentType))
                .WithMessage("File type must be png, jpeg, or jpg.");
        }
    }
}

