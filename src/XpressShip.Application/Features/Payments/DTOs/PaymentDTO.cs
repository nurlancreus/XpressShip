using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Features.Shipments.DTOs;
using XpressShip.Domain.Entities;
using XpressShip.Domain.Enums;

namespace XpressShip.Application.Features.Payments.DTOs
{
    public record PaymentDTO
    {
        public string TransactionId { get; set; }
        public ShipmentDTO Shipment { get; set; }

        public string Method { get; set; }
        public string Status { get; set; }
        public string Currency { get; set; }
        public PaymentDTO(Payment payment)
        {
            Shipment = new ShipmentDTO(payment.Shipment);
            TransactionId = payment.TransactionId;
            Method = payment.Method.ToString();
            Status = payment.Status.ToString();
            Currency = payment.Currency.ToString();
        }
    }
}
