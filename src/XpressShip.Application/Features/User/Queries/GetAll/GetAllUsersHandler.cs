using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.User.DTOs;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities.Users;

namespace XpressShip.Application.Features.User.Queries.GetAll
{
    public class GetAllUsersHandler : IQueryHandler<GetAllUsersQuery, IEnumerable<UserDTO>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetAllUsersHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<IEnumerable<UserDTO>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = _userManager.Users;

            if (request.UserType == "sender") users = users.OfType<Sender>();
            else if (request.UserType == "admin") users = users.OfType<Admin>();

            if (request.IsActive is bool isActive) users = users.Where(u => u.IsActive == isActive);

            var dtos = await users.Select(u => new UserDTO(u)).ToListAsync(cancellationToken);

            return Result<IEnumerable<UserDTO>>.Success(dtos);
        }
    }
}
