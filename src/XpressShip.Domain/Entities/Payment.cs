using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Entities.Base;
using XpressShip.Domain.Enums;

namespace XpressShip.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public string TransactionId { get; set; } = string.Empty;
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public PaymentCurrency Currency { get; set; }
        public Guid ShipmentId { get; set; }
        public Shipment Shipment { get; set; } = null!;

        private Payment()
        {

        }

        private Payment(PaymentMethod method, PaymentCurrency currency)
        {
            Status = PaymentStatus.Pending;
            Method = method;
            Currency = currency;
        }

        public static Payment Create(PaymentMethod method, PaymentCurrency currency)
        {
            return new Payment(method, currency);

        }

    }
}
