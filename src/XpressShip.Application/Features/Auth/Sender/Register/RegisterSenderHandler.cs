using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Hubs;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services;
using XpressShip.Application.Features.Auth.Sender.Register;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Entities.Users;
using XpressShip.Domain.Validation;
using SenderUser = XpressShip.Domain.Entities.Users.Sender;

namespace XpressShip.Application.Features.Auth.Sender.Register
{
    public class RegisterSenderHandler : ICommandHandler<RegisterSenderCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICountryRepository _countryRepository;
        private readonly IGeoInfoService _geoInfoService;
        private readonly IAdminHubService _adminHubService;

        public RegisterSenderHandler(UserManager<ApplicationUser> userManager, ICountryRepository countryRepository, IGeoInfoService geoInfoService, IAdminHubService adminHubService)
        {
            _userManager = userManager;
            _countryRepository = countryRepository;
            _geoInfoService = geoInfoService;
            _adminHubService = adminHubService;
        }

        public async Task<Result<Unit>> Handle(RegisterSenderCommand request, CancellationToken cancellationToken)
        {
            if (request.Password != request.ConfirmPassword) return Result<Unit>.Failure(Error.BadRequestError("Passwords do not match."));

            var country = await _countryRepository.Table.Include(c => c.Cities).FirstOrDefaultAsync(c => c.Name == request.Address.Country, cancellationToken);

            if (country is null) return Result<Unit>.Failure(Error.BadRequestError("Country is not supported"));

            var city = country.Cities.FirstOrDefault(c => c.Name == request.Address.City);

            if (city is null) return Result<Unit>.Failure(Error.BadRequestError("City is not supported"));

            var sender = SenderUser.Create(request.FirstName, request.LastName, request.UserName, request.Email, request.Phone);

            var geoInfoResult = await _geoInfoService.GetLocationGeoInfoByNameAsync(request.Address.Country, request.Address.City, cancellationToken);

            if (!geoInfoResult.IsSuccess) Result<Unit>.Failure(geoInfoResult.Error);

            var address = Address.Create(request.Address.PostalCode, request.Address.Street, geoInfoResult.Value.Latitude, geoInfoResult.Value.Longitude);

            sender.Address = address;

            var registerResult = await _userManager.CreateAsync(sender, request.Password);

            if (!registerResult.Succeeded) return Result<Unit>.Failure(Error.RegisterError());

            await _adminHubService.AdminNewSenderMessageAsync("New sender account is registered. Activate it.", cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
