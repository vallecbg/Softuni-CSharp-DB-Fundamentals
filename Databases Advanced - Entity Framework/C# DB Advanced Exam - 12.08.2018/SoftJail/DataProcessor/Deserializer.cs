using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SoftJail.Data.Models;
using SoftJail.Data.Models.Enums;
using SoftJail.DataProcessor.ImportDto;

namespace SoftJail.DataProcessor
{

    using Data;
    using System;

    public class Deserializer
    {
        public const string InvalidMessage = "Invalid Data";

        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var resultMessages = new List<string>();

            var departments = JsonConvert.DeserializeObject<ImportDepartmentsCellsDto[]>(jsonString);
            var resultDepartments = new List<Department>();

            foreach (var departmentDto in departments)
            {
                if (departmentDto.Name.Length < 3 || departmentDto.Name.Length > 25)
                {
                    resultMessages.Add(InvalidMessage);
                    continue;
                }

                var hasInvalidData = false;
                var departmentCells = new List<Cell>();
                foreach (var departmentCell in departmentDto.Cells)
                {
                    if (departmentCell.CellNumber < 1 ||
                        departmentCell.CellNumber > 1000)
                    {
                        resultMessages.Add(InvalidMessage);
                        hasInvalidData = true;
                        break;
                    }

                    Cell cell = new Cell()
                    {
                        CellNumber = departmentCell.CellNumber,
                        HasWindow = departmentCell.HasWindow
                    };

                    departmentCells.Add(cell);
                }

                if (hasInvalidData)
                {
                    continue;
                }

                var currDepartment = new Department()
                {
                    Name = departmentDto.Name,
                    Cells = departmentCells
                };

                resultDepartments.Add(currDepartment);
                resultMessages.Add($"Imported {currDepartment.Name} with {currDepartment.Cells.Count} cells");
            }

            context.Departments.AddRange(resultDepartments);
            context.SaveChanges();

            return string.Join(Environment.NewLine, resultMessages);
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var resultMessages = new List<string>();

            var prisoners = JsonConvert.DeserializeObject<ImportPrisonersMailsDto[]>(jsonString, settings);
            var resultPrisoners = new List<Prisoner>();

            foreach (var prisonerDto in prisoners)
            {
                if (prisonerDto.FullName == null || prisonerDto.Nickname == null ||
                    prisonerDto.FullName.Length < 3 || prisonerDto.FullName.Length > 20 ||
                    !Regex.Match(prisonerDto.Nickname, @"The [A-Z]{1}[a-z]*").Success ||
                    prisonerDto.Age < 18 || prisonerDto.Age > 65)
                {
                    resultMessages.Add(InvalidMessage);
                    continue;
                }

                if (!IsValid(prisonerDto))
                {
                    resultMessages.Add(InvalidMessage);
                    continue;
                }

                DateTime incarceration;
                if (!DateTime.TryParseExact(prisonerDto.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out incarceration))
                {
                    resultMessages.Add(InvalidMessage);
                    continue;
                }

                DateTime release;
                if (prisonerDto.ReleaseDate != null)
                {
                    if (!DateTime.TryParseExact(prisonerDto.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out release))
                    {
                        resultMessages.Add(InvalidMessage);
                        continue;
                    }
                }

                var prisonerMails = new List<Mail>();
                var hasInvalidData = false;

                foreach (var prisonerDtoMail in prisonerDto.Mails)
                {
                    if (!Regex.Match(prisonerDtoMail.Address, @"[A-Za-z0-9\s]*(str\.)").Success)
                    {
                        resultMessages.Add(InvalidMessage);
                        hasInvalidData = true;
                        break;
                    }
                }

                if (hasInvalidData)
                {
                    continue;
                }

                Prisoner prisoner = new Prisoner()
                {
                    FullName = prisonerDto.FullName,
                    Nickname = prisonerDto.Nickname,
                    Age = prisonerDto.Age,
                    IncarcerationDate = DateTime.ParseExact(prisonerDto.IncarcerationDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture),
                    ReleaseDate = prisonerDto.ReleaseDate == null ? (DateTime?)null : DateTime.ParseExact(prisonerDto.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                    Bail = prisonerDto.Bail,
                    CellId = prisonerDto.CellId
                };

                context.Prisoners.Add(prisoner);
                context.SaveChanges();


                var invalid = false;
                foreach (var dtoMail in prisonerDto.Mails)
                {
                    if (!IsValid(dtoMail))
                    {
                        resultMessages.Add(InvalidMessage);
                        invalid = true;
                        break;
                    }
                    Mail mail = new Mail()
                    {
                        Description = dtoMail.Description,
                        Sender = dtoMail.Sender,
                        Address = dtoMail.Address,
                        PrisonerId = prisoner.Id
                    };
                    prisonerMails.Add(mail);
                }

                if (invalid)
                {
                    continue;
                }
                context.Mails.AddRange(prisonerMails);
                context.SaveChanges();

                resultPrisoners.Add(prisoner);
                resultMessages.Add($"Imported {prisoner.FullName} {prisoner.Age} years old");
            }

            return string.Join(Environment.NewLine, resultMessages);
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            var resultMessages = new List<string>();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportOfficersPrisonersDto[]), new XmlRootAttribute("Officers"));

            var officersDto = (ImportOfficersPrisonersDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var officers = new List<Officer>();

            foreach (var officerDto in officersDto)
            {
                if (!IsValid(officerDto))
                {
                    resultMessages.Add(InvalidMessage);
                    continue;
                }

                if (officerDto.Name.Length < 3 || officerDto.Name.Length > 30 ||
                    officerDto.Money < 0)
                {
                    resultMessages.Add(InvalidMessage);
                    continue;
                }

                var positionParse = Enum.TryParse<Position>(officerDto.Position, out Position positionResult);
                var weaponParse = Enum.TryParse<Weapon>(officerDto.Weapon, out Weapon weaponResult);
                if (positionParse == false || weaponParse == false)
                {
                    resultMessages.Add(InvalidMessage);
                    continue;
                }

                Officer officer = new Officer()
                {
                    FullName = officerDto.Name,
                    Salary = officerDto.Money,
                    Position = positionResult,
                    Weapon = weaponResult,
                    DepartmentId = officerDto.DepartmentId
                };

                var officerPrisoners = new List<OfficerPrisoner>();

                foreach (var prisonerDto in officerDto.Prisoners)
                {
                    if (!IsValid(prisonerDto))
                    {
                        resultMessages.Add(InvalidMessage);
                        continue;
                    }
                    OfficerPrisoner prisoner = new OfficerPrisoner()
                    {
                        PrisonerId = prisonerDto.Id,
                        Officer = officer
                    };

                    officerPrisoners.Add(prisoner);
                }

                officer.OfficerPrisoners = officerPrisoners;

                if (!IsValid(officer))
                {
                    resultMessages.Add(InvalidMessage);
                    continue;
                }

                officers.Add(officer);
                resultMessages.Add($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");
            }

            context.Officers.AddRange(officers);
            context.SaveChanges();

            return string.Join(Environment.NewLine, resultMessages);
        }

        public static bool IsValid(object obj)
        {
            ValidationContext validationContext = new ValidationContext(obj);
            List<ValidationResult> validationResults = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext, validationResults, true);
        }
    }
}