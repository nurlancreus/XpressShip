using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Interfaces.Services.Calculator;
using XpressShip.Application.Options;
using XpressShip.Domain.Entities;

namespace XpressShip.Infrastructure.Services.Calculator
{
    public class DeliveryCalculatorService : IDeliveryCalculatorService
    {
        private const int defaultDays = 5;

        public DateTime CalculateEstimatedDelivery(Shipment shipment)
        {
            var originAddress = shipment.OriginAddress ?? shipment.ApiClient.Address;

            var distance = IDeliveryCalculatorService.CalculateDistance(originAddress, shipment.DestinationAddress);

            int baseDays = CalculateDefaultDeliveryTime(distance, shipment.Rate);
            int deliveryTime = IDeliveryCalculatorService.CalculateDeliveryTime(baseDays, shipment.Method, shipment.Rate);

            return DateTime.Now.AddDays(deliveryTime);
        }

        private static int CalculateDefaultDeliveryTime(double distance, ShipmentRate rate)
        {
            return defaultDays + (int)Math.Ceiling(distance /100);
        }
    }
}
