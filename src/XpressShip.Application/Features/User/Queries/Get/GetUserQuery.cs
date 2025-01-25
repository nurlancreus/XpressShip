using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.User.DTOs;

namespace XpressShip.Application.Features.User.Queries.Get
{
    public record GetUserQuery : IQuery<UserDTO>
    {
        public string Id { get; set; } = string.Empty;
    }
}
