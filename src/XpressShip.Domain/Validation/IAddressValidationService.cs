using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XpressShip.Domain.Validation
{
    public interface IAddressValidationService
    {
        Task<bool> ValidateCountryAsync(string countryName, bool throwException = true, CancellationToken cancellationToken = default);
        Task<bool> ValidateCountryAndCityAsync(string countryName, string cityName, bool throwException = true, CancellationToken cancellationToken = default);
        Task<bool> ValidateCountryAndPostalCodeAsync(string countryName, string postalCode, bool throwException = true, CancellationToken cancellationToken = default);
        Task<bool> ValidateCountryCityAndPostalCodeAsync(string countryName, string cityName, string postalCode, bool throwException = true, CancellationToken cancellationToken = default);
    }
}
