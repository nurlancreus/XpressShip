using System.Diagnostics.CodeAnalysis;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.Payments.DTOs;

namespace XpressShip.Application.Features.Payments.Queries.GetAll
{
    public class GetAllPaymentsQuery : IQuery<IEnumerable<PaymentDTO>> , IParsable<GetAllPaymentsQuery>
    {
        public Guid? ClientId { get; set; }
        public string? SenderId { get; set; }

        public static GetAllPaymentsQuery Parse(string s, IFormatProvider? provider)
        {
            if (TryParse(s, provider, out var result)) return result;
            
            throw new FormatException("Invalid format for GetAllPaymentsQuery.");
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out GetAllPaymentsQuery result)
        {
            result = null;

            if (string.IsNullOrEmpty(s)) return false;
            
            var queryParams = s.Split('&')
                               .Select(part => part.Split('='))
                               .ToDictionary(split => split[0], split => split.Length > 1 ? split[1] : null);

            result = new GetAllPaymentsQuery
            {
                ClientId = queryParams.TryGetValue("clientId", out var clientIdStr) && Guid.TryParse(clientIdStr, out var clientId)
                           ? clientId
                           : null,
                SenderId = queryParams.TryGetValue("senderId", out var senderId) ? senderId : null
            };

            return true;
        }
    }
}
