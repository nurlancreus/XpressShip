using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Interfaces.Services.Calculator;
using XpressShip.Domain.Entities;

namespace XpressShip.Infrastructure.Services.Calculator
{
    public class CostCalculatorService : ICostCalculatorService
    {
        public decimal CalculateShippingCost(Shipment shipment)
        {
            decimal baseCost = shipment.Rate.BaseRate;
            decimal weightCost = ICostCalculatorService.CalculateWeightCost(shipment.Weight, shipment.Rate);
            decimal volumeCost = ICostCalculatorService.CalculateSizeCost(shipment.Dimensions, shipment.Rate);

            var originAddress = shipment.OriginAddress ?? shipment.ApiClient.Address;

            var distance = IDeliveryCalculatorService.CalculateDistance(originAddress, shipment.DestinationAddress);

            decimal distanceCost = ICostCalculatorService.CalculateDistanceCost(distance, shipment.Rate);

            decimal totalCost = ICostCalculatorService.CalculateDeliveryCost(baseCost + weightCost + volumeCost + distanceCost, shipment.Method, shipment.Rate);

            var taxAppliedPrice = ITaxCalculatorService.CalculateTaxAppliedPrice(totalCost, shipment.DestinationAddress);

            return taxAppliedPrice;
        }
    }
}
