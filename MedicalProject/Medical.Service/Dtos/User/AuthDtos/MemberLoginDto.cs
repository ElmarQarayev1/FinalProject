using FluentValidation;

namespace Medical.Service.Dtos.User.AuthDtos
{
    public class MemberLoginDto
    {
      
        public string Email { get; set; }

        public string? Password { get; set; }
    }

    public class MemberLoginDtoValidator : AbstractValidator<MemberLoginDto>
    {
        public MemberLoginDtoValidator()
        {
            
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
        .NotEmpty()
        .When(x => !string.IsNullOrEmpty(x.Password))
        .WithMessage("Password is required.")
        .MinimumLength(8)
        .When(x => !string.IsNullOrEmpty(x.Password))
        .WithMessage("Password must be at least 8 characters.");


        }
    }
}
