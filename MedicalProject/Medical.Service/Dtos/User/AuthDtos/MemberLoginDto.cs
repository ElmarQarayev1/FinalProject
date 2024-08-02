using FluentValidation;

namespace Medical.Service.Dtos.User.AuthDtos
{
    public class MemberLoginDto
    {
      
        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class MemberLoginDtoValidator : AbstractValidator<MemberLoginDto>
    {
        public MemberLoginDtoValidator()
        {
            
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
          
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
                .Matches(@"^[A-Z]").WithMessage("Password must start with an uppercase letter.");
        }
    }
}
