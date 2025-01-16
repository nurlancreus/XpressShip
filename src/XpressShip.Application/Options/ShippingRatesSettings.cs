using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XpressShip.Application.Options
{
    public class ShippingRatesSettings
    {
        public decimal BaseRateForKg { get; set; }
        public decimal BaseRateForKm { get; set; }
        public decimal BaseRateForVolume { get; set; }
        public decimal ExpressRateMultiplier { get; set; }
        public decimal OvernightRateMultiplier { get; set; }
        public int DefaultDays { get; set; }
        public double DefaultDistance { get; set; }
        public double ExpressDeliveryMultiplier { get; set; }
        public double OvernightDeliveryMultiplier { get; set; }
    }
}
