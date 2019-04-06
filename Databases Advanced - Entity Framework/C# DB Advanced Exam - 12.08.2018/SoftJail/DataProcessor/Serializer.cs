using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SoftJail.DataProcessor.ExportDto;
using Formatting = Newtonsoft.Json.Formatting;

namespace SoftJail.DataProcessor
{

    using Data;
    using System;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisoners = context.Prisoners
                .Where(x => ids.Contains(x.Id))
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.FullName,
                    CellNumber = x.Cell.CellNumber,
                    Officers = x.PrisonerOfficers.Select(c => new
                    {
                        OfficerName = c.Officer.FullName,
                        Department = c.Officer.Department.Name
                    })
                        .OrderBy(c => c.OfficerName),
                    TotalOfficerSalary = x.PrisonerOfficers.Sum(c => c.Officer.Salary)
                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id);

            var json = JsonConvert.SerializeObject(prisoners, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });

            return json;
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var prisonersNamesArr = prisonersNames.Split(",");

            var prisoners = context
                .Prisoners
                .Where(x => prisonersNamesArr.Contains(x.FullName))
                .Select(x => new ExportPrisonersInboxDto
                {
                    Id = x.Id,
                    Name = x.FullName,
                    IncarcerationDate = x.IncarcerationDate.ToString("yyyy-MM-dd"),
                    EncryptedMessages = x.Mails.Select(c => new EncryptedMessagesDto()
                    {
                        Description = ReverseMessage(c.Description)
                    }).ToArray()
                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToArray();

            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(ExportPrisonersInboxDto[]), new XmlRootAttribute("Prisoners"));
            serializer.Serialize(new StringWriter(sb), prisoners, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));

            var result = sb.ToString();
            return result;
        }

        private static string ReverseMessage(string message)
        {
            return new string(message.Reverse().ToArray());
        }
    }
}