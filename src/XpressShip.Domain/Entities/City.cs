using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Entities.Base;

namespace XpressShip.Domain.Entities
{
    public class City : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public Guid CountryId { get; set; }
        public Country Country { get; set; } = null!;

        public ICollection<Address> Addresses { get; set; } = [];

        private City()
        {
            
        }
        private City(string name, Guid countryId)
        {
            Name = name;
            CountryId = countryId;
        }
        private City(string name, Country country)
        {
            Name = name;
            Country = country;
        }

        public static City Create(string name, Guid countryId)
        {
            return new City(name, countryId);
        }

        public static City Create(string name, Country country)
        {
            return new City(name, country);
        }
    }
}
