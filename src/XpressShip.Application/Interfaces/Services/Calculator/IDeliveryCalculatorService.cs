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
        double CalculateDistance(Address originAddress, Address destinationAddress);

        double CalculateDistance(double originLatitude, double originLongitude, double destinationLatitude, double destinationLongitude);

        int CalculateDeliveryTime(int baseDays, ShipmentMethod method, ShipmentRate rate);

    }
}
