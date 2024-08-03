using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Medical.Service.Dtos.User.OrderDtos
{
	public class BasketItemDto
	{
        public int MedicineId { get; set; }

        public int Count { get; set; }
    }
    public class BasketItemDtoValidator : AbstractValidator<BasketItemDto>
    {
        public BasketItemDtoValidator()
        {
            RuleFor(x => x.MedicineId)
                .GreaterThan(0).WithMessage("MedicineId must be greater than 0");

            RuleFor(x => x.Count)
                .GreaterThan(0).WithMessage("Count must be greater than 0");

           
        }
    }
}

