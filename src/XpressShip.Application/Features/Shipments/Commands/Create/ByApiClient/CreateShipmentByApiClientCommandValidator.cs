using FluentValidation;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services;
using XpressShip.Application.Features.Addresses;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;
using XpressShip.Domain.Validation;

namespace XpressShip.Application.Features.Shipments.Commands.Create.ByApiClient
{
    public class CreateShipmentByApiClientCommandValidator : AbstractValidator<CreateShipmentByApiClientCommand>
    {
        public CreateShipmentByApiClientCommandValidator(IShipmentRateRepository shipmentRateRepository, IAddressValidationService addressValidationService, IGeoInfoService geoInfoService)
        {
            RuleFor(x => x.Method)
                .NotEmpty()
                .WithMessage("Shipment method is required.")
                .Must(method => Enum.TryParse<ShipmentMethod>(method, true, out _))
                .WithMessage("Invalid shipment method.");

            RuleFor(x => x.Weight)
                .GreaterThan(0)
                .WithMessage("Weight must be greater than zero.");

            RuleFor(x => x.Dimensions)
                .NotEmpty()
                .WithMessage("Dimensions are required.")
                .Must(dimensions => Shipment.ValidateDimensions(dimensions, false))
                .WithMessage("Dimensions must be in the format 'LxWxH'.");

            RuleFor(x => x.Origin)
                .SetValidator(new AddressCommandDTOValidator(addressValidationService)!)
                .When(x => x.Origin != null);

            RuleFor(x => x.Destination)
                .NotNull()
                .WithMessage("Destination address is required.")
                .SetValidator(new AddressCommandDTOValidator(addressValidationService));

            RuleFor(x => x.ShipmentRateId)
                .NotEmpty()
                .WithMessage("Shipment rate is required.")
                .MustAsync(async (rateId, cancellationToken) =>
                {
                    return await shipmentRateRepository.IsExistAsync(r => r.Id == rateId, cancellationToken);
                })
                .WithMessage("Invalid shipment rate.");

            // Custom Validations
            RuleFor(x => x)
                .CustomAsync(async (command, context, cancellationToken) =>
                {
                    var rate = await shipmentRateRepository.GetByIdAsync(command.ShipmentRateId, false, cancellationToken);

                    if (rate == null)
                    {
                        context.AddFailure("ShipmentRateId", "The specified shipment rate does not exist.");
                        return;
                    }

                    // Validate Weight 
                    if (command.Weight < rate.MinWeight || command.Weight > rate.MaxWeight)
                    {
                        context.AddFailure("Weight", $"Weight must be between {rate.MinWeight} and {rate.MaxWeight} kg.");
                    }

                    // Calculate Volume and Validate 
                    var volume = Shipment.CalculateVolume(command.Dimensions);
                    if (volume < rate.MinVolume || volume > rate.MaxVolume)
                    {
                        context.AddFailure("Dimensions", $"Volume must be between {rate.MinVolume} and {rate.MaxVolume} cubic units.");
                    }

                    // Validate Distance Between Origin and Destination
                    if (command.Origin != null && command.Destination != null)
                    {
                        var originGeoResult = await geoInfoService.GetLocationGeoInfoByNameAsync(command.Origin.Country, command.Origin.City, cancellationToken);
                        var destinationGeoResult = await geoInfoService.GetLocationGeoInfoByNameAsync(command.Destination.Country, command.Destination.City, cancellationToken);

                        if (!originGeoResult.IsSuccess || !destinationGeoResult.IsSuccess)
                        {
                            context.AddFailure("Distance", "Could not calculate distance due to invalid origin or destination.");
                            return;
                        }

                        var distance = Address.CalculateDistance(originGeoResult.Value.Latitude, originGeoResult.Value.Longitude,
                                                         destinationGeoResult.Value.Latitude, destinationGeoResult.Value.Longitude);

                        if (distance < rate.MinDistance || distance > rate.MaxDistance)
                        {
                            context.AddFailure("Distance", $"Distance must be between {rate.MinDistance} and {rate.MaxDistance} km.");
                        }
                    }
                });
        }
    }
}
