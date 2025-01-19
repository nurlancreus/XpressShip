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
        decimal CalculateDeliveryCost(decimal subtotal, ShipmentMethod method, ShipmentRate rate);

        decimal CalculateWeightCost(double weight, ShipmentRate rate);

        decimal CalculateDistanceCost(double distance, ShipmentRate rate);

        decimal CalculateSizeCost(string dimensions, ShipmentRate rate);

        decimal CalculateSizeCost(int volume, ShipmentRate rate);

    }
}
