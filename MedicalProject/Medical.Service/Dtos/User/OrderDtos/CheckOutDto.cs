using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Medical.Service.Dtos.User.OrderDtos
{
	public class CheckOutDto
	{

      
        public List<BasketItemDto> BasketItems { get; set; } 
        public string Address { get; set; }
        public string Phone { get; set; }

    }
    public class CheckOutDtoValidator : AbstractValidator<CheckOutDto>
    {
        public CheckOutDtoValidator()
        {
           

            RuleFor(x => x.BasketItems)
                .NotNull().WithMessage("BasketItems cannot be null")
                .Must(items => items != null && items.Any()).WithMessage("BasketItems must contain at least one item")
                .DependentRules(() =>
                {
                    RuleForEach(x => x.BasketItems).SetValidator(new BasketItemDtoValidator());
                });

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address cannot be empty");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone cannot be empty")
                .Matches(@"^\+?\d{10,15}$").WithMessage("Phone must be a valid phone number");
        }
    }
}


