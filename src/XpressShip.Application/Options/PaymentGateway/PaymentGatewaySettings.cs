﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XpressShip.Application.Options.PaymentGateway
{
    public class PaymentGatewaySettings
    {
        public StripeSettings Stripe { get; set; } = null!;
    }
}
