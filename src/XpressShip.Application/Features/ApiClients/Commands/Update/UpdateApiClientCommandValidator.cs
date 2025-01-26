using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Features.Addresses;
using XpressShip.Domain;
using XpressShip.Domain.Validation;

namespace XpressShip.Application.Features.ApiClients.Commands.Update
{
    public class UpdateApiClientCommandValidator : AbstractValidator<UpdateApiClientCommand>
    {
        public UpdateApiClientCommandValidator(IApiClientRepository apiClientRepository, IAddressValidationService addressValidationService)
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("API Client ID is required.");

            RuleFor(x => x.CompanyName)
                .MaximumLength(Constants.ApiClientCompanyNameMaxLength)
                .WithMessage($"Company name must not exceed {Constants.ApiClientCompanyNameMaxLength} characters.")
                .MustAsync(async (command, name, cancelltationToken) =>
                {
                    var apiClient = await apiClientRepository.Table.AsNoTracking().FirstOrDefaultAsync(c => c.CompanyName == name, cancelltationToken);

                    return apiClient == null || command.Id == apiClient.Id;
                })
                .WithMessage("Api Client with this name is already exists.")
                .When(x => !string.IsNullOrEmpty(x.CompanyName));

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Invalid email format.")
                .MustAsync(async (command, email, cancelltationToken) =>
                {
                    var apiClient = await apiClientRepository.Table.AsNoTracking().FirstOrDefaultAsync(c => c.Email == email, cancelltationToken);

                    return apiClient == null || command.Id == apiClient.Id;
                })
                .WithMessage("Api Client with this email is already exists.")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Address)
                .SetValidator(new AddressCommandDTOValidator(addressValidationService)!)
                .When(x => x.Address != null);
        }
    }
}
