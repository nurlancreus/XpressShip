using Geolocation;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;
using XpressShip.Domain.Validation;

namespace XpressShip.Application.Interfaces.Services
{
    public interface ICalculatorService
    {
        DateTime CalculateEstimatedDelivery(Shipment shipment);
        decimal CalculateShippingCost(Shipment shipment);
        public static double CalculateDistance(Address originAddress, Address destinationAddress)
        {
            return CalculateDistance(originAddress.Latitude, originAddress.Longitude, destinationAddress.Latitude, destinationAddress.Longitude);
        }

        public static double CalculateDistance(double originLatitude, double originLongitude, double destinationLatitude, double destinationLongitude)
        {
            var origin = new Coordinate(originLatitude, originLongitude);
            var destination = new Coordinate(destinationLatitude, destinationLongitude);

            var distance = GeoCalculator.GetDistance(origin, destination, 2, DistanceUnit.Kilometers); // in km

            return distance;
        }

        public static int CalculateDeliveryTime(int baseDays, ShipmentMethod method, ShipmentRate rate)
        {
            return method switch
            {
                ShipmentMethod.Standard => baseDays,
                ShipmentMethod.Express => (int)Math.Ceiling(baseDays * rate.ExpressDeliveryTimeMultiplier),
                ShipmentMethod.Overnight => (int)Math.Ceiling(baseDays * rate.OvernightDeliveryTimeMultiplier),
                _ => baseDays
            };
        }

        public static decimal CalculateDeliveryCost(decimal subtotal, ShipmentMethod method, ShipmentRate rate)
        {
            return method switch
            {
                ShipmentMethod.Standard => subtotal,
                ShipmentMethod.Express => subtotal * (decimal)rate.ExpressRateMultiplier,
                ShipmentMethod.Overnight => subtotal * (decimal)rate.OvernightRateMultiplier,
                _ => subtotal
            };
        }

        public static decimal CalculateWeightCost(double weight, ShipmentRate rate)
        {
            IValidationService.ValidateWeigth(weight, rate);

            return (decimal)(weight * rate.BaseRateForKg);
        }

        public static decimal CalculateDistanceCost(double distance, ShipmentRate rate)
        {
            IValidationService.ValidateDistance(distance, rate);

            return (decimal)(distance * rate.BaseRateForKm);
        }

        public static decimal CalculateSizeCost(string dimensions, ShipmentRate rate)
        {
            int volume = CalculateVolume(dimensions);

            return CalculateSizeCost(volume, rate);
        }

        public static decimal CalculateSizeCost(int volume, ShipmentRate rate)
        {
            IValidationService.ValidateVolume(volume, rate);

            return volume * (decimal)rate.BaseRateForVolume;
        }

        public static int CalculateVolume(string dimensions)
        {
            IValidationService.ValidateDimensions(dimensions);

            return dimensions.Split('x').Select(int.Parse).Aggregate((x, y) => x * y);
        }

        public static double CalculateHaversine(double originLatitude, double originLongitude, double destinationLatitude, double destinationLongitude)
        {
            const double EarthRadius = 6371.0; // Earth's radius in kilometers

            // Convert latitude and longitude from degrees to radians
            double lat1 = originLatitude * (Math.PI / 180.0);
            double lon1 = originLongitude * (Math.PI / 180.0);
            double lat2 = destinationLatitude * (Math.PI / 180.0);
            double lon2 = destinationLongitude * (Math.PI / 180.0);

            // Difference in coordinates
            double dLat = lat2 - lat1;
            double dLon = lon2 - lon1;

            // Haversine formula
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            // Calculate the distance
            double distance = EarthRadius * c; // Distance in kilometers
            return distance;
        }
        public static double CalculateHaversine(Address origin, Address destination)
        {
            IValidationService.ValidateAddress(origin);
            IValidationService.ValidateAddress(destination);

            return CalculateHaversine(origin.Latitude, origin.Longitude, destination.Latitude, destination.Longitude);
        }
    }
}
