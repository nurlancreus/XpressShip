using FluentValidation;
using Microsoft.EntityFrameworkCore;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services;
using XpressShip.Application.Features.Addresses;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;
using XpressShip.Domain.Validation;

namespace XpressShip.Application.Features.Shipments.Commands.UpdateDetails
{
    public class UpdateShipmentDetailsCommandValidator : AbstractValidator<UpdateShipmentDetailsCommand>
    {
        public UpdateShipmentDetailsCommandValidator(
            IShipmentRepository shipmentRepository,
            IShipmentRateRepository shipmentRateRepository,
            IAddressValidationService addressValidationService,
            IGeoInfoService geoInfoService)
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Shipment ID is required.")
                .MustAsync(async (id, cancellationToken) =>
                {
                    return await shipmentRepository.IsExistAsync(s => s.Id == id, cancellationToken);
                })
                .WithMessage("Shipment does not exist.");

            RuleFor(x => x.Method)
                .Must(method => string.IsNullOrEmpty(method) || Enum.TryParse<ShipmentMethod>(method, true, out _))
                .WithMessage("Invalid shipment method.");

            RuleFor(x => x.Weight)
                .GreaterThan(0)
                .When(x => x.Weight != null)
                .WithMessage("Weight must be greater than zero.");

            RuleFor(x => x.Dimensions)
                .Must(dimensions => string.IsNullOrEmpty(dimensions) || Shipment.ValidateDimensions(dimensions, false))
                .WithMessage("Dimensions must be in the format 'LxWxH'.");

            RuleFor(x => x.Origin)
                .SetValidator(new AddressCommandDTOValidator(addressValidationService)!)
                .When(x => x.Origin != null);

            RuleFor(x => x.Destination)
                .SetValidator(new AddressCommandDTOValidator(addressValidationService)!)
                .When(x => x.Destination != null);

            // Custom Validations
            RuleFor(x => x)
                .CustomAsync(async (command, context, cancellationToken) =>
                {
                    ShipmentRate? rate;

                    if (command.ShipmentRateId != null)
                    {
                        rate = await shipmentRateRepository.GetByIdAsync(command.ShipmentRateId.Value, false, cancellationToken);

                        if (rate == null)
                        {
                            context.AddFailure("ShipmentRateId", "The specified shipment rate does not exist.");
                            return;
                        }
                    }
                    else
                    {
                        var shipment = await shipmentRepository.Table
                            .Include(s => s.Rate)
                            .FirstOrDefaultAsync(s => s.Id == command.Id, cancellationToken);

                        if (shipment == null)
                        {
                            context.AddFailure("Id", "Shipment does not exist.");
                            return;
                        }

                        rate = shipment.Rate;
                    }

                    if (command.Weight != null &&
                        (command.Weight < rate.MinWeight || command.Weight > rate.MaxWeight))
                    {
                        context.AddFailure("Weight", $"Weight must be between {rate.MinWeight} and {rate.MaxWeight} kg.");
                    }

                    if (!string.IsNullOrEmpty(command.Dimensions))
                    {
                        var volume = Shipment.CalculateVolume(command.Dimensions);
                        if (volume < rate.MinVolume || volume > rate.MaxVolume)
                        {
                            context.AddFailure("Dimensions", $"Volume must be between {rate.MinVolume} and {rate.MaxVolume} cubic units.");
                        }
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

                        var distance = Address.CalculateDistance(
                            originGeoResult.Value.Latitude, originGeoResult.Value.Longitude,
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
