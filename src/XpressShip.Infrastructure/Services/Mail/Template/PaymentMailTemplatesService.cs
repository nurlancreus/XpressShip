using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Interfaces.Services.Mail.Template;

namespace XpressShip.Infrastructure.Services.Mail.Template
{
    public class PaymentMailTemplatesService : IPaymentMailTemplatesService
    {
        public string GeneratePaymentConfirmationEmail(string transactionId, string recipientName, decimal amount, string currency, DateTime date)
        {
            return $@"
                <html>
                <body>
                    <h1>Payment Confirmation</h1>
                    <p>Dear {recipientName},</p>
                    <p>Your payment has been successfully processed.</p>
                    <p>Transaction ID: <b>{transactionId}</b></p>
                    <p>Amount: <b>{amount} {currency}</b></p>
                    <p>Date: <b>{date:F}</b></p>
                    <p>Thank you for choosing XpressShip!</p>
                </body>
                </html>
            ";
        }

        public string GeneratePaymentFailedEmail(string recipientName, string transactionId, string reason = "Unknown")
        {
            return $@"
                <html>
                <body>
                    <h1>Payment Failed</h1>
                    <p>Dear {recipientName},</p>
                    <p>We encountered an issue with your payment.</p>
                    <p>Transaction ID: <b>{transactionId}</b></p>
                    <p>Reason: <b>{reason}</b></p>
                    <p>Please try again or contact our support team for assistance.</p>
                </body>
                </html>
            ";
        }

        public string GenerateRefundNotificationEmail(string transactionId, string recipientName, decimal refundedAmount, string currency, DateTime refundDate)
        {
            return $@"
                <html>
                <body>
                    <h1>Refund Processed</h1>
                    <p>Dear {recipientName},</p>
                    <p>Your refund has been processed successfully.</p>
                    <p>Transaction ID: <b>{transactionId}</b></p>
                    <p>Refunded Amount: <b>{refundedAmount} {currency}</b></p>
                    <p>Date: <b>{refundDate:F}</b></p>
                    <p>We apologize for any inconvenience caused.</p>
                </body>
                </html>
            ";
        }

        public string GeneratePaymentCanceledEmail(string transactionId, string recipientName, string reason = "Unknown")
        {
            return $@"
                <html>
                <body>
                    <h1>Payment Canceled</h1>
                    <p>Dear {recipientName},</p>
                    <p>Your payment has been canceled.</p>
                    <p><strong>Transaction ID:</strong> {transactionId}</p>
                    <p><strong>Reason:</strong> {reason}</p>
                    <p>If you believe this was a mistake, please contact our support team for assistance.</p>
                </body>
                </html>
            ";
        }
    }
}
