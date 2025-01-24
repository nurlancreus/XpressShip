using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Domain.Abstractions;
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

        public async Task<Result<bool>> ValidateCountryAsync(string countryName,CancellationToken cancellationToken = default)
        {
            var isCountryExist = await _countryRepository.IsExistAsync(c => c.Name == countryName, cancellationToken);

            if (!isCountryExist)
                 return Result<bool>.Failure(Error.ConflictError($"Country ({countryName}) is not supported"));

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> ValidateCountryAndCityAsync(string countryName, string cityName,CancellationToken cancellationToken = default)
        {
            var country = await _countryRepository.Table
                                 .Include(c => c.Cities)
                                 .Select(c => new { c.Name, c.Cities })
                                 .FirstOrDefaultAsync(c => c.Name == countryName, cancellationToken);

            if (country is null)
                return Result<bool>.Failure(Error.ConflictError($"Country ({countryName}) is not supported"));
            else if (!country.Cities.Select(c => c.Name).Any(cName => cName == cityName))
                return Result<bool>.Failure(Error.ConflictError($"City ({cityName}) is invalid for the specified country."));

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> ValidateCountryAndPostalCodeAsync(string countryName, string postalCode,CancellationToken cancellationToken = default)
        {
            var country = await _countryRepository.Table
                                 .Select(c => new { c.Name, c.PostalCodePattern })
                                 .FirstOrDefaultAsync(c => c.Name == countryName, cancellationToken);

            if (country is null)
                return Result<bool>.Failure(Error.ConflictError($"Country ({countryName}) is not supported"));
            else
            {
                var isMatch = new Regex(country.PostalCodePattern).IsMatch(postalCode);

                if (!isMatch)
                    return Result<bool>.Failure(Error.ConflictError("Postal code is invalid for the specified country."));     
            }

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> ValidateCountryCityAndPostalCodeAsync(string countryName, string cityName, string postalCode,CancellationToken cancellationToken = default)
        {
            var country = await _countryRepository.Table
                                 .Include(c => c.Cities)
                                 .Select(c => new { c.Name, c.Cities, c.PostalCodePattern })
                                 .FirstOrDefaultAsync(c => c.Name == countryName, cancellationToken);

            if (country is null)
                return Result<bool>.Failure(Error.ConflictError($"Country ({countryName}) is not supported"));
            else
            {
                var isMatch = new Regex(country.PostalCodePattern).IsMatch(postalCode);

                if (!isMatch)
                    return Result<bool>.Failure(Error.ConflictError("Postal code is invalid for the specified country."));

                if (!country.Cities.Select(c => c.Name).Any(cName => cName == cityName))
                    return Result<bool>.Failure(Error.ConflictError($"City ({cityName}) is invalid for the specified country."));
            }

            return Result<bool>.Success(true);
        }
    }
}
