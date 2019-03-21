namespace MyApp.Core
{
    using Microsoft.Extensions.DependencyInjection;
    using MyApp.Core.Commands.Contracts;
    using MyApp.Core.Contracts;
    using System;
    using System.Linq;
    using System.Reflection;

    public class CommandIntepreter : ICommandIntepreter
    {
        private const string Suffix = "Command";
        private readonly IServiceProvider serviceProvider;

        public CommandIntepreter(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }


        public string Read(string[] inputArgs)
        {
            var commandName = inputArgs[0] + Suffix;
            var commandParams = inputArgs.Skip(1).ToArray();

            var type = Assembly.GetCallingAssembly()
                .GetTypes()
                .FirstOrDefault(x => x.Name == commandName);

            if (type == null)
            {
                throw new ArgumentNullException("Invalid command!");
            }

            var constructor = type.GetConstructors()
                .FirstOrDefault();

            var constructorParams = constructor
                .GetParameters()
                .Select(x => x.ParameterType)
                .ToArray();

            var services = constructorParams
                .Select(this.serviceProvider.GetService)
                .ToArray();

            /* OR 
            var command = (ICommand)Activator.CreateInstance(type, services);
            */
            var command = (ICommand)constructor
                .Invoke(services);
            

            string result = command.Execute(commandParams);

            return result;
        }
    }
}
