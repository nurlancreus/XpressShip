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
        public string TrackingNumber { get; set; } = string.Empty; 
        public ShipmentStatus Status { get; set; }  
        public DateTime EstimatedDate { get; set; }
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

        public Guid? ApiClientId { get; set; } 
        public ApiClient? ApiClient { get; set; } 

        public Payment? Payment { get; set; }

        private Shipment()
        {
            
        }

        private Shipment(double weight, string dimensions, ShipmentMethod method, string? note)
        {
            TrackingNumber = Generator.GenerateTrackingNumber();
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
