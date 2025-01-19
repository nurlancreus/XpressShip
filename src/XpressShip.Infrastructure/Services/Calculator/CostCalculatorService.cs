using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Interfaces.Services.Calculator;
using XpressShip.Application.Utilities;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;
using XpressShip.Domain.Validation;

namespace XpressShip.Infrastructure.Services.Calculator
{
    public class CostCalculatorService : ICostCalculatorService
    {
        private readonly IDeliveryCalculatorService _deliveryCalculatorService;
        private readonly ITaxCalculatorService _taxCalculatorService;

        public CostCalculatorService(IDeliveryCalculatorService deliveryCalculatorService, ITaxCalculatorService taxCalculatorService)
        {
            _deliveryCalculatorService = deliveryCalculatorService;
            _taxCalculatorService = taxCalculatorService;
        }

        public decimal CalculateShippingCost(Shipment shipment)
        {
            decimal baseCost = shipment.Rate.BaseRate;
            decimal weightCost = CalculateWeightCost(shipment.Weight, shipment.Rate);
            decimal volumeCost = CalculateSizeCost(shipment.Dimensions, shipment.Rate);

            var originAddress = shipment.OriginAddress ?? shipment.ApiClient?.Address;

            var distance = _deliveryCalculatorService.CalculateDistance(originAddress!, shipment.DestinationAddress);

            decimal distanceCost = CalculateDistanceCost(distance, shipment.Rate);

            decimal totalCost = CalculateDeliveryCost(baseCost + weightCost + volumeCost + distanceCost, shipment.Method, shipment.Rate);

            var taxAppliedPrice = _taxCalculatorService.CalculateTaxAppliedPrice(totalCost, shipment.DestinationAddress);

            return taxAppliedPrice;
        }

        public decimal CalculateDeliveryCost(decimal subtotal, ShipmentMethod method, ShipmentRate rate)
        {
            return method switch
            {
                ShipmentMethod.Standard => subtotal,
                ShipmentMethod.Express => subtotal * (decimal)rate.ExpressRateMultiplier,
                ShipmentMethod.Overnight => subtotal * (decimal)rate.OvernightRateMultiplier,
                _ => subtotal
            };
        }

        public decimal CalculateWeightCost(double weight, ShipmentRate rate)
        {
            ValidationRules.ValidateWeight(weight, rate);

            return (decimal)(weight * rate.BaseRateForKg);
        }

        public decimal CalculateDistanceCost(double distance, ShipmentRate rate)
        {
            ValidationRules.ValidateDistance(distance, rate);

            return (decimal)(distance * rate.BaseRateForKm);
        }

        public decimal CalculateSizeCost(string dimensions, ShipmentRate rate)
        {
            int volume = CalculatorUtility.CalculateVolume(dimensions);

            return CalculateSizeCost(volume, rate);
        }

        public decimal CalculateSizeCost(int volume, ShipmentRate rate)
        {
            ValidationRules.ValidateVolume(volume, rate);

            return volume * (decimal)rate.BaseRateForVolume;
        }
    }
}
