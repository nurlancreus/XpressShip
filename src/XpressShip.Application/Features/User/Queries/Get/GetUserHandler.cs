using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.User.DTOs;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities.Users;

namespace XpressShip.Application.Features.User.Queries.Get
{
    public class GetUserHandler : IQueryHandler<GetUserQuery, UserDTO>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetUserHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<UserDTO>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id);

            if (user is null) return Result<UserDTO>.Failure(Error.NotFoundError(nameof(user)));

            return Result<UserDTO>.Success(new UserDTO(user));
        }
    }
}
