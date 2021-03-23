using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace FuelEfficiency
{
    class Program
    {
        static void Main(string[] args)
        {
            var vehicles = ProcessVehicles("data/2020_FE_Guide.csv");

            var query1 = vehicles.OrderByDescending(v => v.Combined)
                                .ThenBy(v => v.Name)
                                .Take(20);

            var query2 = vehicles.OrderBy(v => v.Combined)
                                 .Take(20);

            var query3 = vehicles
                            .Where(v => 
                                v.ReleaseDate > new DateTimeOffset(
                                    new DateTime(2019, 06, 30)))
                            .OrderByDescending(v => v.Combined)
                            .ThenBy(v => v.Name)
                            .Take(10);

            var query4 = vehicles.Where(c => c.Transmission.StartsWith("Manual"))
                                 .OrderByDescending(v => v.Combined)
                                 .ThenBy(v => v.Name)
                                 .Take(10);

            var query5 = vehicles.Where(c => c.Cylinders <= 6 && 
                                        c.Transmission.StartsWith("Auto"))
                                 .OrderBy(c => c.Combined)
                                 .ThenBy(c => c.Name)
                                 .Take(5);
            //---
            foreach (var vehicle in query5)
            {
                Console.WriteLine($"{vehicle.Manufacturer} {vehicle.Name} : {vehicle.Combined}");
            }
        }

        public static List<Vehicle> ProcessVehicles(string path)
        {
            var query =
                    File.ReadAllLines(path)
                    .Skip(1)
                    .Where(l => l.Length > 1)
                    .ToVehicle();

            return query.ToList();
        }
    }

    public static class VehicleExtensions
    {
        public static IEnumerable<Vehicle> ToVehicle(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var column = line.Split(";");

                yield return new Vehicle()
                {
                    ModelYear = int.Parse(column[0]),
                    Manufacturer = column[1],
                    Name = column[2],
                    Cylinders = int.Parse(column[3]),
                    Transmission = column[4],
                    City = double.Parse(column[5]),
                    Highway = double.Parse(column[6]),
                    Combined = double.Parse(column[7]),
                    ReleaseDate = DateTimeOffset
                                    .ParseExact(
                                        column[8],
                                        "dd/MM/yyyy",
                                        CultureInfo.InvariantCulture)
                };
            }
        }
    }

    public class Vehicle
    {
        public int ModelYear { get; set; }
        public string Manufacturer { get; set; }
        public string Name { get; set; }
        public int Cylinders { get; set; }
        public string Transmission { get; set; }
        public double City { get; set; }
        public double Highway { get; set; }
        public double Combined { get; set; }
        public DateTimeOffset ReleaseDate { get; set; }
    }
}
