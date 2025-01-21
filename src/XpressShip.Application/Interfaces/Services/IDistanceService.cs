using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Entities;

namespace XpressShip.Application.Interfaces.Services
{
    public interface IDistanceService
    {
        double CalculateDistance(Address origin, Address destination);
        double CalculateDistance(double originLat, double originLon, double destLat, double destLon);
    }
}
