using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Entities.Base;
using XpressShip.Domain.Validation;

namespace XpressShip.Domain.Entities
{
    public class Address : BaseEntity
    {
        public string Street { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Guid CityId { get; set; }
        public City City { get; set; } = null!;
        public Guid? ClientId { get; set; }
        public ApiClient? Client { get; set; }
        public ICollection<Shipment> ShipmentsOrigin { get; set; } = [];
        public ICollection<Shipment> ShipmentsDestination { get; set; } = [];
        private Address()
        {

        }
        private Address(string postalCode, string street, double latitude, double longitude)
        {
            ValidationRules.IsValidLatitude(latitude);
            ValidationRules.IsValidLongitude(longitude);

            PostalCode = postalCode;
            Street = street;
            Latitude = latitude;
            Longitude = longitude;
        }

        public static Address Create(string postalCode, string street, double latitude, double longitude)
        {
            return new Address(postalCode, street, latitude, longitude);
        }

        public double CalculateDistance(Address destination)
        {
            if (destination == null) throw new ArgumentNullException(nameof(destination), "Destination cannot be null");

            return CalculateDistance(Latitude, Longitude, destination.Latitude, destination.Longitude);
        }

        private static double CalculateDistance(double originLatitude, double originLongitude, double destinationLatitude, double destinationLongitude)
        {
            const double EarthRadius = 6371.0; // Earth's radius in kilometers

            // Convert latitude and longitude from degrees to radians
            double lat1 = DegreesToRadians(originLatitude);
            double lon1 = DegreesToRadians(originLongitude);
            double lat2 = DegreesToRadians(destinationLatitude);
            double lon2 = DegreesToRadians(destinationLongitude);

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

        private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;
    }
}
