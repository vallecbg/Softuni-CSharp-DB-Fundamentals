using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.Dtos.Export;
using CarDealer.Models;
using Microsoft.SqlServer.Server;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<CarDealerProfile>();
            });

            //var inputXml = File.ReadAllText("../../../Datasets/sales.xml");

            using (CarDealerContext context = new CarDealerContext())
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();
                var result = GetSalesWithAppliedDiscount(context);
                Console.WriteLine(result);
            }
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XDocument doc = XDocument.Parse(inputXml);

            var suppliersXml = doc.Root
                .Elements()
                .ToList();

            var suppliers = new List<Supplier>();

            suppliersXml.ForEach(x =>
            {
                var currentSupplier = new Supplier();
                currentSupplier.Name = x.Element("name").Value;
                currentSupplier.IsImporter = Convert.ToBoolean(x.Element("isImporter")?.Value);
                

                suppliers.Add(currentSupplier);
            });

            context.Suppliers.AddRange(suppliers);
            int affectedRows = context.SaveChanges();

            return $"Successfully imported {affectedRows}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XDocument doc = XDocument.Parse(inputXml);

            var partsXml = doc.Root
                .Elements()
                .ToList();

            var parts = new List<Part>();

            partsXml.ForEach(x =>
            {
                var currPart = new Part();
                currPart.Name = x.Element("name").Value;
                currPart.Price = decimal.Parse(x.Element("price").Value);
                currPart.Quantity = int.Parse(x.Element("quantity").Value);
                if (context.Suppliers
                    .Any(y => y.Id == int.Parse(x.Element("supplierId").Value)))
                {
                    currPart.SupplierId = int.Parse(x.Element("supplierId").Value);
                    parts.Add(currPart);
                }
            });

            context.Parts.AddRange(parts);
            int affectedRows = context.SaveChanges();

            return $"Successfully imported {affectedRows}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XDocument doc = XDocument.Parse(inputXml);

            var carsXml = doc.Root
                .Elements()
                .ToList();

            var cars = new List<Car>();

            var allPartsIds = context.Parts
                .Select(x => x.Id)
                .ToList();

            foreach (var x in carsXml)
            {
                var currCar = new Car();

                currCar.Make = x.Element("make").Value;
                currCar.Model = x.Element("model").Value;
                currCar.TravelledDistance = long.Parse(x.Element("TraveledDistance").Value);

                var partIds = new HashSet<int>();

                foreach (var part in x.Element("parts").Elements())
                {
                    partIds.Add(int.Parse(part.Attribute("id").Value));
                }

                foreach (var partId in partIds)
                {
                    if (!allPartsIds.Contains(partId))
                    {
                        continue;
                    }

                    PartCar currPart = new PartCar()
                    {
                        Car = currCar,
                        PartId = partId
                    };

                    currCar.PartCars.Add(currPart);
                }

                cars.Add(currCar);
            }

            context.Cars.AddRange(cars);
            int affectedRows = context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XDocument doc = XDocument.Parse(inputXml);

            var customersXml = doc.Root
                .Elements()
                .ToList();

            var customers = new List<Customer>();

            customersXml.ForEach(x =>
            {
                var currCustomer = new Customer();

                currCustomer.Name = x.Element("name").Value;
                currCustomer.BirthDate = DateTime.Parse(x.Element("birthDate").Value);
                currCustomer.IsYoungDriver = Convert.ToBoolean(x.Element("isYoungDriver").Value);

                customers.Add(currCustomer);
            });

            context.Customers.AddRange(customers);
            int affectedRows = context.SaveChanges();

            return $"Successfully imported {affectedRows}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XDocument doc = XDocument.Parse(inputXml);

            var salesXml = doc.Root
                .Elements()
                .ToList();

            var sales = new List<Sale>();

            salesXml.ForEach(x =>
            {
                var currSale = new Sale();

                if (context.Cars.Any(y => y.Id == int.Parse(x.Element("carId").Value)))
                {
                    currSale.CarId = int.Parse(x.Element("carId").Value);
                    currSale.CustomerId = int.Parse(x.Element("customerId").Value);
                    currSale.Discount = decimal.Parse(x.Element("discount").Value);

                    sales.Add(currSale);
                }
            });

            context.Sales.AddRange(sales);
            int affectedRows = context.SaveChanges();

            return $"Successfully imported {affectedRows}";
        }


        public static string GetCarsWithDistance(CarDealerContext context)
        {
            //Get all cars with distance more than 2,000,000. Order them by make, then by model alphabetically. Take top 10 records.

            var cars = context
                .Cars
                .Where(x => x.TravelledDistance > 2_000_00)
                .OrderBy(x => x.Make)
                .ThenBy(x => x.Model)
                .ProjectTo<GetCarsWithDistanceDto>()
                .Take(10)
                .ToArray();

            var xml = SerializeObject(cars, "cars");
            return xml;
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context
                .Cars
                .Where(x => x.Make == "BMW")
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .ProjectTo<GetCarsFromMakeBmwDto>()
                .ToArray();

            var xml = SerializeObject(cars, "cars");
            return xml;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context
                .Suppliers
                .Where(x => x.IsImporter == false)
                .ProjectTo<GetLocalSuppliersDto>()
                .ToArray();

            var xml = SerializeObject(suppliers, "suppliers");
            return xml;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            //Get all cars along with their list of parts. For the car get only make, model and travelled distance and for the parts get only name and price and sort all parts by price (descending). Sort all cars by travelled distance (descending) then by model (ascending). Select top 5 records.

            var cars = context
                .Cars
                .ProjectTo<GetCarsWithTheirListOfPartsDto>()
                .OrderByDescending(x => x.TravelledDistance)
                .ThenBy(x => x.Model)
                .Take(5)
                .ToArray();

            var xml = SerializeObject(cars, "cars");
            return xml;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            //Get all customers that have bought at least 1 car and get their names, bought cars count and total spent money on cars. Order the result list by total spent money descending.

            var customers = context
                .Customers
                .Where(x => x.Sales.Count >= 1)
                .ProjectTo<GetTotalSalesByCustomerDto>()
                .OrderByDescending(x => x.SpentMoney)
                .ToArray();

            var xml = SerializeObject(customers, "customers");
            return xml;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            //Get all sales with information about the car, customer and price of the sale with and without discount.
            var sales = context
                .Sales
                .ProjectTo<GetSalesWithAppliedDiscountDto>()
                .ToArray();

            var xml = SerializeObject(sales, "sales");
            return xml;

        }

        public static string SerializeObject<T>(T values, string rootName, bool omitXmlDeclaration = false,
            bool indentXml = true)
        {
            string xml = string.Empty;

            var serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootName));

            var settings = new XmlWriterSettings()
            {
                Indent = indentXml,
                OmitXmlDeclaration = omitXmlDeclaration
            };

            XmlSerializerNamespaces @namespace = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, values, @namespace);
                xml = stream.ToString();
            }

            return xml;
        }
    }
}