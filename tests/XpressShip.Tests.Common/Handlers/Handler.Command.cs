using Moq;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Notification;
using XpressShip.Application.Abstractions.Services;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.ApiClients.Commands.Create;
using XpressShip.Domain.Validation;
using XpressShip.Application.Behaviours;
using XpressShip.Domain.Abstractions;
using XpressShip.Application.Features.ApiClients.DTOs;
using XpressShip.Domain.Entities;

using DataConstants = XpressShip.Tests.Common.Constants.Constants;
using System.Text.RegularExpressions;

namespace XpressShip.Tests.Common.Handlers
{
    public static partial class Handler
    {
        public class Command
        {
            public static IQueryable<Country> Countries => DataConstants.Country.CountryData.Select(countryData =>
            {
                var (postalCodePattern, countryCode, cities, taxPercentage) = countryData.Value;

                var country = Country.Create(countryData.Key, countryCode, postalCodePattern, taxPercentage);

                var cityDatas = cities.Select(city => City.Create(city, country)).ToList();

                country.Cities = cityDatas;

                return country;
            }).AsQueryable();

            public readonly Mock<ICountryRepository> mockCountryRepository;
            public readonly Mock<IGeoInfoService> mockGeoInfoService;
            public readonly Mock<IUnitOfWork> mockUnitOfWork;
            public readonly Mock<IAdminNotificationService> mockAdminNotificationService;

            public readonly Mock<IAddressValidationService> mockAddressValidationService;

            public Command()
            {
                mockCountryRepository = new Mock<ICountryRepository>();
                mockGeoInfoService = new Mock<IGeoInfoService>();
                mockUnitOfWork = new Mock<IUnitOfWork>();
                mockAdminNotificationService = new Mock<IAdminNotificationService>();
                mockAddressValidationService = new Mock<IAddressValidationService>();

                #region MockAddressValidation
                mockAddressValidationService
                    .Setup(x => x.ValidateCountryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((string countryName, CancellationToken _) =>
                        DataConstants.Country.CountryData.ContainsKey(countryName)
                            ? Result<bool>.Success(true)
                            : Result<bool>.Failure(Error.ConflictError($"Country ({countryName}) is not supported")));

                mockAddressValidationService
                    .Setup(x => x.ValidateCountryAndCityAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((string countryName, string cityName, CancellationToken _) =>
                        DataConstants.Country.CountryData.TryGetValue(countryName, out var countryData) && countryData.cities.Contains(cityName)
                            ? Result<bool>.Success(true)
                            : Result<bool>.Failure(Error.ConflictError($"City ({cityName}) is invalid for the specified country.")));

                mockAddressValidationService
                    .Setup(x => x.ValidateCountryAndPostalCodeAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((string countryName, string postalCode, CancellationToken _) =>
                        DataConstants.Country.CountryData.TryGetValue(countryName, out var countryData) &&
                        new Regex(countryData.postalCodePattern).IsMatch(postalCode)
                            ? Result<bool>.Success(true)
                            : Result<bool>.Failure(Error.ConflictError("Postal code is invalid for the specified country.")));

                mockAddressValidationService
                    .Setup(x => x.ValidateCountryCityAndPostalCodeAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((string countryName, string cityName, string postalCode, CancellationToken _) =>
                        DataConstants.Country.CountryData.TryGetValue(countryName, out var countryData) &&
                        countryData.cities.Contains(cityName) &&
                        new Regex(countryData.postalCodePattern).IsMatch(postalCode)
                            ? Result<bool>.Success(true)
                            : Result<bool>.Failure(Error.ConflictError("Address validation failed.")));
                #endregion
            }

            public class ApiClient
            {
                private readonly Mock<IApiClientRepository> _mockApiClientRepository;
                private readonly CreateApiClientCommandValidator _validators;
                private readonly ExceptionHandlingPipelineBehavior<CreateApiClientCommand, Result<KeysDTO>> _exceptionHandlingPipelineBehavior;
                private readonly ValidationPipelineBehaviour<CreateApiClientCommand, Result<KeysDTO>> _validationPipelineBehaviour;

                private readonly CreateApiClientHandler _handler;

                public ApiClient(Command command)
                {
                    _mockApiClientRepository = new Mock<IApiClientRepository>();

                    _validators = new CreateApiClientCommandValidator(_mockApiClientRepository.Object, command.mockAddressValidationService.Object);

                    _exceptionHandlingPipelineBehavior = new ExceptionHandlingPipelineBehavior<CreateApiClientCommand, Result<KeysDTO>>();
                    _validationPipelineBehaviour = new ValidationPipelineBehaviour<CreateApiClientCommand, Result<KeysDTO>>([_validators]);

                    _handler = new CreateApiClientHandler(
                        _mockApiClientRepository.Object,
                        command.mockCountryRepository.Object,
                        command.mockGeoInfoService.Object,
                        command.mockUnitOfWork.Object,
                        command.mockAdminNotificationService.Object);

                }

                public async Task<Result<KeysDTO>> Handle(CreateApiClientCommand command)
                {

                    return await _exceptionHandlingPipelineBehavior.Handle(command, () => _validationPipelineBehaviour.Handle(command, () => _handler.Handle(command, CancellationToken.None), CancellationToken.None), CancellationToken.None);
                }
            }
        }
    }
}
