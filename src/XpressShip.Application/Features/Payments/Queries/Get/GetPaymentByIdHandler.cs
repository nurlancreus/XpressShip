using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Payments.DTOs;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Interfaces.Services.Session;
using XpressShip.Application.Responses;

namespace XpressShip.Application.Features.Payments.Queries.Get
{
    public class GetPaymentByIdHandler : IRequestHandler<GetPaymentByIdQuery, ResponseWithData<PaymentDTO>>
    {
        private readonly IApiClientSessionService _apiClientSessionService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly bool IsAdmin = true;

        public GetPaymentByIdHandler(IApiClientSessionService apiClientSessionService, IPaymentRepository paymentRepository)
        {
            _apiClientSessionService = apiClientSessionService;
            _paymentRepository = paymentRepository;
        }

        public async Task<ResponseWithData<PaymentDTO>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.Table
                             .Include(p => p.Shipment)
                                 .ThenInclude(s => s.ApiClient)
                                    .ThenInclude(c => c!.Address)
                             .Include(p => p.Shipment)
                                .ThenInclude(s => s.OriginAddress)
                             .Include(p => p.Shipment)
                                .ThenInclude(s => s.DestinationAddress)
                             .AsNoTracking()
                             .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (payment is null) throw new Exception();

            var keys = _apiClientSessionService.GetClientApiAndSecretKey();

            if (keys is (string apikey, string secretKey))
            {
                if (payment.Shipment.ApiClient is null || payment.Shipment.ApiClient.ApiKey != apikey || payment.Shipment.ApiClient.SecretKey != secretKey)
                    throw new UnauthorizedAccessException("You're not authorized to get payment details!");

            }
            else if (!IsAdmin) throw new UnauthorizedAccessException("You're not authorized to get payment details!");

            return new ResponseWithData<PaymentDTO>
            {
                IsSuccess = true,
                Data = new PaymentDTO(payment),
                Message = "Payment details retrieved successfully"
            };
        }
    }
}
