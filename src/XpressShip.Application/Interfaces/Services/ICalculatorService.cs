using XpressShip.Domain.Entities;
using XpressShip.Domain.Validation;

namespace XpressShip.Application.Interfaces.Services
{
    public interface ICalculatorService
    {
        DateTime CalculateEstimatedDelivery(Shipment shipment);
        decimal CalculateShippingCost(Shipment shipment);
        public static int CalculateVolume(string dimensions)
        {
            IValidationService.ValidateDimensions(dimensions);

            return dimensions.Split('x').Select(int.Parse).Aggregate((x, y) => x * y);
        }

        public static double CalculateDistance(double originLatitude, double originLongitude, double destinationLatitude, double destinationLongitude)
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
        public static double CalculateDistance(Address origin, Address destination)
        {
            IValidationService.ValidateAddress(origin);
            IValidationService.ValidateAddress(destination);

            return CalculateDistance(origin.Latitude, origin.Longitude, destination.Latitude, destination.Longitude);
        }
    }
}
