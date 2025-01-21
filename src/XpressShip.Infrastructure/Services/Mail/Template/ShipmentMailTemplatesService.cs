using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Interfaces.Services.Mail.Template;

namespace XpressShip.Infrastructure.Services.Mail.Template
{
    public class ShipmentMailTemplatesService : IShipmentMailTemplatesService
    {
        public string GenerateShipmentConfirmationEmail(string trackingNumber, string recipientName, DateTime estimatedDeliveryDate)
        {
            return $@"
                <html>
                <body>
                    <h1>Shipment Confirmation</h1>
                    <p>Dear {recipientName},</p>
                    <p>Your shipment with tracking number <b>{trackingNumber}</b> has been confirmed.</p>
                    <p>Estimated delivery date: <b>{estimatedDeliveryDate:F}</b>.</p>
                    <p>Thank you for choosing XpressShip!</p>
                </body>
                </html>
            ";
        }

        public string GenerateShipmentDeliveredEmail(string trackingNumber, string recipientName, DateTime deliveryDate)
        {
            return $@"
                <html>
                <body>
                    <h1>Shipment Delivered</h1>
                    <p>Dear {recipientName},</p>
                    <p>Your shipment with tracking number <b>{trackingNumber}</b> was successfully delivered on <b>{deliveryDate:F}</b>.</p>
                    <p>Thank you for using XpressShip!</p>
                </body>
                </html>
            ";
        }

        public string GenerateShipmentDelayedEmail(string trackingNumber, string recipientName, string reason = "Unknown")
        {
            return $@"
                <html>
                <body>
                    <h1>Shipment Delay Notification</h1>
                    <p>Dear {recipientName},</p>
                    <p>We regret to inform you that your shipment with tracking number <b>{trackingNumber}</b> has been delayed.</p>
                    <p>Reason: <b>{reason}</b>.</p>
                    <p>We apologize for the inconvenience and are working to resolve the issue promptly.</p>
                </body>
                </html>
            ";
        }

        public string GenerateShipmentFailedEmail(string trackingNumber, string recipientName, string reason = "Unknown")
        {
            return $@"
                <html>
                <body>
                    <h1>Shipment Failed</h1>
                    <p>Dear {recipientName},</p>
                    <p>We regret to inform you that your shipment with tracking number <b>{trackingNumber}</b> has failed.</p>
                    <p>Reason: <b>{reason}</b>.</p>
                    <p>We apologize for the inconvenience and are working to resolve the issue promptly.</p>
                </body>
                </html>
            ";
        }

        public string GenerateShipmentCanceledEmail(string trackingNumber, string recipientName)
        {
            return $@"
                <html>
                <body>
                    <h1>Shipment Canceled</h1>
                    <p>Dear {recipientName},</p>
                    <p>Your shipment with tracking number <b>{trackingNumber}</b> has been canceled.</p>
                    <p>If you have any questions, please contact us.</p>
                </body>
                </html>
            ";
        }
    }
}
