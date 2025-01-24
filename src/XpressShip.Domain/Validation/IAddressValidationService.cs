using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Abstractions;

namespace XpressShip.Domain.Validation
{
    public interface IAddressValidationService
    {
        Task<Result<bool>> ValidateCountryAsync(string countryName, CancellationToken cancellationToken = default);
        Task<Result<bool>> ValidateCountryAndCityAsync(string countryName, string cityName, CancellationToken cancellationToken = default);
        Task<Result<bool>> ValidateCountryAndPostalCodeAsync(string countryName, string postalCode, CancellationToken cancellationToken = default);
        Task<Result<bool>> ValidateCountryCityAndPostalCodeAsync(string countryName, string cityName, string postalCode, CancellationToken cancellationToken = default);
    }
}
