using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Options;
using XpressShip.Application.Options.Cache;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Validation;

namespace XpressShip.Infrastructure.Services.Validation
{
    public class AddressValidationService : IAddressValidationService
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMemoryCache _cache;
        private readonly string _cacheKey;
        private readonly TimeSpan _absoluteDurationInHours;
        private readonly TimeSpan _slidingDurationInHours;

        public AddressValidationService(IOptionsSnapshot<InMemoryCacheSettings> options, ICountryRepository countryRepository, IMemoryCache cache)
        {
            var cacheSettings = options.Get(InMemoryCacheSettings.CountryData);

            _cacheKey = cacheSettings.CacheKey;
            _absoluteDurationInHours = TimeSpan.FromHours(cacheSettings.AbsoluteExpirationInHours);
            _slidingDurationInHours = TimeSpan.FromHours(cacheSettings.SlidingExpirationInHours);

            _countryRepository = countryRepository;
            _cache = cache;
        }

        // Load all countries into cache
        private async Task<Dictionary<string, (HashSet<string> Cities, string PostalCodePattern)>> LoadCountriesDataAsync(CancellationToken cancellationToken)
        {
            var countries = await _countryRepository.Table
                .Include(c => c.Cities)
                .Select(c => new { c.Name, Cities = c.Cities.Select(city => city.Name).ToHashSet(), c.PostalCodePattern })
                .ToListAsync(cancellationToken);

            return countries.ToDictionary(c => c.Name, c => (c.Cities, c.PostalCodePattern));
        }

        // Get cached country data (or load if missing)
        private async Task<Dictionary<string, (HashSet<string> Cities, string PostalCodePattern)>> GetCachedCountriesDataAsync(CancellationToken cancellationToken)
        {
            return await _cache.GetOrCreateAsync(_cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _absoluteDurationInHours;
                entry.SlidingExpiration = _slidingDurationInHours;
                return await LoadCountriesDataAsync(cancellationToken);
            }) ?? [];
        }

        // Validate Country 
        public async Task<Result<bool>> ValidateCountryAsync(string countryName, CancellationToken cancellationToken = default)
        {
            var countriesData = await GetCachedCountriesDataAsync(cancellationToken);

            if (!countriesData.ContainsKey(countryName))
                return Result<bool>.Failure(Error.ConflictError($"Country ({countryName}) is not supported"));

            return Result<bool>.Success(true);
        }

        // Validate Country & City 
        public async Task<Result<bool>> ValidateCountryAndCityAsync(string countryName, string cityName, CancellationToken cancellationToken = default)
        {
            var countriesData = await GetCachedCountriesDataAsync(cancellationToken);

            if (!countriesData.TryGetValue(countryName, out var countryData))
                return Result<bool>.Failure(Error.ConflictError($"Country ({countryName}) is not supported"));

            if (!countryData.Cities.Contains(cityName))
                return Result<bool>.Failure(Error.ConflictError($"City ({cityName}) is invalid for the specified country."));

            return Result<bool>.Success(true);
        }

        // Validate Country & Postal Code 
        public async Task<Result<bool>> ValidateCountryAndPostalCodeAsync(string countryName, string postalCode, CancellationToken cancellationToken = default)
        {
            var countriesData = await GetCachedCountriesDataAsync(cancellationToken);

            if (!countriesData.TryGetValue(countryName, out var countryData))
                return Result<bool>.Failure(Error.ConflictError($"Country ({countryName}) is not supported"));

            var isMatch = new Regex(countryData.PostalCodePattern).IsMatch(postalCode);
            if (!isMatch)
                return Result<bool>.Failure(Error.ConflictError("Postal code is invalid for the specified country."));

            return Result<bool>.Success(true);
        }

        // Validate Country, City & Postal Code 
        public async Task<Result<bool>> ValidateCountryCityAndPostalCodeAsync(string countryName, string cityName, string postalCode, CancellationToken cancellationToken = default)
        {
            var countriesData = await GetCachedCountriesDataAsync(cancellationToken);

            if (!countriesData.TryGetValue(countryName, out var countryData))
                return Result<bool>.Failure(Error.ConflictError($"Country ({countryName}) is not supported"));

            if (!countryData.Cities.Contains(cityName))
                return Result<bool>.Failure(Error.ConflictError($"City ({cityName}) is invalid for the specified country."));

            var isMatch = new Regex(countryData.PostalCodePattern).IsMatch(postalCode);
            if (!isMatch)
                return Result<bool>.Failure(Error.ConflictError("Postal code is invalid for the specified country."));

            return Result<bool>.Success(true);
        }

        // Refresh Cache if Country Data Changes **
        public async Task RefreshCountriesCacheAsync(CancellationToken cancellationToken = default)
        {
            var newData = await LoadCountriesDataAsync(cancellationToken);
            _cache.Set(_cacheKey, newData, _absoluteDurationInHours);
        }
    }
}
