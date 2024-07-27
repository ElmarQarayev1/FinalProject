using System;
using FluentValidation;
using Medical.Service.Dtos.Admin.CategoryDtos;

namespace Medical.Service.Dtos.Admin.AuthDtos
{
	public class SuperAdminCreateAdminDto
	{
        public string UserName { get; set; }

        public string Password { get; set; }
       
    }

    public class SuperAdminCreateDtoValidator : AbstractValidator<SuperAdminCreateAdminDto>
    {
        public SuperAdminCreateDtoValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().MaximumLength(35).MinimumLength(2);

            RuleFor(x => x.Password).NotEmpty().MaximumLength(35).MinimumLength(2);

        }
    }


}

