using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Medical.Core.Enum;
using Medical.Service.Dtos.User.AppointmentDtos;

namespace Medical.Service.Dtos.User.OrderDtos
{
	public class OrderCreateDto
	{
        public string Address { get; set; }

        public string Phone { get; set; }

    }
    public class OrderCreateDtoValidator : AbstractValidator<OrderCreateDto>
    {
        public OrderCreateDtoValidator()
        {
            RuleFor(x => x.Phone).NotNull().MinimumLength(9);

        }
    }

}

