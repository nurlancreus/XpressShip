using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XpressShip.Domain
{
    public static class Constants
    {
        // Api Client constants
        public const int ApiClientCompanyNameMaxLength = 75;

        // Account constants
        public const int FirstNameMaxLength = 50;
        public const int LastNameMaxLength = 50;
        public const int UserNameMaxLength = 50;

        public const int PasswordMinLength = 8;
        public const int PasswordMaxLength = 50;
    }
}
