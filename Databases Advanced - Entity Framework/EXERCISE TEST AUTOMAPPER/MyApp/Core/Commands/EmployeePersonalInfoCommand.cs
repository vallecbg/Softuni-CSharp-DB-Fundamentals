using AutoMapper;
using MyApp.Core.Commands.Contracts;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Core.Commands
{
    public class EmployeePersonalInfoCommand : ICommand
    {
        private readonly MyAppContext context;
        private readonly Mapper mapper;

        public EmployeePersonalInfoCommand(MyAppContext context, Mapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public string Execute(string[] inputArgs)
        {
            var employeeId = int.Parse(inputArgs[0]);

            var employee = this.context.Employees.Find(employeeId);

            return $"ID: {employeeId} - {employee.FirstName} {employee.LastName} -  ${employee.Salary:f2}" + Environment.NewLine +
                    $"Birthday: {String.Format("{0:dd-MM-yyyy}", employee.Birthday)}" + Environment.NewLine +
                    $"Address: {employee.Address}";
        }
    }
}
