using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XpressShip.Application.Abstractions.Services.Mail.Template
{
    public interface IShipmentMailTemplatesService
    {
        string GenerateShipmentConfirmationEmail(string trackingNumber, string recipientName, DateTime estimatedDeliveryDate);
        string GenerateShipmentDeliveredEmail(string trackingNumber, string recipientName, DateTime deliveryDate);
        string GenerateShipmentDelayedEmail(string trackingNumber, string recipientName, string reason = "Unknown");
        string GenerateShipmentFailedEmail(string trackingNumber, string recipientName, string reason = "Unknown");
        string GenerateShipmentCanceledEmail(string trackingNumber, string recipientName);
    }

}
