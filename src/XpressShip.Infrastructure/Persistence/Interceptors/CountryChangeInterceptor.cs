using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Validation;

namespace XpressShip.Infrastructure.Persistence.Interceptors
{
    public class CountryChangeInterceptor : SaveChangesInterceptor
    {
        private readonly IAddressValidationService _addressValidationService;

        public CountryChangeInterceptor(IAddressValidationService addressValidationService)
        {
            _addressValidationService = addressValidationService;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var dbContext = eventData.Context;

            var trackedCountries = dbContext!.ChangeTracker.Entries<Country>();
            var trackedCities = dbContext!.ChangeTracker.Entries<City>();

            var isTrackedCountriesChanged = trackedCountries.Any(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);

            var isTrackedCitiesChanged = trackedCities.Any(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);

            if (isTrackedCountriesChanged || isTrackedCitiesChanged)
            {
                await _addressValidationService.RefreshCountriesCacheAsync(cancellationToken);
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }

}
