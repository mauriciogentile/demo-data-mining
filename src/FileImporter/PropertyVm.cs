namespace FileImporter
{
    public class PropertyVm
    {
        public int Id { get; set; }
        public string OperationType { get; set; }
        public string ZoneReputation { get; set; }
        public string PropertyType { get; set; }
        public int Bathrooms { get; set; }
        public int Bedrooms { get; set; }
        public bool HasParking { get; set; }
        public int SquareMts { get; set; }
        public int Age { get; set; }
        public int DistanceToStation { get; set; }
        public int Price { get; set; }
        public bool HasError
        {
            get { return !string.IsNullOrEmpty(Error); }
        }
        public string Error { get; set; }
    }
}
