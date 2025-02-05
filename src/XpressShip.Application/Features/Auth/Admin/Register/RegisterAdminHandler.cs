using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Hubs;
using XpressShip.Application.Abstractions.Services.Notification;
using XpressShip.Application.Abstractions.Services.Token;
using XpressShip.Application.DTOs.Token;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities.Users;
using AdminUser = XpressShip.Domain.Entities.Users.Admin;

namespace XpressShip.Application.Features.Auth.Admin.Register
{
    public class RegisterAdminHandler : ICommandHandler<RegisterAdminCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAdminNotificationService _adminNotificationService;
        public RegisterAdminHandler(UserManager<ApplicationUser> userManager, IAdminNotificationService adminNotificationService)
        {
            _userManager = userManager;
            _adminNotificationService = adminNotificationService;
        }

        public async Task<Result<Unit>> Handle(RegisterAdminCommand request, CancellationToken cancellationToken)
        {
            if (request.Password != request.ConfirmPassword) return Result<Unit>.Failure(Error.BadRequestError("Passwords do not match."));

            var admin = AdminUser.Create(request.FirstName, request.LastName, request.UserName, request.Email, request.Phone);

            var registerResult = await _userManager.CreateAsync(admin, request.Password);

            if (!registerResult.Succeeded) return Result<Unit>.Failure(Error.RegisterError());

            await _userManager.AddToRoleAsync(admin, "Admin");

            await _adminNotificationService.SendNewAdminNotificationAsync(admin, cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
