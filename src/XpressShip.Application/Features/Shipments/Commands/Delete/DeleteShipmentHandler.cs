using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions;
using XpressShip.Application.Abstractions.Repositories;
using XpressShip.Application.Abstractions.Services.Session;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Application.Responses;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Exceptions;

namespace XpressShip.Application.Features.Shipments.Commands.Delete
{
    public class DeleteShipmentHandler : ICommandHandler<DeleteShipmentCommand>
    {
        private readonly IApiClientSession _apiClientSession;
        private readonly IJwtSession _jwtSession;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteShipmentHandler(IApiClientSession apiClientSession, IJwtSession jwtSession, IShipmentRepository shipmentRepository, IUnitOfWork unitOfWork)
        {
            _apiClientSession = apiClientSession;
            _jwtSession = jwtSession;
            _shipmentRepository = shipmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(DeleteShipmentCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentRepository.Table
                                .Include(s => s.ApiClient)
                                .Include(s => s.Sender)
                                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (shipment is null) return Result<Unit>.Failure(Error.NotFoundError("Shipment is not found"));

            if (shipment.ApiClient is not null)
            {
                var keysResult = _apiClientSession.GetClientApiAndSecretKey();

                if (keysResult.IsSuccess)
                {
                    if (shipment.ApiClient.ApiKey != keysResult.Value.apiKey || shipment.ApiClient.SecretKey != keysResult.Value.secretKey)
                    {
                        return Result<Unit>.Failure(Error.UnauthorizedError("You are not authorized to delete this shipment"));
                    }
                }
            }
            else if (shipment.Sender is not null)
            {
                var senderIdResult = _jwtSession.GetUserId();

                if (senderIdResult.IsFailure) return Result<Unit>.Failure(senderIdResult.Error);

                if (shipment.Sender.Id != senderIdResult.Value)
                {
                    return Result<Unit>.Failure(Error.UnauthorizedError("You are not authorized to delete this shipment"));
                }
            }
            else return Result<Unit>.Failure(Error.UnexpectedError("Shipment initiator is not found"));

            _shipmentRepository.Delete(shipment);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
