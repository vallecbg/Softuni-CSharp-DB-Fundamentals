using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using AutoMapper;
using FastFood.Data;
using FastFood.DataProcessor.Dto.Import;
using FastFood.Models;
using FastFood.Models.Enums;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal;
using Newtonsoft.Json;

namespace FastFood.DataProcessor
{
    public static class Deserializer
    {
        private const string FailureMessage = "Invalid data format.";
        private const string SuccessMessage = "Record {0} successfully imported.";

        public static string ImportEmployees(FastFoodDbContext context, string jsonString)
        {
            var resultMessages = new List<string>();

            //var employees = JsonConvert.DeserializeAnonymousType(jsonString, new[] { new { Name = String.Empty, Age = 0, Position = String.Empty } });
            var employees = JsonConvert.DeserializeObject<ImportEmployeeDto[]>(jsonString);

            foreach (var employee in employees)
            {
                if (employee.Name.Length < 3 || employee.Name.Length > 30 ||
                    employee.Age < 15 || employee.Age > 80 || employee.Position == null || employee.Position.Length < 3 || employee.Position.Length > 30)
                {
                    resultMessages.Add(FailureMessage);
                    continue;
                }

                if (!context.Positions.Any(x => x.Name == employee.Position))
                {
                    context.Positions.Add(new Position() { Name = employee.Position });
                    context.SaveChanges();
                }

                var position = context.Positions.First(x => x.Name == employee.Position);
                context.Employees.Add(new Employee() { Name = employee.Name, Age = employee.Age, Position = position });
                context.SaveChanges();
                resultMessages.Add(string.Format(SuccessMessage, employee.Name));
            }

            return string.Join(Environment.NewLine, resultMessages);
        }

        public static string ImportItems(FastFoodDbContext context, string jsonString)
        {
            var resultMessages = new List<string>();

            var items = JsonConvert.DeserializeObject<ImportItemsDto[]>(jsonString);
            foreach (var item in items)
            {
                if (item.Name.Length < 3 || item.Name.Length > 30 || item.Name == null || item.Category == null || item.Category.Length < 3 || item.Category.Length > 30 || item.Price < 0.01m)
                {
                    resultMessages.Add(FailureMessage);
                    continue;
                }

                if (context.Items.Any(x => x.Name == item.Name))
                {
                    resultMessages.Add(FailureMessage);
                    continue;
                }

                if (!context.Categories.Any(x => x.Name == item.Category))
                {
                    context.Categories.Add(new Category() { Name = item.Category });
                    context.SaveChanges();
                }

                var category = context.Categories.First(x => x.Name == item.Category);
                context.Items.Add(new Item()
                {
                    Name = item.Name,
                    Price = item.Price,
                    Category = category
                });
                context.SaveChanges();

                resultMessages.Add(string.Format(SuccessMessage, item.Name));
            }

            return string.Join(Environment.NewLine, resultMessages);
        }

        public static string ImportOrders(FastFoodDbContext context, string xmlString)
        {
            var messagesOutput = new List<string>();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportOrdersDto[]), new XmlRootAttribute("Orders"));

            var ordersDto = (ImportOrdersDto[])xmlSerializer.Deserialize(new StringReader(xmlString));
            var orders = new List<Order>();
            foreach (var orderDto in ordersDto)
            {
                if (!context.Employees.Any(x => x.Name == orderDto.Employee))
                {
                    continue;
                }

                var currentEmployee = context.Employees.First(x => x.Name == orderDto.Employee);
                var currentDateTime = DateTime.ParseExact(orderDto.DateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                var currentType = Enum.Parse<OrderType>(orderDto.Type);

                var orderItems = new List<OrderItem>();

                foreach (var item in orderDto.Items)
                {
                    if (!context.Items.Any(x => x.Name == item.Name))
                    {
                        continue;
                    }

                    var currentItem = context.Items.First(x => x.Name == item.Name);

                    OrderItem orderItem = new OrderItem()
                    {
                        ItemId = currentItem.Id,
                        Quantity = item.Quantity
                    };

                    orderItems.Add(orderItem);
                }

                Order order = new Order()
                {
                    Customer = orderDto.Customer,
                    Employee = currentEmployee,
                    DateTime = currentDateTime,
                    Type = currentType,
                    OrderItems = orderItems
                };

                orders.Add(order);
                messagesOutput.Add($"Order for {order.Customer} on {order.DateTime:dd/MM/yyyy HH:mm} added");
            }

            context.Orders.AddRange(orders);
            context.SaveChanges();

            return string.Join(Environment.NewLine, messagesOutput);

            //XDocument doc = XDocument.Parse(xmlString);

            //var ordersXml = doc.Root
            //    .Elements()
            //    .ToList();

            //var customers = new List<Order>();

            //var messagesOutput = new List<string>();

            //foreach (var orderXml in ordersXml)
            //{
            //    var order = new Order();

            //    order.Customer = orderXml.Element("Customer").Value;

            //    if (!context.Employees.Any(x => x.Name == orderXml.Element("Employee").Value))
            //    {
            //        continue;
            //    }

            //    order.Employee = context.Employees.First(x => x.Name == orderXml.Element("Employee").Value);

            //    //order.DateTime =
            //    //    DateTime.ParseExact(orderXml.Element("DateTime").Value, "dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture);
            //    order.DateTime =
            //        DateTime.Parse(orderXml.Element("DateTime").Value);

            //    order.Type = Enum.Parse<OrderType>(orderXml.Element("Type").Value);

            //    var items = new List<Item>();

            //    var itemsXml = orderXml.Elements("Items");


            //    foreach (var item in itemsXml)
            //    {
            //        if (!context.Items.Any(x => x.Name == item.Element("Name").Value))
            //        {
            //            continue;
            //        }

            //        var currentItem = context.Items.First(x => x.Name == item.Element("Name").Value);

            //        OrderItem orderItem = new OrderItem()
            //        {
            //            Order = order,
            //            Item = currentItem
            //        };
            //    }
            //}
        }
    }
}