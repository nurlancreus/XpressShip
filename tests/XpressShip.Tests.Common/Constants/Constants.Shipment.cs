using XpressShip.Domain.Enums;

namespace XpressShip.Tests.Common.Constants
{
    public static partial class Constants
    {
        public static class Shipment
        {
            public const string Note = "Handle with care";
            public const string Dimensions = "2x2x3";
            public const double Weight = 5;
            public const ShipmentMethod Method = ShipmentMethod.Standard;

            public const string NewNote = "Please handle with care";
            public const string NewDimensions = "3x3x4";
            public const double NewWeight = 3;
            public const ShipmentMethod NewMethod = ShipmentMethod.Overnight;
        }

        public static class MediumShipment
        {
            public const string Note = "Fragile contents";
            public const string Dimensions = "4x4x3";
            public const double Weight = 20;
            public const ShipmentMethod Method = ShipmentMethod.Express;

            public const string NewNote = "Please handle carefully, fragile contents";
            public const string NewDimensions = "3x3x4";
            public const double NewWeight = 15.0;
            public const ShipmentMethod NewMethod = ShipmentMethod.Overnight;
        }

        public static class LargeShipment
        {
            public const string Note = "Heavy and bulky";
            public const string Dimensions = "5x5x4";
            public const double Weight = 50.0;
            public const ShipmentMethod Method = ShipmentMethod.Standard;

            public const string NewNote = "Please handle with extra care, heavy and bulky";
            public const string NewDimensions = "4x4x5";
            public const double NewWeight = 45.0;
            public const ShipmentMethod NewMethod = ShipmentMethod.Express;
        }
    }
}