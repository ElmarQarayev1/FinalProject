using System;
using FluentValidation;

namespace Medical.Service.Dtos.Admin.CategoryDtos
{
	public class CategoryCreateDto
	{
        public string Name { get; set; }
    }

    public class CategoryCreateDtoValidator : AbstractValidator<CategoryCreateDto>
    {
        public CategoryCreateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(35).MinimumLength(2);
           
        }
    }
}

