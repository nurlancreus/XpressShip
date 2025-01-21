﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XpressShip.Application.Notifications.Payment
{
    public record PaymentCanceledNotification : INotification
    {
        public string TransactionId { get; set; } = string.Empty;
    }
}
