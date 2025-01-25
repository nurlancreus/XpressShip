using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.User.DTOs;

namespace XpressShip.Application.Features.User.Queries.GetAll
{
    public record GetAllUsersQuery : IQuery<IEnumerable<UserDTO>>
    {
        public string? UserType { get; set; }
        public bool? IsActive { get; set; }
    }
}
