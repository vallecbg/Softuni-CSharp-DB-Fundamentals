using Microsoft.Extensions.DependencyInjection;
using MyApp.Core.Contracts;
using System;
using System.Linq;

namespace MyApp.Core
{
    public class Engine : IEngine
    {
        private readonly IServiceProvider provider;

        public Engine(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public void Run()
        {
            string input;

            while ((input = Console.ReadLine()) != "Exit")
            {
                string[] inputArgs = input
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToArray();

                var commandInterpreter = this.provider.GetService<ICommandIntepreter>();
                var result = commandInterpreter.Read(inputArgs);

                Console.WriteLine(result);
            }
        }
    }
}
