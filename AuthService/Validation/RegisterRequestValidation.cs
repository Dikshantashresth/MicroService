using AuthService.DTO;
using AuthService.Model;
using FluentValidation;
using Microsoft.AspNetCore.Identity.Data;

namespace AuthService.Validation
{
    public class RegisterRequestValidation:AbstractValidator<User>
    {
        public RegisterRequestValidation()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(3).WithMessage("Name must be more than 3 characters.")
                .MaximumLength(10).WithMessage("Name must be less than 20 characters.");

            
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be more than 8 characters")
                .MaximumLength(20).WithMessage("Passowrd must be less than 20 characters")
                .Matches("[A-Z]").WithMessage("Password should contain a lower case character")
                .Matches("[0-9]").WithMessage("Password should contain a number")
                .Matches("[a-z]").WithMessage("Password should contain a lower case character");

        }
    }
}
