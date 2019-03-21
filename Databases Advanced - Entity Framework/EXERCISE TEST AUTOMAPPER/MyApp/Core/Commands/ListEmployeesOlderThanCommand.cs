using AutoMapper;
using MyApp.Core.Commands.Contracts;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Core.Commands
{
    public class ListEmployeesOlderThanCommand : ICommand
    {
        private readonly MyAppContext context;
        private readonly Mapper mapper;

        public ListEmployeesOlderThanCommand(MyAppContext context, Mapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public string Execute(string[] inputArgs)
        {
            var age = int.Parse(inputArgs[0]);
            var sb = new StringBuilder();
            foreach (var emp in context.Employees)
            {
                if (emp.Birthday != null && DateTime.Now.Year - emp.Birthday.Value.Year > age)
                {
                    sb.AppendLine($"{emp.FirstName} {emp.LastName} - ${emp.Salary:f2} - Manager: {((emp.Manager != null) ? emp.Manager.FirstName : "[no manager]")}");
                }
            }

            return sb.ToString().TrimEnd();
        }
    }
}
