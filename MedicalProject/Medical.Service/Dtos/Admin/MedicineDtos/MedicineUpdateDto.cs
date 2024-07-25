using System;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Medical.Service.Dtos.Admin.MedicineDtos
{
	public class MedicineUpdateDto
	{
      
        public string Name { get; set; }

        public double Price { get; set; }

        public string Desc { get; set; }

        public int CategoryId { get; set; }

        public List<IFormFile> Files { get; set; }

         public List<int>? ExistPictureIds { get; set; } = new List<int>();

    }
    public class MedicineUpdateDtoValidator : AbstractValidator<MedicineUpdateDto>
    {
        public MedicineUpdateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(35).MinimumLength(2);
            RuleFor(x => x.Price).NotEmpty().GreaterThan(0);
            RuleFor(x => x.Desc).NotEmpty().MaximumLength(200);


            RuleForEach(x => x.Files)
                 .Must(file => file.Length <= 2 * 1024 * 1024)
                .WithMessage("Each file must be less than or equal to 2MB.")
                .Must(file => new[] { "image/png", "image/jpeg" }.Contains(file.ContentType))
                .WithMessage("Each file type must be png, jpeg, or jpg.");

        }

    }
}

