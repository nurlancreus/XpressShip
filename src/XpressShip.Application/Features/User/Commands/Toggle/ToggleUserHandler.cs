using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities.Users;

namespace XpressShip.Application.Features.User.Commands.Toggle
{
    public class ToggleUserHandler : ICommandHandler<ToggleUserCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtSession _jwtSession;

        public ToggleUserHandler(UserManager<ApplicationUser> userManager, IJwtSession jwtSession)
        {
            _userManager = userManager;
            _jwtSession = jwtSession;
        }

        public async Task<Result<Unit>> Handle(ToggleUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user is null) return Result<Unit>.Failure(Error.NotFoundError(nameof(user)));

            if (user is Admin admin)
            {
                var isSuperAdminAuthResult = _jwtSession.IsSuperAdminAuth();

                if (!isSuperAdminAuthResult.IsSuccess) return Result<Unit>.Failure(isSuperAdminAuthResult.Error);

                admin.Toggle();
            }
            else if (user is Sender sender)
            {
                var isAdminAuthResult = _jwtSession.IsAdminAuth();

                if (!isAdminAuthResult.IsSuccess) return Result<Unit>.Failure(isAdminAuthResult.Error);

                sender.Toggle();
            }

            await _userManager.UpdateAsync(user);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
