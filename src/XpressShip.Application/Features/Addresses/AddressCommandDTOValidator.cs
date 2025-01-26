using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                .WithMessage("Country name must not exceed 50 characters.")
                .MustAsync(async (country, cancellationToken) =>
                {
                    var validationResult = await addressValidationService.ValidateCountryAsync(country, cancellationToken);

                    return validationResult.IsSuccess;
                })
                .WithMessage("Country is not supported.");

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("City is required.")
                .MaximumLength(50)
                .WithMessage("City name must not exceed 50 characters.")
                .MustAsync(async (dto, city, context, cancellationToken) =>
                {
                    var country = context.InstanceToValidate.Country;

                    var validationResult = await addressValidationService.ValidateCountryAndCityAsync(country, city, cancellationToken);

                    return validationResult.IsSuccess;
                })
                .WithMessage("City is invalid for the specified country.");

            RuleFor(x => x.PostalCode)
                .NotEmpty()
                .WithMessage("Postal code is required.")
                .MustAsync(async (dto, postalCode, context, cancellationToken) =>
                {
                    var country = context.InstanceToValidate.Country;

                    var validationResult = await addressValidationService.ValidateCountryAndPostalCodeAsync(country, postalCode, cancellationToken);

                    return validationResult.IsSuccess;
                })
                .WithMessage("Postal code is invalid for the specified country.");
        }
    }
}
