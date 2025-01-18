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
                throw new ValidationException("Invalid dimensions format. Expected format: LxWxH.");

            return isValid;
        }

        // Validate that the volume is within allowed range
        public static bool ValidateVolume(double volume, ShipmentRate rate, bool throwException = true)
        {
            bool isValid = volume > 0 && rate.MinVolume <= volume && volume <= rate.MaxVolume;
            if (!isValid && throwException)
                throw new ValidationException("Volume is out of the allowed range.");

            return isValid;
        }

        // Validate that the distance is within allowed range
        public static bool ValidateDistance(double distance, ShipmentRate rate, bool throwException = true)
        {
            bool isValid = distance > 0 && rate.MinDistance <= distance && distance <= rate.MaxDistance;
            if (!isValid && throwException)
                throw new ValidationException("Distance is out of the allowed range.");


            return isValid;
        }

        // Validate that the weight is within allowed range
        public static bool ValidateWeigth(double weight, ShipmentRate rate, bool throwException = true)
        {
            bool isValid = weight > 0 && rate.MinWeight <= weight && weight <= rate.MaxWeight;
            if (!isValid && throwException)
                throw new ValidationException("Weight is out of the allowed range.");

            return isValid;
        }

        // Improved regex for dimension validation
        [GeneratedRegex(DimensionPattern)]
        private static partial Regex DimensionsPatternRegex();
    }
}
