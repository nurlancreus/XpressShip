using FluentValidation;
using XpressShip.Application.Features.Addresses.DTOs;
using XpressShip.Domain.Validation;

namespace XpressShip.Application.Features.Addresses
{
    public class AddressCommandDTOValidator : AbstractValidator<AddressCommandDTO>
    {
        public AddressCommandDTOValidator(IAddressValidationService addressValidationService)
        {
            RuleFor(x => x.Country)
                .NotEmpty()
                .WithMessage("Country is required.")
                .MaximumLength(50)
                .WithMessage("Country name '{PropertyValue}' must not exceed 50 characters.")
                .MustAsync(async (country, cancellationToken) =>
                {
                    var validationResult = await addressValidationService.ValidateCountryAsync(country, cancellationToken);
                    return validationResult.IsSuccess;
                })
                .WithMessage(country => $"Country '{country}' is not supported.");

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("City is required.")
                .MaximumLength(50)
                .WithMessage("City name '{PropertyValue}' must not exceed 50 characters.")
                .MustAsync(async (dto, city, context, cancellationToken) =>
                {
                    var country = context.InstanceToValidate.Country;
                    var validationResult = await addressValidationService.ValidateCountryAndCityAsync(country, city, cancellationToken);
                    return validationResult.IsSuccess;
                })
                .WithMessage((request, city) => $"City '{city}' is invalid for the specified country '{request.Country}'.");

            RuleFor(x => x.PostalCode)
                .NotEmpty()
                .WithMessage("Postal code is required.")
                .MustAsync(async (dto, postalCode, context, cancellationToken) =>
                {
                    var country = context.InstanceToValidate.Country;
                    var validationResult = await addressValidationService.ValidateCountryAndPostalCodeAsync(country, postalCode, cancellationToken);
                    return validationResult.IsSuccess;
                })
                .WithMessage((request, postalCode) => $"Postal code '{postalCode}' is invalid for the specified country '{request.Country}'.");
        }
    }
}
