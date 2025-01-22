using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XpressShip.Infrastructure.SignalR.Constants
{
    public static class ReceiveFunctionNames
    {
        public static class PaymentHub
        {
            public const string PaymentSucceededMessage = "receivePaymentSucceededMessage";
            public const string PaymentCanceledMessage = "receivePaymentCanceledMessage";
            public const string PaymentRefundedMessage = "receivePaymentRefundedMessage";
            public const string PaymentFailedMessage = "receivePaymentFailedMessage";
        }

        public static class ShipmentHub
        {
            public const string ShipmentShippedMessage = "receiveShipmentShipped";
            public const string ShipmentDeliveredMessage = "receiveShipmentDelivered";
            public const string ShipmentCanceledMessage = "receiveShipmentCanceled";
            public const string ShipmentDelayedMessage = "receiveShipmentDelayed";
            public const string ShipmentFailedMessage = "receiveShipmentFailed";
        }

        public static class AdminHub
        {
            public const string AdminNewShipmentMessage = "receiveAdminNewShipment";
            public const string AdminShipmentUpdatedMessage = "receiveAdminShipmentUpdated";
            public const string AdminNewApiClientMessage = "receiveAdminNewApiClient";
            public const string AdminApiClientUpdatedMessage = "receiveAdminApiClientUpdated";
            public const string AdminShipmentIssueMessage = "receiveAdminShipmentIssue";
        }
    }
}
