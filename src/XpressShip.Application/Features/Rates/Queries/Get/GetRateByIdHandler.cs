using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Application.Features.Rates.DTOs;
using XpressShip.Domain.Abstractions;

namespace XpressShip.Application.Features.Rates.Queries.Get
{
    public class GetRateByIdHandler : IQueryHandler<GetRateByIdQuery, ShipmentRateDTO>
    {
        private readonly IJwtSession _jwtSession;
        private readonly IShipmentRateRepository _shipmentRateRepository;

        public GetRateByIdHandler(IJwtSession jwtSession, IShipmentRateRepository shipmentRateRepository)
        {
            _jwtSession = jwtSession;
            _shipmentRateRepository = shipmentRateRepository;
        }

        public async Task<Result<ShipmentRateDTO>> Handle(GetRateByIdQuery request, CancellationToken cancellationToken)
        {
            var isAdminResult = _jwtSession.IsAdminAuth();

            if(isAdminResult.IsFailure) return Result<ShipmentRateDTO>.Failure(isAdminResult.Error);

            var rate = await _shipmentRateRepository.GetByIdAsync(request.Id, false, cancellationToken);

            if (rate is null) return Result<ShipmentRateDTO>.Failure(Error.NotFoundError("Rate is not found"));

            return Result<ShipmentRateDTO>.Success(new ShipmentRateDTO(rate));
        }
    }
}
