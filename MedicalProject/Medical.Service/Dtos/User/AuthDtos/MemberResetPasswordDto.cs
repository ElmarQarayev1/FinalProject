using System;
using FluentValidation;

namespace Medical.Service.Dtos.User.AuthDtos
{
    public class MemberResetPasswordDto
    {
        public string NewPassword { get; set; }

        public string ConfirmNewPassword { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }
    }

    public class MemberResetPasswordDtoValidator : AbstractValidator<MemberResetPasswordDto>
    {
        public MemberResetPasswordDtoValidator()
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MaximumLength(50).WithMessage("New password must be less than 50 characters.")
                .MinimumLength(8).WithMessage("New password must be at least 8 characters.");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage("Confirm new password is required.")
                .MaximumLength(50).WithMessage("Confirm new password must be less than 50 characters.")
                .MinimumLength(8).WithMessage("Confirm new password must be at least 8 characters.");

            RuleFor(x => x)
                .Must(x => x.NewPassword == x.ConfirmNewPassword)
                .WithMessage("New password and confirm password must be the same.");
        }
    }
}
