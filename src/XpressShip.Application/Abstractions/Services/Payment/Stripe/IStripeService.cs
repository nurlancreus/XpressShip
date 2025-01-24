using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.Abstractions.Services.Payment;

namespace XpressShip.Application.Abstractions.Services.Payment.Stripe
{
    public interface IStripeService : IPaymentGatewayService
    {
    }
}
