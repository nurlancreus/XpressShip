using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;

namespace XpressShip.Application.Features.User.Commands.Toggle
{
    public record ToggleUserCommand : ICommand
    {
        public string UserId { get; set; } = string.Empty;
    }
}
