using System;
using FluentValidation;
using Medical.Core.Entities;

namespace Medical.Service.Dtos.User.AppointmentDtos
{
	public class AppointmentCreateDto
	{
        public string? AppUserId { get; set; }

        public int DoctorId { get; set; }

        public string Phone { get; set; }

        public DateTime Date { get; set; }
    }
    public class AppointmentCreateDtoValidator : AbstractValidator<AppointmentCreateDto>
    {
        public AppointmentCreateDtoValidator()
        {
            RuleFor(x => x.Date)
                .Must(date => date >= DateTime.Now)
                .WithMessage("Date cannot be in the past.");
        }
    }
}

