using Geolocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;

namespace XpressShip.Application.Interfaces.Services.Calculator
{
    public interface IDeliveryCalculatorService
    {
        DateTime CalculateEstimatedDelivery(Shipment shipment);
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
        public static double CalculateDistanceByHaversine(double originLatitude, double originLongitude, double destinationLatitude, double destinationLongitude)
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
    }
}
