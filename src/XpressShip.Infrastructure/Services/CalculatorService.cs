using Microsoft.Extensions.Options;
using XpressShip.Application.Interfaces.Services;
using XpressShip.Application.Options;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;
using XpressShip.Domain.Validation;

using Geolocation;

namespace XpressShip.Infrastructure.Services
{
    public class CalculatorService : ICalculatorService
    {
        private readonly ShippingRatesSettings _shippingRatesSettings;

        public CalculatorService(IOptions<ShippingRatesSettings> options)
        {
            _shippingRatesSettings = options.Value;
        }

        // Calculate estimated delivery date for a shipment
        public DateTime CalculateEstimatedDelivery(Shipment shipment)
        {
            var originAddress = shipment.OriginAddress ?? shipment.ApiClient.Address;

            var distance = ICalculatorService.CalculateDistance(originAddress, shipment.DestinationAddress);

            int baseDays = CalculateDefaultDeliveryTime(distance, shipment.Rate);
            int deliveryTime = ICalculatorService.CalculateDeliveryTime(baseDays, shipment.Method, shipment.Rate);

            return DateTime.Now.AddDays(deliveryTime);
        }

        // Calculate the shipping cost based on weight, dimensions, distance, and shipping method
        public decimal CalculateShippingCost(Shipment shipment)
        {
            decimal baseCost = shipment.Rate.BaseRate;
            decimal weightCost = ICalculatorService.CalculateWeightCost(shipment.Weight, shipment.Rate);
            decimal volumeCost = ICalculatorService.CalculateSizeCost(shipment.Dimensions, shipment.Rate);

            var originAddress = shipment.OriginAddress ?? shipment.ApiClient.Address;

            var distance = ICalculatorService.CalculateDistance(originAddress, shipment.DestinationAddress);

            decimal distanceCost = ICalculatorService.CalculateDistanceCost(distance, shipment.Rate);

            decimal totalCost = ICalculatorService.CalculateDeliveryCost(baseCost + weightCost + volumeCost + distanceCost, shipment.Method, shipment.Rate);

            return totalCost;
        }

        private int CalculateDefaultDeliveryTime(double distance, ShipmentRate rate)
        {
            return _shippingRatesSettings.DefaultDays + (int)Math.Ceiling(distance / _shippingRatesSettings.DefaultDistance);
        }  
    }
}
