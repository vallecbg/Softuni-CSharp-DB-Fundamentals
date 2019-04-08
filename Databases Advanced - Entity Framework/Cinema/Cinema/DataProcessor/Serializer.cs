using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Cinema.DataProcessor.ExportDto;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace Cinema.DataProcessor
{
    using System;

    using Data;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            var result = context
                .Movies
                //todo check double >= int
                //todo check second statement
                .Where(x => x.Rating >= rating && x.Projections.Any())
                .Select(x => new
                {
                    MovieName = x.Title,
                    Rating = $"{x.Rating:f2}",
                    TotalIncomes = $"{x.Projections.SelectMany(c => c.Tickets).Sum(z => z.Price):f2}",
                    Customers = x.Projections.SelectMany(c => c.Tickets).Select(z => new
                    {
                        FirstName = z.Customer.FirstName,
                        LastName = z.Customer.LastName,
                        Balance = $"{z.Customer.Balance:f2}"
                    })
                        .OrderByDescending(c => c.Balance)
                        .ThenBy(c => c.FirstName)
                        .ThenBy(c => c.LastName)
                        //.Distinct()
                })
                .OrderByDescending(x => decimal.Parse(x.Rating))
                .ThenByDescending(x => decimal.Parse(x.TotalIncomes))
                .Take(10)
                .ToArray();

            var json = JsonConvert.SerializeObject(result, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });

            return json;
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            var customers = context
                .Customers
                .Where(x => x.Age >= age)
                .Select(x => new ExportTopCustomersDto()
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SpentMoney = $"{x.Tickets.Sum(c => c.Price):f2}",
                    SpentTime = TimeSpan.FromTicks(x.Tickets.Select(c => c.Projection).Sum(y => y.Movie.Duration.Ticks)).ToString("hh\\:mm\\:ss")
                })
                .OrderByDescending(x => decimal.Parse(x.SpentMoney))
                .Take(10)
                .ToArray();
            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(ExportTopCustomersDto[]), new XmlRootAttribute("Customers"));
            serializer.Serialize(new StringWriter(sb), customers, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));

            var result = sb.ToString();
            return result;
        }
    }
}