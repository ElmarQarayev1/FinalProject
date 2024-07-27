using FluentValidation;

namespace Medical.Service.Dtos.Admin.AuthDtos
{
    public class AdminUpdateDto
    {
        public string UserName { get; set; }

        public string? CurrentPassword { get; set; }

        public string? NewPassword { get; set; }

        public string? ConfirmPassword { get; set; }
    }

    public class AdminUpdateDtoValidator : AbstractValidator<AdminUpdateDto>
    {
        public AdminUpdateDtoValidator()
        {
            RuleFor(x => x.UserName)
                
                .MaximumLength(50)
                .MinimumLength(2);

            RuleFor(x => x.CurrentPassword)
                
                .MaximumLength(50)
                .MinimumLength(8);

            RuleFor(x => x.NewPassword)
               
                .MaximumLength(50)
                .MinimumLength(8);

            RuleFor(x => x.ConfirmPassword)
               
                .MaximumLength(50)
                .MinimumLength(8);

             RuleFor(x => x)
                .Must(x => x.NewPassword == x.ConfirmPassword)
                .WithMessage("New password and confirm password must be the same.");
        }
    }
}
