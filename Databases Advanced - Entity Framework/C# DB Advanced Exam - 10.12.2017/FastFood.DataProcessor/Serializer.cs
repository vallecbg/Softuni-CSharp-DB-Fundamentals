using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using AutoMapper.QueryableExtensions;
using FastFood.Data;
using FastFood.DataProcessor.Dto.Export;
using FastFood.Models.Enums;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace FastFood.DataProcessor
{
	public class Serializer
	{
		public static string ExportOrdersByEmployee(FastFoodDbContext context, string employeeName, string orderType)
        {
            var orders = context
                .Orders
                .Where(x => x.Employee.Name == employeeName &&
                            x.Type.ToString() == orderType)
                .Select(x => new
                {
                    x.Customer,
                    Items = x.OrderItems
                        .Select(oi => new
                        {
                            oi.Item.Name,
                            oi.Item.Price,
                            oi.Quantity
                        }),
                    TotalPrice = x.OrderItems
                        .Sum(oi => oi.Quantity * oi.Item.Price)
                })
                .OrderByDescending(x => x.TotalPrice)
                .ThenByDescending(x => x.Items.Count())
                .ToList();

            var result = new
            {
                Name = employeeName,
                Orders = orders,
                TotalMade = orders.Sum(o => o.TotalPrice)
            };

            var json = JsonConvert.SerializeObject(result, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });

            return json;
        }

		public static string ExportCategoryStatistics(FastFoodDbContext context, string categoriesString)
        {
            var categoriesNames = categoriesString.Split(",");

            var stats = context
                .Categories
                .Where(x => categoriesNames.Any(c => c.Equals(x.Name, StringComparison.OrdinalIgnoreCase)))
                .Select(x => new
                {
                    Name = x.Name,
                    TopItem = x.Items
                        .OrderByDescending(i => i.OrderItems
                            .Sum(oi => oi.Quantity * oi.Item.Price))
                        .FirstOrDefault()
                })
                .Select(x => new ExportCategoryStatisticsDto()
                {
                    Name = x.Name,
                    MostPopularItem = new MostPopularItemDto()
                    {
                        Name = x.TopItem.Name,
                        TimesSold = x.TopItem.OrderItems
                            .Select(oi => oi.Quantity)
                            .Sum(),
                        TotalMade = x.TopItem.OrderItems
                            .Sum(oi => oi.Quantity * oi.Item.Price)
                    }
                })
                .OrderByDescending(x => x.MostPopularItem.TotalMade)
                .ThenByDescending(x => x.MostPopularItem.TimesSold)
                .ToArray();

            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(ExportCategoryStatisticsDto[]), new XmlRootAttribute("Categories"));
            serializer.Serialize(new StringWriter(sb), stats, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));

            var result = sb.ToString();
            return result;
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