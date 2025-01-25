using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.User.DTOs;
using XpressShip.Application.Features.User.Queries.GetAll;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities.Users;

namespace XpressShip.Application.Features.User.Queries.GetAllSenders
{
    public class GetAllSendersHandler : IQueryHandler<GetAllSendersQuery, IEnumerable<SenderDTO>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetAllSendersHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<IEnumerable<SenderDTO>>> Handle(GetAllSendersQuery request, CancellationToken cancellationToken)
        {
            var users = _userManager.Users.OfType<Sender>()
                            .Include(u => u.Shipments)
                                .ThenInclude(s => s.OriginAddress)
                            .Include(u => u.Shipments)
                                .ThenInclude(s => s.DestinationAddress)
                            .Include(u => u.Shipments)
                                .ThenInclude(s => s.Rate)
                            .Include(u => u.Shipments)
                                .ThenInclude(s => s.Payment)
                            .Include(u => u.Address)
                            .AsQueryable();

            if (request.IsActive is bool isActive) users = users.Where(u => u.IsActive == isActive);

            var dtos = await users.Select(u => new SenderDTO(u)).ToListAsync(cancellationToken);

            return Result<IEnumerable<SenderDTO>>.Success(dtos);
        }
    }
}
