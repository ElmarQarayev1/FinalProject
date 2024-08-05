using System;
using FluentValidation;

namespace Medical.Service.Dtos.User.AuthDtos
{
    public class MemberProfileEditDto
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public string? CurrentPassword { get; set; }

        public string? NewPassword { get; set; }

        public string? ConfirmNewPassword { get; set; }

        public bool HasPassword { get; set; }

        public bool IsGoogleUser { get; set; }
    }

    public class MemberProfileEditDtoValidator : AbstractValidator<MemberProfileEditDto>
    {
        public MemberProfileEditDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("User name is required.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.");

            RuleFor(x => x.NewPassword)
                .Equal(x => x.ConfirmNewPassword).When(x => !string.IsNullOrEmpty(x.NewPassword))
                .WithMessage("New password and confirm password do not match.");
        }
    }
}
