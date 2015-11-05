using System;
using System.Collections.Generic;
using System.Configuration;
using FileHelpers;

namespace Demo.Data
{
    class Program
    {
        static void Main(string[] args)
        {
            var properties = new List<Property>();
            using (var context = new DemoDbContext(ConfigurationManager.ConnectionStrings["Demo"].ConnectionString))
            {
                //context.Database.ExecuteSqlCommand("DELETE FROM [Properties]");

                int total = args.Length > 0 ? Convert.ToInt32(args[0]) : 100000;

                for (int i = 0; i < total; i++)
                {
                    var property = new Property();
                    property.Randomize();

                    property.Price = Convert.ToInt32(property.OperationType == OperationType.Rent ?
                        property.CalculateRentPrice() : property.CalculateSalePrice());

                    properties.Add(property);

                    if (i % 100 == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("{0}/{1}", i, total);
                    }
                }

                context.Properties.AddRange(properties);

                Console.WriteLine("Saving into database...");
                //context.SaveChanges();
                Console.WriteLine("Data saved!");
            }

            var engine = new FileHelperEngine<Property>();
            engine.WriteFile(string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}.csv", DateTime.Now), properties);

            Console.ResetColor();
            Console.WriteLine("Press any key to exit...");
            Console.Read();
        }
    }
}
