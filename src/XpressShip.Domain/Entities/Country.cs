using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Entities.Base;

namespace XpressShip.Domain.Entities
{
    public class Country : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string PostalCodePattern { get; set; } = string.Empty;
        public decimal TaxPercentage { get; set; }

        public ICollection<City> Cities { get; set; } = [];

        private Country()
        {
            
        }

        private Country(string name, string code, string postalCodePatterm, decimal taxPercentage)
        {
            Name = name;
            Code = code;
            PostalCodePattern = postalCodePatterm;
            TaxPercentage = taxPercentage;
        }

        public static Country Create(string name, string code, string postalCodePatterm, decimal taxPercentage)
        {
            return new Country(name, code, postalCodePatterm, taxPercentage);
        }
    }
}
