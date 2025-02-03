using System.Text.RegularExpressions;
using XpressShip.Domain.Entities;
using System;
using XpressShip.Domain.Exceptions;

namespace XpressShip.Domain.Validation
{
    public partial class ValidationRules
    {
        private const string DimensionPattern = @"^\s*(\d+)\s*[xX]\s*(\d+)\s*[xX]\s*(\d+)\s*(cm|CM)?\s*$";

        public static bool ValidateDimensions(string dimensions, bool throwException = true)
        {
            bool isValid = new Regex(DimensionPattern).IsMatch(dimensions);
            if (!isValid && throwException)
                throw new XpressShipException("Invalid dimensions format. Expected format: LxWxH.");

            return isValid;
        }

        public static bool ValidateVolume(double volume, ShipmentRate rate, bool throwException = true)
        {
            bool isValid = volume > 0 && rate.MinVolume <= volume && volume <= rate.MaxVolume;
            if (!isValid && throwException)
                throw new XpressShipException("Volume is out of the allowed range.");

            return isValid;
        }

        public static bool ValidateDistance(double distance, ShipmentRate rate, bool throwException = true)
        {
            bool isValid = distance > 0 && rate.MinDistance <= distance && distance <= rate.MaxDistance;
            if (!isValid && throwException)
                throw new XpressShipException("Distance is out of the allowed range.");


            return isValid;
        }

        public static bool ValidateWeight(double weight, ShipmentRate rate, bool throwException = true)
        {
            bool isValid = weight > 0 && rate.MinWeight <= weight && weight <= rate.MaxWeight;
            if (!isValid && throwException)
                throw new XpressShipException("Weight is out of the allowed range.");

            return isValid;
        }

        public static bool IsValidLatitude(double latitude, bool throwException = true)
        {
            bool isValid = latitude is >= -90 and <= 90;

            if (!isValid && throwException)
                throw new XpressShipException($"Invalid latitude: {latitude}. Latitude must be between -90 and 90 degrees.");

            return isValid;

        }

        public static bool IsValidLongitude(double longitude, bool throwException = true)
        {

            bool isValid = longitude is >= -180 and <= 180;

            if (!isValid && throwException)
                throw new XpressShipException($"Invalid longitude: {longitude}. Longitude must be between -180 and 180 degrees.");

            return isValid;

        }
    }
}
