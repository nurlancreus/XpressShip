﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.DTOs.Token;

namespace XpressShip.Application.Features.Auth.Sender.Login
{
    public record LoginSenderCommand : ICommand<TokenDTO>
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
