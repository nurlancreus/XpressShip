using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XpressShip.Tests.Common.Constants
{
    public static partial class Constants
    {
        public static class OriginAddress
        {
            public const string PostalCode = "XYZ1000";
            public const string Street = "St.John";
            public const double Lat = 80;
            public const double Lon = -170;

            public const string Country = "Azerbaijan";
            public const string City = "Baku";
            public const string Code = "AZE";
            public const string PostalCodePattern = @"AZ\s\d{4}$";
            public const decimal TaxPercentage = 20;
        }

        public static class DestinationAddress
        {
            public const string PostalCode = "ZYX1000";
            public const string Street = "St.James";
            public const double Lat = 80;
            public const double Lon = -175;

            public const string Country = "Azerbaijan";
            public const string City = "Baku";
            public const string Code = "AZE";
            public const string PostalCodePattern = @"AZ\s\d{4}$";
            public const decimal TaxPercentage = 20;
        }
    }
}
