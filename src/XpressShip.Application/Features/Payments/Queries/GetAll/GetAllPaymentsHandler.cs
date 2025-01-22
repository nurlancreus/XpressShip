using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Payments.DTOs;
using XpressShip.Application.Interfaces.Repositories;
using XpressShip.Application.Interfaces.Services.Session;
using XpressShip.Application.Responses;
using XpressShip.Domain.Entities;

namespace XpressShip.Application.Features.Payments.Queries.GetAll
{
    public class GetAllPaymentsHandler : IRequestHandler<GetAllPaymentsQuery, ResponseWithData<IEnumerable<PaymentDTO>>>
    {
        private readonly IApiClientSessionService _apiClientSessionService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly bool IsAdmin = true;

        public GetAllPaymentsHandler(IApiClientSessionService apiClientSessionService, IPaymentRepository paymentRepository)
        {
            _apiClientSessionService = apiClientSessionService;
            _paymentRepository = paymentRepository;
        }

        public async Task<ResponseWithData<IEnumerable<PaymentDTO>>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
        {
            if (!IsAdmin) throw new UnauthorizedAccessException("You're not authorized to get payment details!");

            var payments = _paymentRepository.Table
                             .Include(p => p.Shipment)
                                 .ThenInclude(s => s.ApiClient)
                                    .ThenInclude(c => c!.Address)
                             .Include(p => p.Shipment)
                                .ThenInclude(s => s.OriginAddress)
                             .Include(p => p.Shipment)
                                .ThenInclude(s => s.DestinationAddress)
                             .AsNoTracking()
                             .AsQueryable();

            if (request.ClientId is Guid clientId)
            {
                payments = payments.Where(p => p.Shipment.ApiClient != null && p.Shipment.ApiClient.Id == clientId);
            }

            return new ResponseWithData<IEnumerable<PaymentDTO>>
            {
                IsSuccess = true,
                Data = await payments.Select(p => new PaymentDTO(p)).ToListAsync(cancellationToken),
                Message = "Payments retrieved successfully"
            };
        }
    }
}
