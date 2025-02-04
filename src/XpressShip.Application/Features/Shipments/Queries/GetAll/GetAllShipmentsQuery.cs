using System.Diagnostics.CodeAnalysis;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Features.Shipments.DTOs;

namespace XpressShip.Application.Features.Shipments.Queries.GetAll
{
    public record GetAllShipmentsQuery : IQuery<IEnumerable<ShipmentDTO>>, IParsable<GetAllShipmentsQuery>
    {
        public string? Status { get; set; }
        public string? OriginCountry {  get; set; }
        public string? OriginCity { get; set; }

        public string? DestinationCountry { get; set; }
        public string? DestinationCity { get; set; }

        public static GetAllShipmentsQuery Parse(string s, IFormatProvider? provider)
        {
            if (TryParse(s, provider, out var result)) return result;
            
            throw new FormatException("Invalid format for GetAllShipmentsQuery.");
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out GetAllShipmentsQuery result)
        {
            result = null;

            if (string.IsNullOrEmpty(s)) return false;
            
            var queryParams = s.Split('&')
                               .Select(part => part.Split('='))
                               .ToDictionary(split => split[0], split => split[1]);

            result = new GetAllShipmentsQuery
            {
                Status = queryParams.TryGetValue("status", out var status) ? status : null,
                OriginCountry = queryParams.TryGetValue("originCountry", out var originCountry) ? originCountry : null,
                OriginCity = queryParams.TryGetValue("originCity", out var originCity) ? originCity : null,
                DestinationCountry = queryParams.TryGetValue("destinationCountry", out var destinationCountry) ? destinationCountry : null,
                DestinationCity = queryParams.TryGetValue("destinationCity", out var destinationCity) ? destinationCity : null
            };

            return true;
        }
    }
}
