using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XpressShip.Application.Interfaces.Services.Session
{
    public interface IClientSessionService
    {
        (string apiKey, string secretKey)? GetClientApiAndSecretKey(bool throwException = true);
        string? GetApiKey(bool throwException = true);
        string? GetSecretKey(bool throwException = true);
    }
}
