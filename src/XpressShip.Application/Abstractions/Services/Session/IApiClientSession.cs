using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Abstractions;

namespace XpressShip.Application.Abstractions.Services.Session
{
    public interface IApiClientSession
    {
        Result<Guid> GetClientId();
        Result<(string apiKey, string secretKey)> GetClientApiAndSecretKey();
        Result<string> GetApiKey();
        Result<string> GetSecretKey();
    }
}
