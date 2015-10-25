using System;
using System.Linq;
using System.Text;

namespace Demo.Data
{
    public static class PropertyExtensions
    {
        private const int LandSquareMtRefPrice = 100;
        private const int BadAreaRefPrice = 250;
        private const int FairAreaRefPrice = 300;
        private const int GoodAreaRefPrice = 400;
        private const int ExcellentAreaRefPrice = 600;
        static readonly Random random = new Random();

        public static double CalculateRentPrice(this Property property)
        {
            var rentPrice = CalculateSalePrice(property) / random.Next(200, 220);
            if (property.PropertyType == PropertyType.Apartment)
            {
                rentPrice *= 1.2;
            }
            return Math.Round(rentPrice / 100d, 0) * 100;
        }

        public static double CalculateSalePrice(this Property property)
        {
            double price = 0;

            switch (property.ZoneReputation)
            {
                case ZoneReputation.Bad:
                    price = property.SquareMts * BadAreaRefPrice;
                    break;
                case ZoneReputation.Excellent:
                    price = property.SquareMts * ExcellentAreaRefPrice;
                    break;
                case ZoneReputation.Fair:
                    price = property.SquareMts * FairAreaRefPrice;
                    break;
                case ZoneReputation.Good:
                    price = property.SquareMts * GoodAreaRefPrice;
                    break;
            }


            price += (property.TotalSquareMts - property.SquareMts) * LandSquareMtRefPrice;
            price *= property.DistanceToStation < 1000 ? 1.5 : (property.DistanceToStation < 2000 ? 1.25 : 1);
            price *= 1 - (property.Age * .005);
            price *= property.Bathrooms * 1.05;
            price *= property.Bedrooms * 1.1;
            price *= property.HasBackyard ? 1.2 : 1;
            price *= property.HasParking ? 1.2 : 1;
            price *= property.WithFurniture ? 1.1 : 1;

            switch (property.PropertyType)
            {
                case PropertyType.Attached:
                case PropertyType.Apartment:
                    price *= 1;
                    break;
                case PropertyType.Detached:
                    price *= 1.2;
                    break;
                case PropertyType.Maisonette:
                    price *= .9;
                    break;
            }

            price *= 1 + (random.NextInt16(-1, 1) * random.NextDouble() / 10);
            return Math.Round(price / 1000d, 0) * 1000;
        }

        public static void Randomize(this Property property)
        {
            property.WithFurniture = random.NextDouble() < 0.1;
            property.OperationType = random.Next(9) <= 5 ? OperationType.Rent : OperationType.Sale;

            var houseTypeRnd = random.Next(10);

            if (houseTypeRnd < 3)
                property.PropertyType = PropertyType.Apartment;
            else if (houseTypeRnd < 6)
                property.PropertyType = PropertyType.Attached;
            else if (houseTypeRnd < 8)
                property.PropertyType = PropertyType.Detached;
            else
                property.PropertyType = PropertyType.Maisonette;

            var reputationRnd = random.Next(10);

            if (reputationRnd < 2)
                property.ZoneReputation = ZoneReputation.Excellent;
            else if (houseTypeRnd < 4)
                property.ZoneReputation = ZoneReputation.Bad;
            else if (houseTypeRnd < 7)
                property.ZoneReputation = ZoneReputation.Good;
            else
                property.ZoneReputation = ZoneReputation.Fair;

            var roomSize = random.Next(2, 4);

            switch (property.PropertyType)
            {
                case PropertyType.Apartment:
                    property.Age = random.NextInt16(0, 25);
                    property.Bathrooms = random.NextInt16(1, 3);
                    property.Bedrooms = random.NextInt16(1, 3);
                    property.DistanceToStation = random.NextInt16(50, 1000);
                    property.SquareMts = roomSize * property.Bedrooms + random.NextInt16(50, 80);
                    property.HasParking = random.NextDouble() < .2;
                    property.TotalSquareMts = property.SquareMts;
                    break;

                case PropertyType.Attached:
                    property.Age = random.NextInt16(0, 50);
                    property.Bathrooms = random.NextInt16(1, 4);
                    property.Bedrooms = random.NextInt16(2, 4);
                    property.DistanceToStation = random.NextInt16(50, 2000);
                    property.SquareMts = roomSize * property.Bedrooms + random.NextInt16(100, 250);
                    property.TotalSquareMts = property.SquareMts + random.NextInt16(20, 60);
                    property.HasParking = random.NextDouble() < .8;
                    break;

                case PropertyType.Detached:
                    property.Age = random.NextInt16(0, 50);
                    property.Bathrooms = random.NextInt16(2, 4);
                    property.Bedrooms = random.NextInt16(3, 5);
                    property.DistanceToStation = random.NextInt16(100, 3000);
                    property.SquareMts = roomSize * property.Bedrooms + random.NextInt16(150, 250);
                    property.TotalSquareMts = property.SquareMts + random.Next(50, 200);
                    property.HasParking = true;
                    break;

                case PropertyType.Maisonette:
                    var baths = random.NextInt16(0, 2);
                    property.Age = random.NextInt16(20, 50);
                    property.Bathrooms = baths == 0 ? 1 : baths;
                    property.Bedrooms = random.NextInt16(2, 3);
                    property.DistanceToStation = random.NextInt16(50, 2000);
                    property.SquareMts = roomSize * property.Bedrooms + random.NextInt16(100, 300);
                    property.TotalSquareMts = property.SquareMts + random.Next(10, 50);
                    property.HasParking = random.NextDouble() < .4;
                    break;
            }

            if (property.Bathrooms > property.Bedrooms)
            {
                property.Bathrooms = property.Bedrooms;
            }

            property.HasBackyard = property.TotalSquareMts - property.SquareMts > 20;
        }

        public static string PrintValues(this Property property)
        {
            var builder = new StringBuilder();
            property.GetType().GetProperties().ToList().ForEach(p => builder.Append(string.Format("{0} | ", p.GetValue(property))));
            return builder.Remove(builder.Length - 2, 1).ToString();
        }

        public static short NextInt16(this Random next, short min, short max)
        {
            return Convert.ToInt16(random.Next(min * 10, max * 10) / 10);
        }

        public static string PrintAttributes(this Type type)
        {
            var builder = new StringBuilder();
            type.GetProperties().ToList().ForEach(p => builder.Append(string.Format("{0} | ", p.Name)));
            return builder.Remove(builder.Length - 2, 1).ToString();
        }

        public static string ToSingletonQuery(this Property property)
        {
            string query = "SELECT '{0}' AS [Property Type], '{1}' AS [Operation Type], {2} AS [Age], {3} AS [Bathrooms], {4} AS [Bedrooms],";
            query += "{5} AS [Has Parking], {6} AS [Has Backyard], {7} AS [Distance To Station], '{8}' AS [Zone Reputation],";
            query += "{9} AS [Square Mts], {10} AS [With Furniture]";
            return string.Format(query, property.PropertyType, property.OperationType, property.Age, property.Bathrooms, property.Bedrooms,
                property.HasParking ? 1 : 0, property.HasBackyard ? 1 : 0, property.DistanceToStation, property.ZoneReputation,
                property.SquareMts, property.WithFurniture ? 1 : 0);
        }
    }
}
