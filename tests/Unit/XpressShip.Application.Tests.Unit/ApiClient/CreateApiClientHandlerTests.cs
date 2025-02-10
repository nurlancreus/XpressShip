using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Notification;
using XpressShip.Application.Abstractions.Services;
using XpressShip.Application.Abstractions;
using Moq;
using XpressShip.Application.Features.ApiClients.Commands.Create;
using XpressShip.Domain.Validation;
using XpressShip.Tests.Common.Factories;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities;
using XpressShip.Application.DTOs;

using DataConstants = XpressShip.Tests.Common.Constants.Constants;
using Microsoft.EntityFrameworkCore;
using XpressShip.Domain.Entities.Users;
using FluentAssertions;
using XpressShip.Application.Features.ApiClients.DTOs;
using Moq.EntityFrameworkCore;
using XpressShip.Tests.Common.Handlers;

namespace XpressShip.Application.Tests.Unit.ApiClient
{
    public class CreateApiClientHandlerTests
    {
        private readonly IQueryable<Country> _countries = Handler.Command.Countries;

        private readonly Mock<ICountryRepository> _mockCountryRepository;
        private readonly Mock<IGeoInfoService> _mockGeoInfoService;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Handler.Command.ApiClient _handler;
        public CreateApiClientHandlerTests()
        {
            var command = new Handler.Command();

            _mockCountryRepository = command.mockCountryRepository;

            _mockGeoInfoService = command.mockGeoInfoService;
            _mockUnitOfWork = command.mockUnitOfWork;

            _handler = new Handler.Command.ApiClient(command);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsIsSuccessTrueAndKeysDTO()
        {
            // Arrange
            var request = Factory.ApiClient.GenerateApiClientCommand();

            var geoInfoResult = Result<LocationGeoInfoDTO>.Success(new LocationGeoInfoDTO { Latitude = 10.0, Longitude = 20.0, Name = DataConstants.AddressCommand.Country });
            _mockGeoInfoService.Setup(x => x.GetLocationGeoInfoByNameAsync(request.Address.Country, request.Address.City, It.IsAny<CancellationToken>()))
                .ReturnsAsync(geoInfoResult);

            _mockCountryRepository.Setup(x => x.Table)
                .ReturnsDbSet(_countries);

            _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            // Act
            var result = await _handler.Handle(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<KeysDTO>();
        }

        [Fact]
        public async Task Handle_InValidRequest_ReturnsIsFailureAndValidationError()
        {
            // Arrange
            var request = Factory.ApiClient.GenerateInValidApiClientCommand();

            var geoInfoResult = Result<LocationGeoInfoDTO>.Success(new LocationGeoInfoDTO { Latitude = 10.0, Longitude = 20.0, Name = DataConstants.AddressCommand.Country });
            _mockGeoInfoService.Setup(x => x.GetLocationGeoInfoByNameAsync(request.Address.Country, request.Address.City, It.IsAny<CancellationToken>()))
                .ReturnsAsync(geoInfoResult);

            _mockCountryRepository.Setup(x => x.Table)
                .ReturnsDbSet(_countries);

            _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            // Act
            var result = await _handler.Handle(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public async Task Handle_InValidLocation_ReturnsIsFailureAndUnexpectedError()
        {
            // Arrange
            var request = Factory.ApiClient.GenerateApiClientCommand();

            var geoInfoResult = Result<LocationGeoInfoDTO>.Success(new LocationGeoInfoDTO { Latitude = 0.0, Longitude = -200.0, Name = DataConstants.AddressCommand.Country });
            _mockGeoInfoService.Setup(x => x.GetLocationGeoInfoByNameAsync(request.Address.Country, request.Address.City, It.IsAny<CancellationToken>()))
                .ReturnsAsync(geoInfoResult);

            _mockCountryRepository.Setup(x => x.Table)
                .ReturnsDbSet(_countries);

            _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            // Act
            var result = await _handler.Handle(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Type.Should().Be(ErrorType.Unexpected);
        }
    }
}
