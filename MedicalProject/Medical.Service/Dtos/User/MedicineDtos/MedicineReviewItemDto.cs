using System;
using FluentValidation;
using Medical.Core.Enum;
using Medical.Service.Dtos.User.MedicineDtos;


namespace Medical.Service.Dtos.User.MedicineDtos
{
	public class MedicineReviewItemDto
	{
       

        public int? MedicineId { get; set; }

        public string Text { get; set; }

        public byte Rate { get; set; }
    }
    public class MedicineReviewItemDtoValidator : AbstractValidator<MedicineReviewItemDto>
    {
        public MedicineReviewItemDtoValidator()
        {
            RuleFor(x => x.Rate)
                .GreaterThanOrEqualTo((byte)1)
                .LessThanOrEqualTo((byte)5)
                .WithMessage("Rate must be between 1 and 5.");
        }
    }


}

