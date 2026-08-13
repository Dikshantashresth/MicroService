using AuthService.DTO;
using FluentValidation;
using System.Data;

namespace AuthService.Validation
{
    public class LoginRequestValidation:AbstractValidator<LoginReq>
    {
       public LoginRequestValidation()
        {
            RuleFor(x => x.Email)
                .Empty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid Email Address");

            RuleFor(x => x.Password)
                .Empty().WithMessage("Passsword is required")
                .MaximumLength(20).WithMessage("Email cannot be more than 10 characters")
                .MinimumLength(8).WithMessage("Password cannot be less than 8 characters");
        }
    }
}
