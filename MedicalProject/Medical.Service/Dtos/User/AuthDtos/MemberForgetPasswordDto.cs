using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Medical.Service.Dtos.User.AuthDtos
{
	public class MemberForgetPasswordDto
	{
        
        public string Email { get; set; }
    }
    public class MemberForgetPasswordDtoValidator : AbstractValidator<MemberForgetPasswordDto>
    {
        public MemberForgetPasswordDtoValidator()
        {

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.").MinimumLength(3).MaximumLength(100)
                .EmailAddress().WithMessage("Invalid email format.");

           
        }
    }
}

