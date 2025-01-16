using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain;
using XpressShip.Domain.Entities.Base;
using XpressShip.Domain.Enums;

namespace XpressShip.Domain.Entities
{
    public class Shipment : BaseEntity
    {
        public string TrackingNumber { get; set; } = string.Empty;  // Unique tracking number
        public ShipmentStatus Status { get; set; }  // Using enum for status
        public DateTime EstimatedDate { get; set; }
        public ShipmentMethod Method { get; set; }  // Shipping method using enum
        public double Weight { get; set; }  // Weight of the package
        public string Dimensions { get; set; } = string.Empty;  // Package dimensions (e.g., "10x10x10 cm")
        public string? Note { get; set; }  // Additional note for the shipment
        public decimal Cost { get; set; }  // Cost of the shipment
        public Guid ShipmentRateId { get; set; }  // Foreign key to ShipmentRate
        public ShipmentRate Rate { get; set; } = null!;
        public Guid? OriginAddressId { get; set; }  // Foreign key to Address
        public Address? OriginAddress { get; set; } // Navigation property to Origin Address if null then origin address taken from client (company)
        public Guid DestinationAddressId { get; set; }  // Foreign key to Address
        public Address DestinationAddress { get; set; } = null!;  // Navigation property to Destination Address

        public Guid ApiClientId { get; set; } // Foreign key to ApiClient
        public ApiClient ApiClient { get; set; } = null!; // Navigation property to ApiClient

        private Shipment(double weight, string dimensions, ShipmentMethod method, string? note)
        {
            TrackingNumber = IGenerator.GenerateTrackingNumber();
            Status = ShipmentStatus.Pending;
            Weight = weight;
            Dimensions = dimensions;
            Method = method;
            Weight = weight;
        }

        public static Shipment Create(double weight, string dimensions, ShipmentMethod method, string? note)
        {
            return new Shipment(weight, dimensions, method, note);
        }
    }
}
