﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XpressShip.Application.Abstractions.Services.Mail.Template
{
    public interface IPaymentMailTemplatesService
    {
        string GeneratePaymentConfirmationEmail(string transactionId, string recipientName, decimal amount, string currency, DateTime date);
        string GeneratePaymentFailedEmail(string transactionId, string recipientName, string reason = "Unknown");
        string GeneratePaymentRefundedEmail(string transactionId, string recipientName, decimal refundedAmount, string currency, DateTime refundDate);
        string GeneratePaymentCanceledEmail(string transactionId, string recipientName, string reason = "Unknown");

    }
}
