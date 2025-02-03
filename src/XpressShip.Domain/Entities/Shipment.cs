using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using XpressShip.Domain;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities.Base;
using XpressShip.Domain.Entities.Users;
using XpressShip.Domain.Enums;
using XpressShip.Domain.Extensions;
using XpressShip.Domain.Validation;

namespace XpressShip.Domain.Entities
{
    public class Shipment : BaseEntity
    {
        public string TrackingNumber { get; set; } = string.Empty;
        public ShipmentStatus Status { get; set; }
        public DateTime? EstimatedDate { get; set; }
        public ShipmentMethod Method { get; set; }
        public double Weight { get; set; }
        public string Dimensions { get; set; } = string.Empty;
        public string? Note { get; set; }
        public decimal Cost { get; set; }
        public Guid ShipmentRateId { get; set; }
        public ShipmentRate Rate { get; set; } = null!;
        public Guid? OriginAddressId { get; set; }
        public Address? OriginAddress { get; set; }
        public Guid DestinationAddressId { get; set; }
        public Address DestinationAddress { get; set; } = null!;
        public string? SenderId { get; set; }
        public Sender? Sender { get; set; }
        public Guid? ApiClientId { get; set; }
        public ApiClient? ApiClient { get; set; }

        public Payment? Payment { get; set; }

        private Address Origin => (OriginAddress ?? Sender?.Address ?? ApiClient?.Address)!;

        private Shipment() { }

        private Shipment(double weight, string dimensions, ShipmentMethod method, string? note)
        {
            ValidationRules.ValidateDimensions(dimensions);

            TrackingNumber = GenerateTrackingNumber();
            Status = ShipmentStatus.Pending;
            Weight = weight;
            Dimensions = dimensions;
            Method = method;
            Weight = weight;
            Note = note;
        }

        public static Shipment Create(double weight, string dimensions, ShipmentMethod method, string? note)
        {
            return new Shipment(weight, dimensions, method, note);
        }

        public void Update(double? weight, string? dimensions, string? method, string? note)
        {
            if (weight is double newWeight && Weight != weight)
            {
                Weight = newWeight;
            }

            if (dimensions is not null && Dimensions != dimensions)
            {
                Dimensions = dimensions;
            }

            if (!string.IsNullOrEmpty(method))
            {
                var shipmentMethod = method.EnsureEnumValueDefined<ShipmentMethod>();

                if (shipmentMethod != Method) Method = shipmentMethod;
            }

            if (!string.IsNullOrEmpty(note) && Note != note)
            {
                Note = note;
            }
        }

        public void MakePending()
        {
            Status = ShipmentStatus.Pending;
        }

        public void MakeCanceled()
        {
            Status = ShipmentStatus.Canceled;
            EstimatedDate = null;
        }

        public void MakeShipped()
        {
            Status = ShipmentStatus.Shipped;
            EstimatedDate = CalculateEstimatedDelivery();
        }

        public void MakeFailed()
        {
            Status = ShipmentStatus.Failed;
            EstimatedDate = null;
        }

        public void MakeDelivered()
        {
            Status = ShipmentStatus.Delivered;
            EstimatedDate = null;
        }
        public decimal CalculateShippingCost()
        {
            Rate.EnsureNonNull(nameof(Rate));
            Origin.EnsureNonNull(nameof(Origin));
            DestinationAddress.EnsureNonNull(nameof(DestinationAddress));

            decimal baseCost = Rate.BaseRate;

            decimal weightCost = CalculateWeightCost(Weight, Rate);
            decimal volumeCost = CalculateSizeCost(Dimensions, Rate);

            var distance = Origin.CalculateDistance(DestinationAddress);

            decimal distanceCost = CalculateDistanceCost(distance, Rate);

            decimal totalCost = CalculateDeliveryCost(baseCost + weightCost + volumeCost + distanceCost, Method, Rate);

            var taxAppliedPrice = ApplyTax(totalCost);

            return taxAppliedPrice;
        }

        public decimal ApplyTax(decimal totalCost)
        {
            DestinationAddress.EnsureNonNull(nameof(DestinationAddress));

            var taxRate = DestinationAddress.City.Country.TaxPercentage / 100;

            return totalCost * (1 - taxRate);
        }

