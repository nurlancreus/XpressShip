using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Domain.Validation;

namespace XpressShip.Infrastructure.Services.Validation
{
    public class AddressValidationService : IAddressValidationService
    {
        private readonly ICountryRepository _countryRepository;

        public AddressValidationService(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<bool> ValidateCountryAsync(string countryName, bool throwException = true, CancellationToken cancellationToken = default)
        {
            var isCountryExist = await _countryRepository.IsExistAsync(c => c.Name == countryName, cancellationToken);

            if (!isCountryExist)
            {
                if (throwException) throw new ValidationException($"Country ({countryName}) is not supported");
                return false;
            }

            return true;
        }

        public async Task<bool> ValidateCountryAndCityAsync(string countryName, string cityName, bool throwException = true, CancellationToken cancellationToken = default)
        {
            var country = await _countryRepository.Table
                                 .Include(c => c.Cities)
                                 .Select(c => new { c.Name, c.Cities })
                                 .FirstOrDefaultAsync(c => c.Name == countryName, cancellationToken);

            if (country is null)
            {
                if (throwException) throw new ValidationException($"Country ({countryName}) is not supported");
                return false;
            }
            else
            {
                if (!country.Cities.Select(c => c.Name).Any(cName => cName == cityName))
                {
                    if (throwException) throw new ValidationException($"City ({cityName}) is invalid for the specified country.");

                    return false;
                }

            }

            return true;
        }

        public async Task<bool> ValidateCountryAndPostalCodeAsync(string countryName, string postalCode, bool throwException = true, CancellationToken cancellationToken = default)
        {
            var country = await _countryRepository.Table
                                 .Select(c => new { c.Name, c.PostalCodePattern })
                                 .FirstOrDefaultAsync(c => c.Name == countryName, cancellationToken);

            if (country is null)
            {
                if (throwException) throw new ValidationException($"Country ({countryName}) is not supported");

                return false;
            }
            else
            {
                var isMatch = new Regex(country.PostalCodePattern).IsMatch(postalCode);

                if (!isMatch)
                {
                    if (throwException) throw new ValidationException("Postal code is invalid for the specified country.");

                    return false;
                }
            }

            return true;
        }

        public async Task<bool> ValidateCountryCityAndPostalCodeAsync(string countryName, string cityName, string postalCode, bool throwException = true, CancellationToken cancellationToken = default)
        {
            var country = await _countryRepository.Table
                                 .Include(c => c.Cities)
                                 .Select(c => new { c.Name, c.Cities, c.PostalCodePattern })
                                 .FirstOrDefaultAsync(c => c.Name == countryName, cancellationToken);

            if (country is null)
            {
                if (throwException) throw new ValidationException($"Country ({countryName}) is not supported");

                return false;
            }
            else
            {
                var isMatch = new Regex(country.PostalCodePattern).IsMatch(postalCode);

                if (!isMatch)
                {
                    if (throwException) throw new ValidationException("Postal code is invalid for the specified country.");

                    return false;
                }

                if (!country.Cities.Select(c => c.Name).Any(cName => cName == cityName))
                {
                    if (throwException) throw new ValidationException($"City ({cityName}) is invalid for the specified country.");

                    return false;
                }
            }

            return true;
        }
    }
}
