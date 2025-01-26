using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Features.Addresses;
using XpressShip.Domain;
using XpressShip.Domain.Validation;

namespace XpressShip.Application.Features.ApiClients.Commands.Create
{
    public class CreateApiClientCommandValidator : AbstractValidator<CreateApiClientCommand>
    {
        public CreateApiClientCommandValidator(IApiClientRepository apiClientRepository, IAddressValidationService addressValidationService)
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty()
                .WithMessage("Company name is required.")
                .MaximumLength(Constants.ApiClientCompanyNameMaxLength)
                .WithMessage($"Company name must not exceed {Constants.ApiClientCompanyNameMaxLength} characters.")
                .MustAsync(async (name, cancelltationToken) =>
                {
                    var isNameExist = await apiClientRepository.IsExistAsync(c => c.CompanyName == name, cancelltationToken);

                    return !isNameExist;
                })
                .WithMessage("Api Client with this name is already exists.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.")
                .MustAsync(async (email, cancelltationToken) =>
                {
                    var isEmailExist = await apiClientRepository.IsExistAsync(c => c.Email == email, cancelltationToken);

                    return !isEmailExist;
                })
                .WithMessage("Api Client with this email is already exists.");

            RuleFor(x => x.Address)
                .NotNull()
                .WithMessage("Address is required.")
                .SetValidator(new AddressCommandDTOValidator(addressValidationService));
        }
    }
}
