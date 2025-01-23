using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Interfaces;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Interfaces.Services.Session;
using XpressShip.Application.Responses;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Exceptions;

namespace XpressShip.Application.Features.Shipments.Commands.Delete
{
    public class DeleteShipmentHandler : ICommandHandler<DeleteShipmentCommand>
    {
        private readonly IApiClientSessionService _clientSessionService;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteShipmentHandler(IApiClientSessionService clientSessionService, IShipmentRepository shipmentRepository, IUnitOfWork unitOfWork)
        {
            _clientSessionService = clientSessionService;
            _shipmentRepository = shipmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(DeleteShipmentCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentRepository.Table
                                .Include(s => s.Rate)
                                    .ThenInclude(r => r.Shipments)
                                .Include(s => s.OriginAddress)
                                .Include(s => s.DestinationAddress)
                                .Include(s => s.ApiClient)
                                    .ThenInclude(c => c.Address)
                                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (shipment is null) return Result<Unit>.Failure(Error.NotFoundError(nameof(shipment)));

            if (shipment.ApiClient is not null)
            {
                var keys = _clientSessionService.GetClientApiAndSecretKey();

                if (keys is (string apiKey, string secretKey))
                {
                    if (shipment.ApiClient.ApiKey != apiKey || shipment.ApiClient.SecretKey != secretKey)
                    {
                        return Result<Unit>.Failure(Error.UnauthorizedError("You are not authorized to delete this shipment"));
                    }
                }
            }

            _shipmentRepository.Delete(shipment);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
