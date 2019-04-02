using System;
using System.IO;
using System.Linq;
using FastFood.Data;
using FastFood.Models.Enums;
using Newtonsoft.Json;

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
			throw new NotImplementedException();
		}
	}
}