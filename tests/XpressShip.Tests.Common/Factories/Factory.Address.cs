using Bogus;
using XpressShip.Domain.Entities;
using AddressEntity = XpressShip.Domain.Entities.Address;
using DataConstants = XpressShip.Tests.Common.Constants.Constants;

namespace XpressShip.Tests.Common.Factories
{
    public static partial class Factory
    {
        public static class Address
        {
            private static readonly Faker _faker = new();

            public static AddressEntity CreateFakeAddress()
            {
                return AddressEntity.Create(
                    _faker.Address.ZipCode(),
                    _faker.Address.StreetName(),
                    _faker.Address.Latitude(),
                    _faker.Address.Longitude()
                );
            }
            public static AddressEntity GenerateOriginAddress(bool withCity = true)
            {
                var postalCode = DataConstants.OriginAddress.PostalCode;
                var street = DataConstants.OriginAddress.Street;
                var latitude = DataConstants.OriginAddress.Lat;
                var longitude = DataConstants.OriginAddress.Lon;

                var originAddress = AddressEntity.Create(postalCode, street, latitude, longitude);

                if (withCity)
                {
                    var countryName = DataConstants.OriginAddress.Country;
                    var countryCode = DataConstants.OriginAddress.Code;
                    var countryPostalCodePattern = DataConstants.OriginAddress.PostalCodePattern;
                    var countryTaxPercentage = DataConstants.OriginAddress.TaxPercentage;

                    var country = Country.Create(countryName, countryCode, countryPostalCodePattern, countryTaxPercentage);

                    var city = City.Create(DataConstants.OriginAddress.City, country);

                    originAddress.City = city;
                }

                return originAddress;
            }

            public static AddressEntity GenerateDestinationAddress(decimal? taxPercentage = null, bool withCity = true)
            {
                var postalCode = DataConstants.DestinationAddress.PostalCode;
                var street = DataConstants.DestinationAddress.Street;
                var latitude = DataConstants.DestinationAddress.Lat;
                var longitude = DataConstants.DestinationAddress.Lon;

                var destinationAddress = AddressEntity.Create(postalCode, street, latitude, longitude);

                if (withCity)
                {
                    var countryName = DataConstants.DestinationAddress.Country;
                    var countryCode = DataConstants.DestinationAddress.Code;
                    var countryPostalCodePattern = DataConstants.DestinationAddress.PostalCodePattern;
                    var countryTaxPercentage = taxPercentage is not decimal taxPercent ? DataConstants.DestinationAddress.TaxPercentage : taxPercent;

                    var country = Country.Create(countryName, countryCode, countryPostalCodePattern, countryTaxPercentage);

                    var city = City.Create(DataConstants.DestinationAddress.City, country);

                    destinationAddress.City = city;
                }

                return destinationAddress;
            }
        }
    }
}
