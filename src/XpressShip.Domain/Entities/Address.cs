using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Entities.Base;

namespace XpressShip.Domain.Entities
{
    public class Address : BaseEntity
    {
        public string Street { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Guid CityId { get; set; }
        public City City { get; set; } = null!;
        public Guid? ClientId { get; set; } 
        public ApiClient? Client { get; set; }
        public ICollection<Shipment> ShipmentsOrigin { get; set; } = [];
        public ICollection<Shipment> ShipmentsDestination { get; set; } = [];
        private Address()
        {

        }
        private Address(string postalCode, string street, double latitude, double longitude)
        {
            PostalCode = postalCode;
            Street = street;
            Latitude = latitude;
            Longitude = longitude;
        }

        public static Address Create(string postalCode, string street, double latitude, double longitude)
        {
            return new Address(postalCode, street, latitude, longitude);
        }
    }
}
