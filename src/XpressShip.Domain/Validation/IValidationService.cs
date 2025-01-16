using System.Text.RegularExpressions;
using XpressShip.Domain.Entities;
using System;
using XpressShip.Domain.Exceptions;

namespace XpressShip.Domain.Validation
{
    public partial interface IValidationService
    {
        private const string DimensionPattern = @"^\s*(\d+)\s*[xX]\s*(\d+)\s*[xX]\s*(\d+)\s*(cm|CM)?\s*$";
        public static Dictionary<string, (string[] Cities, string PostalCodePattern)> ValidCountries { get; set; } = new()
            {
                { "Azerbaijan", (["Baku"], @"AZ\s\d{4}$") },
                { "Russia", (["Moscow"], @"^\d{6}$") },
                { "Georgia", (["Tbilisi"], @"^\d{4}$") },
                { "Iran", (["Tabriz", "Tehran"], @"^\d{10}$") },
                { "Turkey", (["Ankara", "Istanbul", "Izmir"], @"^\d{5}$") }
            };

        // Validate dimensions in the format LxWxH
        public static bool ValidateDimensions(string dimensions, bool throwException = true)
        {
            bool isValid = DimensionsPatternRegex().IsMatch(dimensions);
            if (!isValid && throwException)
            {
                throw new ValidationException("Invalid dimensions format. Expected format: LxWxH.");
            }
            return isValid;
        }

        // Validate that the volume is within allowed range
        public static bool ValidateVolume(double volume, ShipmentRate rate, bool throwException = true)
        {
            bool isValid = volume > 0 && rate.MinVolume <= volume && volume <= rate.MaxVolume;
            if (!isValid && throwException)
            {
                throw new ValidationException("Volume is out of the allowed range.");
            }
            return isValid;
        }

        // Validate that the distance is within allowed range
        public static bool ValidateDistance(double distance, ShipmentRate rate, bool throwException = true)
        {
            bool isValid = distance > 0 && rate.MinDistance <= distance && distance <= rate.MaxDistance;
            if (!isValid && throwException)
            {
                throw new ValidationException("Distance is out of the allowed range.");
            }
            return isValid;
        }

        // Validate that the weight is within allowed range
        public static bool ValidateWeigth(double weight, ShipmentRate rate, bool throwException = true)
        {
            bool isValid = weight > 0 && rate.MinWeight <= weight && weight <= rate.MaxWeight;
            if (!isValid && throwException)
            {
                throw new ValidationException("Weight is out of the allowed range.");
            }
            return isValid;
        }
        public static bool ValidateAddress(string country, string city, string postalCode, string street, bool throwException = true)
        {
            if (string.IsNullOrWhiteSpace(country) ||
                string.IsNullOrWhiteSpace(city) ||
                string.IsNullOrWhiteSpace(postalCode) ||
                string.IsNullOrWhiteSpace(street))
            {
                if (throwException)
                {
                    throw new ValidationException("Invalid address details.");
                }
                return false;
            }

            if (!ValidCountries.TryGetValue(country, out (string[] Cities, string PostalCodePattern) countryInfo))
            {
                if (throwException)
                {
                    throw new ValidationException("Country is not supported.");
                }
                return false;
            }

            bool isCityValid = countryInfo.Cities.Contains(city);
            bool isPostalCodeValid = new Regex(countryInfo.PostalCodePattern).IsMatch(postalCode);

            if (!(isCityValid && isPostalCodeValid) && throwException)
            {
                throw new ValidationException("City or postal code is invalid for the specified country.");
            }

            return isCityValid && isPostalCodeValid;
        }


        // Validate address fields including format, latitude, and longitude
        public static bool ValidateAddress(Address address, bool throwException = true)
        {
            if (address == null ||
                string.IsNullOrWhiteSpace(address.Street) ||
                string.IsNullOrWhiteSpace(address.City) ||
                string.IsNullOrWhiteSpace(address.PostalCode) ||
                address.Latitude < -90 || address.Latitude > 90 ||
                address.Longitude < -180 || address.Longitude > 180)
            {
                if (throwException)
                {
                    throw new ValidationException("Invalid address details.");
                }
                return false;
            }

            if (!ValidCountries.TryGetValue(address.Country, out (string[] Cities, string PostalCodePattern) countryInfo))
            {
                if (throwException)
                {
                    throw new ValidationException("Country is not supported.");
                }
                return false;
            }

            bool isCityValid = countryInfo.Cities.Contains(address.City);
            bool isPostalCodeValid = new Regex(countryInfo.PostalCodePattern).IsMatch(address.PostalCode);

            if (!(isCityValid && isPostalCodeValid) && throwException)
            {
                throw new ValidationException("City or postal code is invalid for the specified country.");
            }

            return isCityValid && isPostalCodeValid;
        }

        // Improved regex for dimension validation
        [GeneratedRegex(DimensionPattern)]
        private static partial Regex DimensionsPatternRegex();
    }
}
