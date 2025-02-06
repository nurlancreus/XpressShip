using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Extensions;

namespace XpressShip.Domain.Entities.Users
{
    public class ApplicationUser : IdentityUser, IBase
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? UserType { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenEndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DeActivatedAt { get; set; }

        protected ApplicationUser() { }

        private ApplicationUser(string firstName, string lastName, string userName, string email, string phoneNumber)
        {
            firstName.EnsureNonEmpty(nameof(firstName));
            lastName.EnsureNonEmpty(nameof(lastName));
            userName.EnsureNonEmpty(nameof(userName));
            email.EnsureNonEmpty(nameof(email));
            phoneNumber.EnsureNonEmpty(nameof(phoneNumber));

            email.EnsureValidEmail(nameof(email));

            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
            Email = email;
            PhoneNumber = phoneNumber;

            IsActive = false;
        }

        public static ApplicationUser Create(string firstName, string lastName, string userName, string email, string phoneNumber)
        {
            return new ApplicationUser(firstName, lastName, userName, email, phoneNumber);
        }

        public void Toggle()
        {
            IsActive = !IsActive;

            DeActivatedAt = !IsActive ? DateTime.UtcNow : null;
        }
    }
}
