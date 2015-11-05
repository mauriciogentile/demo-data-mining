using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FileHelpers;

namespace Demo.Data
{
    [DelimitedRecord("|")]
    public class Property
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public int Id { get; set; }
        public int Bathrooms { get; set; }
        public int Bedrooms { get; set; }
        public bool HasParking { get; set; }
        public int SquareMts { get; set; }
        public int TotalSquareMts { get; set; }
        public int Age { get; set; }
        public int DistanceToStation { get; set; }
        public bool WithFurniture { get; set; }
        public int Price { get; set; }
        public bool HasBackyard { get; set; }

        [NotMapped]
        public OperationType OperationType { get; set; }

        [NotMapped]
        public ZoneReputation ZoneReputation { get; set; }

        [NotMapped]
        public PropertyType PropertyType { get; set; }

        [Column("PropertyType")]
        public string PropertyTypeString
        {
            get { return PropertyType.ToString(); }
            set { PropertyType = (PropertyType)Enum.Parse(typeof(PropertyType), value); }
        }

        [Column("OperationType")]
        public string OperationTypeString
        {
            get { return OperationType.ToString(); }
            set { OperationType = (OperationType)Enum.Parse(typeof(OperationType), value); }
        }

        [Column("ZoneReputation")]
        public string ZoneReputationString
        {
            get { return ZoneReputation.ToString(); }
            set { ZoneReputation = (ZoneReputation)Enum.Parse(typeof(ZoneReputation), value); }
        }
    }

    public enum OperationType
    {
        Sale,
        Rent
    }
}
