namespace XpressShip.Tests.Common.Constants
{
    public static partial class Constants
    {
        public static class ShipmentRate
        {
            public const string Name = "Small Package - Local";
            public const string Description = "Rate for small packages within local regions.";
            public const decimal BaseRate = 10.00m;
            public const double MinWeight = 0;
            public const double MaxWeight = 5;
            public const double MinDistance = 0;
            public const double MaxDistance = 100;
            public const double MinVolume = 0;
            public const double MaxVolume = 20;
            public const double BaseRateForKm = 0.05;
            public const double BaseRateForKg = 1.5;
            public const double BaseRateForVolume = 2.0;
            public const double ExpressRateMultiplier = 1.2;
            public const double OvernightRateMultiplier = 1.5;
            public const double ExpressDeliveryMultiplier = 0.8;
            public const double OvernightDeliveryMultiplier = 0.5;
        }

        public static class MediumShipmentRate
        {
            public const string Name = "Medium Package - Local";
            public const string Description = "Rate for medium packages within local regions.";
            public const decimal BaseRate = 20.00m;
            public const double MinWeight = 5;
            public const double MaxWeight = 20;
            public const double MinDistance = 0;
            public const double MaxDistance = 200;
            public const double MinVolume = 20;
            public const double MaxVolume = 50;
            public const double BaseRateForKm = 0.10;
            public const double BaseRateForKg = 2.0;
            public const double BaseRateForVolume = 3.0;
            public const double ExpressRateMultiplier = 1.3;
            public const double OvernightRateMultiplier = 1.6;
            public const double ExpressDeliveryMultiplier = 0.7;
            public const double OvernightDeliveryMultiplier = 0.4;
        }

        public static class LargeShipmentRate
        {
            public const string Name = "Large Package - Local";
            public const string Description = "Rate for large packages within local regions.";
            public const decimal BaseRate = 30.00m;
            public const double MinWeight = 20;
            public const double MaxWeight = 50;
            public const double MinDistance = 0;
            public const double MaxDistance = 300;
            public const double MinVolume = 50;
            public const double MaxVolume = 100;
            public const double BaseRateForKm = 0.15;
            public const double BaseRateForKg = 2.5;
            public const double BaseRateForVolume = 4.0;
            public const double ExpressRateMultiplier = 1.4;
            public const double OvernightRateMultiplier = 1.7;
            public const double ExpressDeliveryMultiplier = 0.6;
            public const double OvernightDeliveryMultiplier = 0.3;
        }
    }
}