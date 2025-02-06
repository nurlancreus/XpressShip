using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Extensions;

namespace XpressShip.Domain.Entities.Users
{
    public class Admin : ApplicationUser
    {
        private Admin(string firstName, string lastName, string userName, string email, string phoneNumber)
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

        public static new Admin Create(string firstName, string lastName, string userName, string email, string phoneNumber)
        {
            return new Admin(firstName, lastName, userName, email, phoneNumber);
        }
    }
}
