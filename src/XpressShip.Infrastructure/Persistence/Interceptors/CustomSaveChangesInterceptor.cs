using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Validation;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using XpressShip.Domain.Abstractions;

namespace XpressShip.Infrastructure.Persistence.Interceptors
{
    public class CustomSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly IAddressValidationService _addressValidationService;

        public CustomSaveChangesInterceptor(IAddressValidationService addressValidationService)
        {
            _addressValidationService = addressValidationService;
        }

        public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            var dbContext = eventData.Context;

            if (dbContext is not null)
            {
                await CountryDataChangesAsync(dbContext, cancellationToken);
            }

            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var dbContext = eventData.Context;

            if (dbContext is not null)
            {
                UpdateAuditableEntities(dbContext);
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        static void UpdateAuditableEntities(DbContext dbContext)
        {
            DateTime utcNow = DateTime.UtcNow;
            var entities = dbContext.ChangeTracker.Entries<IBase>();

            foreach (EntityEntry<IBase> entry in entities)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property(nameof(IBase.CreatedAt)).CurrentValue = utcNow;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(IBase.UpdatedAt)).CurrentValue = utcNow;
                }
            }
        }

        async Task CountryDataChangesAsync(DbContext dbContext, CancellationToken cancellationToken)
        {
            var trackedCountries = dbContext.ChangeTracker.Entries<Country>();
            var trackedCities = dbContext.ChangeTracker.Entries<City>();

            var isTrackedCountriesChanged = trackedCountries.Any(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);

            var isTrackedCitiesChanged = trackedCities.Any(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);

            if (isTrackedCountriesChanged || isTrackedCitiesChanged)
            {
                await _addressValidationService.RefreshCountriesCacheAsync(cancellationToken);
            }
        }

    }

}
