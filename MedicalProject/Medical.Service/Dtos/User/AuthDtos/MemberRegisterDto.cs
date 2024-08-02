using FluentValidation;

namespace Medical.Service.Dtos.Admin.AuthDtos
{
    public class MemberRegisterDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class MemberRegisterDtoValidator : AbstractValidator<MemberRegisterDto>
    {
        public MemberRegisterDtoValidator()
        {
            
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required.")
                .Length(2, 35).WithMessage("UserName must be between 2 and 35 characters.");

          
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

          
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FullName is required.")
                .MaximumLength(100).WithMessage("FullName must be less than 100 characters.");

          
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches(@"^[A-Z]").WithMessage("Password must start with an uppercase letter.");

           
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("ConfirmPassword is required.")
                .Equal(x => x.Password).WithMessage("Passwords do not match.");
        }
    }
}
