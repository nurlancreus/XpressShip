using Geolocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Interfaces.Services.Calculator;
using XpressShip.Application.Options;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;

namespace XpressShip.Infrastructure.Services.Calculator
{
    public class DeliveryCalculatorService : IDeliveryCalculatorService
    {
        private const int defaultDays = 5;
 

        public DateTime CalculateEstimatedDelivery(Shipment shipment)
        {
            var originAddress = shipment.OriginAddress ?? shipment.ApiClient?.Address;

            var distance = CalculateDistance(originAddress!, shipment.DestinationAddress);

            int baseDays = CalculateDefaultDeliveryTime(distance, shipment.Rate);
            int deliveryTime = CalculateDeliveryTime(baseDays, shipment.Method, shipment.Rate);

            return DateTime.Now.AddDays(deliveryTime);
        }

        public double CalculateDistance(Address originAddress, Address destinationAddress)
        {
            return CalculateDistance(originAddress.Latitude, originAddress.Longitude, destinationAddress.Latitude, destinationAddress.Longitude);
        }

        public double CalculateDistance(double originLatitude, double originLongitude, double destinationLatitude, double destinationLongitude)
        {
            var origin = new Coordinate(originLatitude, originLongitude);
            var destination = new Coordinate(destinationLatitude, destinationLongitude);

            var distance = GeoCalculator.GetDistance(origin, destination, 2, DistanceUnit.Kilometers); // in km

            return distance;
        }

        public int CalculateDeliveryTime(int baseDays, ShipmentMethod method, ShipmentRate rate)
        {
            return method switch
            {
                ShipmentMethod.Standard => baseDays,
                ShipmentMethod.Express => (int)Math.Ceiling(baseDays * rate.ExpressDeliveryTimeMultiplier),
                ShipmentMethod.Overnight => (int)Math.Ceiling(baseDays * rate.OvernightDeliveryTimeMultiplier),
                _ => baseDays
            };
        }

        private static int CalculateDefaultDeliveryTime(double distance, ShipmentRate rate)
        {
            return defaultDays + (int)Math.Ceiling(distance /100);
        }
    }
}
