using AuthService.DTO;
using FluentValidation;

namespace AuthService.Validation
{
    public class LoginRequestValidation : AbstractValidator<LoginReq>
    {
        public LoginRequestValidation()
        {
            RuleFor(x => x.Email)
                .NotNull().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid Email Address");

            RuleFor(x => x.Password)
                .NotNull().WithMessage("Passsword is required")
                .MaximumLength(20).WithMessage("Email cannot be more than 10 characters")
                .MinimumLength(8).WithMessage("Password cannot be less than 8 characters");
        }
    }
}
