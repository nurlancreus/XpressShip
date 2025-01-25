using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.DTOs.Token;

namespace XpressShip.Application.Features.Auth.Common.RefreshLogin
{
    public record RefreshLoginCommand : ICommand<TokenDTO>
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