        public static decimal CalculateDeliveryCost(decimal subtotal, ShipmentMethod method, ShipmentRate rate)
        {
            method.EnsureEnumValueDefined(nameof(method));
            rate.EnsureNonNull();

            return method switch
            {
                ShipmentMethod.Standard => subtotal,
                ShipmentMethod.Express => subtotal * (decimal)rate.ExpressRateMultiplier,
                ShipmentMethod.Overnight => subtotal * (decimal)rate.OvernightRateMultiplier,
                _ => subtotal
            };
        }

        public static decimal CalculateWeightCost(double weight, ShipmentRate rate)
        {
            weight.EnsureNonZero(nameof(weight));
            rate.EnsureNonNull(nameof(rate));

            ValidationRules.ValidateWeight(weight, rate);

            return (decimal)(weight * rate.BaseRateForKg);
        }

        public static decimal CalculateDistanceCost(double distance, ShipmentRate rate)
        {
            rate.EnsureNonNull(nameof(Rate));

            ValidationRules.ValidateDistance(distance, rate);

            return (decimal)(distance * rate.BaseRateForKm);
        }

        public static decimal CalculateSizeCost(string dimensions, ShipmentRate rate)
        {
            dimensions.EnsureNonEmpty(nameof(dimensions));
            rate.EnsureNonNull(nameof(Rate));

            int volume = CalculateVolume(dimensions);

            return CalculateSizeCost(volume, rate);
        }

        public static decimal CalculateSizeCost(int volume, ShipmentRate rate)
        {
            rate.EnsureNonNull(nameof(Rate));

            ValidationRules.ValidateVolume(volume, rate);

            return volume * (decimal)rate.BaseRateForVolume;
        }

        public static int CalculateVolume(string dimensions)
        {
            dimensions.EnsureNonEmpty(nameof(dimensions));

            ValidationRules.ValidateDimensions(dimensions);

            return dimensions.Split('x').Select(int.Parse).Aggregate((x, y) => x * y);
        }

        public DateTime CalculateEstimatedDelivery()
        {
            Origin.EnsureNonNull(nameof(Origin));
            DestinationAddress.EnsureNonNull(nameof(DestinationAddress));

            var distance = Origin.CalculateDistance(DestinationAddress);

            int baseDays = CalculateDefaultDeliveryTime(distance);
            int deliveryTime = CalculateDeliveryTime(baseDays);

            return DateTime.Now.AddDays(deliveryTime);
        }

        public int CalculateDeliveryTime(int baseDays)
        {
            return Method switch
            {
                ShipmentMethod.Standard => baseDays,
                ShipmentMethod.Express => (int)Math.Ceiling(baseDays * Rate.ExpressDeliveryTimeMultiplier),
                ShipmentMethod.Overnight => (int)Math.Ceiling(baseDays * Rate.OvernightDeliveryTimeMultiplier),
                _ => baseDays
            };
        }

        public (string name, string email) GetRecipient()
        {
            if (ApiClient is null && Sender is null) throw new ArgumentNullException("Both Api Client and Sender cannot be null");
            else if (ApiClient is not null && Sender is not null) throw new ArgumentException("Either Api Client or Sender should be null");

            var email = (ApiClient?.Email ?? Sender?.Email)!;
            var name = (ApiClient?.CompanyName ?? Sender?.UserName)!;

            return (name, email);
        }

        public (InitiatorType initiatorType, string initiatorId) GetInitiatorTypeAndId()
        {
            if (ApiClient is null && Sender is null) throw new ArgumentNullException("Both Api Client and Sender cannot be null");
            else if (ApiClient is not null && Sender is not null) throw new ArgumentException("Either Api Client or Sender should be null");

            InitiatorType initiatorType = ApiClient is not null ? InitiatorType.ApiClient : InitiatorType.Account;

            string? initiatorId = ApiClient?.ApiKey ?? Sender?.Id;

            initiatorType.EnsureEnumValueDefined(nameof(initiatorType));
            initiatorId = initiatorId.EnsureNonEmpty(nameof(initiatorId));

            return (initiatorType, initiatorId);
        }

        private static int CalculateDefaultDeliveryTime(double distance)
        {
            const double distancePerDay = 100;

            return (int)Math.Ceiling(distance / distancePerDay);
        }

        private static string GenerateTrackingNumber()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var uniquePart = new string(Enumerable.Repeat(chars, 8)
                                                  .Select(s => s[random.Next(s.Length)]).ToArray());
            return $"TRK-{DateTime.UtcNow:yyyyMMdd}-{uniquePart}";  // TRK-20240101-AB12CD34
        }
    }
}
