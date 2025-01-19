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

        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public PaymentCurrency Currency { get; set; }
        public PaymentDTO(Payment payment)
        {
            Shipment = new ShipmentDTO(payment.Shipment);
            TransactionId = payment.TransactionId;
            Method = payment.Method;
            Status = payment.Status;
            Currency = payment.Currency;
        }
    }
}
