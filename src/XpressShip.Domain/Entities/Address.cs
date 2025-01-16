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
        public string City { get; set; } = string.Empty;
        public string? State { get; set; }
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // Navigation properties
        public Guid ClientId { get; set; } // Foreign key to ApiClient
        public ApiClient? Client { get; set; }
        public ICollection<Shipment> ShipmentsOrigin { get; set; } = [];
        public ICollection<Shipment> ShipmentsDestination { get; set; } = [];

        private Address(string country, string city, string? state, string postalCode, string street, double latitude, double longitude)
        {
            Country = country;
            City = city;
            State = state;
            PostalCode = postalCode;
            Street = street;
            Latitude = latitude;
            Longitude = longitude;
        }

        public static Address Create(string country, string city, string? state, string postalCode, string street, double latitude, double longitude)
        {
            return new Address(country, city, state, postalCode, street, latitude, longitude);
        }
    }
}
