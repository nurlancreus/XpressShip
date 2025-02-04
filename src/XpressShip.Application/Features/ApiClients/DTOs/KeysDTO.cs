using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XpressShip.Application.Features.ApiClients.DTOs
{
    public record KeysDTO
    {
        public string RawSecretKey { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;

        public KeysDTO(string rawSecretKey, string apiKey)
        {
            RawSecretKey = rawSecretKey;
            ApiKey = apiKey;
        }
    }
}
