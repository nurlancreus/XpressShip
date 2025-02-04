using Microsoft.EntityFrameworkCore;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Application.Features.Rates.DTOs;
using XpressShip.Domain.Abstractions;

namespace XpressShip.Application.Features.Rates.Queries.GetAll
{
    public class GetAllRatesHandler : IQueryHandler<GetAllRatesQuery, IEnumerable<ShipmentRateDTO>>
    {
        private readonly IJwtSession _jwtSession;
        private readonly IShipmentRateRepository _shipmentRateRepository;

        public GetAllRatesHandler(IJwtSession jwtSession, IShipmentRateRepository shipmentRateRepository)
        {
            _jwtSession = jwtSession;
            _shipmentRateRepository = shipmentRateRepository;
        }

        public async Task<Result<IEnumerable<ShipmentRateDTO>>> Handle(GetAllRatesQuery request, CancellationToken cancellationToken)
        {
            var isAdminResult = _jwtSession.IsAdminAuth();

            if (isAdminResult.IsFailure) return Result<IEnumerable<ShipmentRateDTO>>.Failure(isAdminResult.Error);

            var rates = _shipmentRateRepository.Table;

            var dtos = await rates.Select(r => new ShipmentRateDTO(r)).ToListAsync(cancellationToken);

            return Result<IEnumerable<ShipmentRateDTO>>.Success(dtos);
        }
    }
}
