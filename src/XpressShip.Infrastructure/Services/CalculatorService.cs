using Microsoft.Extensions.Options;
using XpressShip.Application.Interfaces.Services;
using XpressShip.Application.Options;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;
using XpressShip.Domain.Validation;

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
            var distance = ICalculatorService.CalculateDistance(shipment.OriginAddress, shipment.DestinationAddress);

            int baseDays = CalculateDefaultDeliveryTime(distance, shipment.Rate);
            int deliveryTime = CalculateDeliveryTime(baseDays, shipment.Method, shipment.Rate);

            return DateTime.Now.AddDays(deliveryTime);
        }

        // Calculate the shipping cost based on weight, dimensions, distance, and shipping method
        public decimal CalculateShippingCost(Shipment shipment)
        {
            decimal baseCost = shipment.Rate.BaseRate;
            decimal weightCost = CalculateWeightCost(shipment.Weight, shipment.Rate);
            decimal volumeCost = CalculateSizeCost(shipment.Dimensions, shipment.Rate);

            var originAddress = shipment.OriginAddress ?? shipment.ApiClient.Address;
            var distance = ICalculatorService.CalculateDistance(originAddress, shipment.DestinationAddress);
            decimal distanceCost = CalculateDistanceCost(distance, shipment.Rate);

            decimal totalCost = CalculateDeliveryCost(baseCost + weightCost + volumeCost + distanceCost, shipment.Method, shipment.Rate);

            return totalCost;
        }

        private int CalculateDefaultDeliveryTime(double distance, ShipmentRate rate)
        {
            return _shippingRatesSettings.DefaultDays + (int)Math.Ceiling(distance / _shippingRatesSettings.DefaultDistance);
        }

        private static int CalculateDeliveryTime(int baseDays, ShipmentMethod method, ShipmentRate rate)
        {
            return method switch
            {
                ShipmentMethod.Standard => baseDays,
                ShipmentMethod.Express => (int)Math.Ceiling(baseDays * rate.ExpressDeliveryTimeMultiplier),
                ShipmentMethod.Overnight => (int)Math.Ceiling(baseDays * rate.OvernightDeliveryTimeMultiplier),
                _ => baseDays
            };
        }

        private static decimal CalculateDeliveryCost(decimal subtotal, ShipmentMethod method, ShipmentRate rate)
        {
            return method switch
            {
                ShipmentMethod.Standard => subtotal,
                ShipmentMethod.Express => subtotal * (decimal)rate.ExpressRateMultiplier,
                ShipmentMethod.Overnight => subtotal * (decimal)rate.OvernightRateMultiplier,
                _ => subtotal
            };
        }

        private static decimal CalculateWeightCost(double weight, ShipmentRate rate)
        {
            IValidationService.ValidateWeigth(weight, rate);

            return (decimal)(weight * rate.BaseRateForKg);
        }

        private static decimal CalculateDistanceCost(double distance, ShipmentRate rate)
        {
            IValidationService.ValidateDistance(distance, rate);

            return (decimal)(distance * rate.BaseRateForKm);
        }

        private static decimal CalculateSizeCost(string dimensions, ShipmentRate rate)
        {
            int volume = ICalculatorService.CalculateVolume(dimensions);

            return CalculateSizeCost(volume, rate);
        }

        private static decimal CalculateSizeCost(int volume, ShipmentRate rate)
        {
            IValidationService.ValidateVolume(volume, rate);

            return volume * (decimal)rate.BaseRateForVolume;
        }
    }
}
