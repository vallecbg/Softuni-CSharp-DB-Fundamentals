using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            using (var context = new SoftUniContext())
            {
                var result = RemoveTown(context);
                Console.WriteLine(result);
            }
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.Salary,
                    e.EmployeeId
                })
                .OrderBy(x => x.EmployeeId);

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary,
                    e.EmployeeId
                })
                .Where(x => x.Salary > 50_000)
                .OrderBy(x => x.FirstName);
            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} - {employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.Department.Name,
                    e.Salary,
                    DepartmentName = e.Department.Name
                })
                .OrderBy(x => x.Salary)
                .ThenByDescending(x => x.FirstName);

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} from {employee.DepartmentName} - ${employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            context.Employees
                .Single(e => e.LastName == "Nakov")
                .Address = address;

            context.SaveChanges();

            var sb = new StringBuilder();

            context.Employees
                .OrderByDescending(e => e.Address.AddressId)
                .Take(10)
                .Select(e => e.Address.AddressText)
                .ToList()
                .ForEach(at => sb.AppendLine(at));

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.EmployeesProjects
                .Any(ep => ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003))
                .Take(10)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    ManagerFirstName = e.Manager.FirstName,
                    ManagerLastName = e.Manager.LastName,
                    Projects = e.EmployeesProjects
                            .Select(ep => ep.Project)
                })
                .ToList();

            var sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");
                foreach (var p in e.Projects)
                {
                    sb.AppendLine($"--{p.Name} - {p.StartDate} - {(p.EndDate == null ? "not finished" : p.EndDate.ToString())}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            var sb = new StringBuilder();

            context.Addresses
                    .GroupBy(a => new
                    {
                        a.AddressId,
                        a.AddressText,
                        a.Town.Name
                    },
                        (key, group) => new
                        {
                            AddressText = key.AddressText,
                            Town = key.Name,
                            Count = group.Sum(a => a.Employees.Count)
                        })
                    .OrderByDescending(o => o.Count)
                    .ThenBy(o => o.Town)
                    .ThenBy(o => o.AddressText)
                    .Take(10)
                    .ToList()
                    .ForEach(o => sb.AppendLine($"{o.AddressText}, {o.Town} - {o.Count} employees"));

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            var employee = context.Employees
                    .Where(e => e.EmployeeId == 147)
                    .Select(e => new
                    {
                        e.FirstName,
                        e.LastName,
                        e.JobTitle,
                        Projects = e.EmployeesProjects
                            .Select(ep => ep.Project.Name)
                            .OrderBy(p => p)
                            .ToList()
                    })
                    .First();

            return ($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}{Environment.NewLine}{String.Join(Environment.NewLine, employee.Projects)}");
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var sb = new StringBuilder();
            context.Departments
                    .Include(d => d.Employees)
                    .Include(d => d.Manager)
                    .Where(d => d.Employees.Count > 5)
                    .OrderBy(d => d.Employees.Count)
                    .ThenBy(d => d.Name)
                    .ToList()
                    .ForEach(d => sb.AppendLine($"{d.Name} - {d.Manager.FirstName} {d.Manager.LastName}{Environment.NewLine}{String.Join(Environment.NewLine, d.Employees.OrderBy(e => e.FirstName).ThenBy(e => e.LastName).Select(e => $"{e.FirstName} {e.LastName} - {e.JobTitle}").ToList())}{Environment.NewLine}{new string('-', 10)}"));

            return sb.ToString().TrimEnd();
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            var sb = new StringBuilder();
            context.Projects.
                    OrderByDescending(p => p.StartDate).
                    Take(10).
                    Select(p => new { p.Name, p.Description, p.StartDate })
                    .OrderBy(p => p.Name)
                    .ToList()
                    .ForEach(p => sb.AppendLine($"{p.Name}{Environment.NewLine}{p.Description}{Environment.NewLine}{p.StartDate}"));

            return sb.ToString().TrimEnd();
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            context.Employees
                    .Where(e => new[] { "Engineering", "Tool Design", "Marketing", "Information Services" }
                        .Contains(e.Department.Name))
                    .ToList()
                    .ForEach(e => e.Salary *= 1.12m);

            context.SaveChanges();

            var sb = new StringBuilder();

            context.Employees
                .Where(e => new[] { "Engineering", "Tool Design", "Marketing", "Information Services" }
                    .Contains(e.Department.Name))
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList()
                .ForEach(e => sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:f2})"));

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var sb = new StringBuilder();

            context.Employees
                    .Where(e => e.FirstName.Substring(0, 2) == "Sa")
                    .Select(e => new { e.FirstName, e.LastName, e.JobTitle, e.Salary })
                    .OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)
                    .ToList()
                    .ForEach(e => sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})"));

            return sb.ToString().TrimEnd();
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            var project = context.Projects.First(p => p.ProjectId == 2);

            context.EmployeesProjects.ToList().ForEach(ep => context.EmployeesProjects.Remove(ep));
            context.Projects.Remove(project);

            context.SaveChanges();

            var sb = new StringBuilder();

            context.Projects.Take(10).Select(p => p.Name).ToList().ForEach(p => sb.AppendLine(p));

            return sb.ToString().TrimEnd();
        }
        public static string RemoveTown(SoftUniContext context)
        {
            var name = "Seattle";

            var town = context.Towns
                .FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (town == null)
            {
                return ($"There is not town with name: {name}");
            }

            context.Employees
                .Where(e => e.Address.Town.TownId == town.TownId)
                .ToList()
                .ForEach(e => e.Address = null);

            var addresses = context.Addresses
                .Where(a => a.TownId == town.TownId)
                .ToArray();

            var addressesCount = addresses.Length;

            context.Addresses.RemoveRange(addresses);
            context.Towns.Remove(town);
            context.SaveChanges();

            string addressPluralisation = addressesCount == 1
                ? "address"
                : "addresses";

            string countPluralisation = addressesCount == 1
                ? "was"
                : "were";

            return ($"{addressesCount} {addressPluralisation} in {name} {countPluralisation} deleted");
        }
    }
}
