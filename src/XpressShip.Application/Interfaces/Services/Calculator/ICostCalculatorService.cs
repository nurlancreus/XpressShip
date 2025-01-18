using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;
using XpressShip.Domain.Validation;

namespace XpressShip.Application.Interfaces.Services.Calculator
{
    public interface ICostCalculatorService
    {
        decimal CalculateShippingCost(Shipment shipment);
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
            int volume = ICalculatorService.CalculateVolume(dimensions);

            return CalculateSizeCost(volume, rate);
        }

        public static decimal CalculateSizeCost(int volume, ShipmentRate rate)
        {
            IValidationService.ValidateVolume(volume, rate);

            return volume * (decimal)rate.BaseRateForVolume;
        }

    }
}
