using Geolocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions.Services;
using XpressShip.Domain.Entities;

namespace XpressShip.Infrastructure.Services
{
    public class DistanceService : IDistanceService
    {
        public double CalculateDistance(Address origin, Address destination)
        => CalculateDistance(origin.Latitude, origin.Longitude, destination.Latitude, destination.Longitude);

        public double CalculateDistance(double originLat, double originLon, double destLat, double destLon)
        {
            var origin = new Coordinate(originLat, originLon);
            var destination = new Coordinate(destLat, destLon);

            var distance = GeoCalculator.GetDistance(origin, destination, 2, DistanceUnit.Kilometers); // in km

            return distance;
        }
    }
}
