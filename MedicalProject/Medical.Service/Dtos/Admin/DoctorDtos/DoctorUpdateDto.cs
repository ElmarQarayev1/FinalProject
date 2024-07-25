using System;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Medical.Service.Dtos.Admin.DoctorDtos
{
	public class DoctorUpdateDto
	{
        public string FullName { get; set; }

        public string Position { get; set; }

        public int DepartmentId { get; set; }

        public string Desc { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public IFormFile? File { get; set; }

        public string? TwitterUrl { get; set; }

        public string? InstagramUrl { get; set; }

        public string? VimeoUrl { get; set; }

        public string? BehanceUrl { get; set; }

        public double ResilienceSkil { get; set; }

        public double EthicSkil { get; set; }

        public double CompassionSkil { get; set; }
    }
    public class DoctorUpdateDtoValidator : AbstractValidator<DoctorUpdateDto>
    {
        public DoctorUpdateDtoValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(40).MinimumLength(2);

            RuleFor(x => x.Desc).NotEmpty().MaximumLength(400).MinimumLength(3);

            RuleFor(x => x.Position).NotEmpty().MaximumLength(100).MinimumLength(1);

            RuleFor(x => x.Address).NotEmpty().MaximumLength(400).MinimumLength(3);

            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(60).MinimumLength(5);

            RuleFor(x => x.Phone).NotEmpty().MaximumLength(30).MinimumLength(3);

            RuleFor(x => x.ResilienceSkil).NotEmpty().GreaterThan(0);

            RuleFor(x => x.EthicSkil).NotEmpty().GreaterThan(0);

            RuleFor(x => x.CompassionSkil).NotEmpty().GreaterThan(0);

            RuleFor(x => x.File)
                .Must(file => file == null || file.Length <= 2 * 1024 * 1024)
                .WithMessage("File must be less than or equal to 2MB.")
                .Must(file => file == null || new[] { "image/png", "image/jpeg" }.Contains(file.ContentType))
                .WithMessage("File type must be png, jpeg, or jpg.");
        }
    }
}

