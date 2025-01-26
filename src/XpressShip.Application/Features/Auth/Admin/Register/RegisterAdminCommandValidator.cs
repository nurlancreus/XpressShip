using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain;
using XpressShip.Domain.Entities.Users;

namespace XpressShip.Application.Features.Auth.Admin.Register
{
    public class RegisterAdminCommandValidator : AbstractValidator<RegisterAdminCommand>
    {
        public RegisterAdminCommandValidator(UserManager<ApplicationUser> userManager)
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required.")
                .MaximumLength(Constants.FirstNameMaxLength)
                .WithMessage($"First name must not exceed {Constants.FirstNameMaxLength} characters.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required.")
                .MaximumLength(Constants.LastNameMaxLength)
                .WithMessage($"Last name must not exceed {Constants.LastNameMaxLength} characters.");

            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("Username is required.")
                .MaximumLength(Constants.UserNameMaxLength)
                .WithMessage($"Username must not exceed {Constants.UserNameMaxLength} characters.")
                .MustAsync(async (username, cancellationToken) =>
                {
                    var user = await userManager.FindByNameAsync(username);
                    return user == null;
                })
                .WithMessage("This username is already in use.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email address.")
                .MustAsync(async (email, cancellationToken) =>
                {
                    var user = await userManager.FindByEmailAsync(email);
                    return user == null;
                })
                .WithMessage("This email is already registered.");

            RuleFor(x => x.Phone)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Invalid phone number format.")
                .MustAsync(async (phone, cancellationToken) =>
                {
                    var user = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone, cancellationToken);

                    return user == null;
                })
                .WithMessage("This phone number is already registered.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(Constants.PasswordMinLength)
                .WithMessage($"Password must be at least {Constants.PasswordMinLength} characters long.")
                .MaximumLength(Constants.PasswordMaxLength)
                .WithMessage($"Password must not exceed {Constants.PasswordMaxLength} characters.")
                .Matches(@"[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]")
                .WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]")
                .WithMessage("Password must contain at least one digit.")
                .Matches(@"[\W_]")
                .WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage("Confirm Password is required.")
                .Equal(x => x.Password)
                .WithMessage("Passwords do not match.");
        }
    }
}
