using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XpressShip.Domain.Enums
{
    public enum ShipmentStatus
    {
        Pending,
        Shipped,
        InTransit,
        OutForDelivery,
        Delivered,
        Failed,
        Canceled
    }
}
