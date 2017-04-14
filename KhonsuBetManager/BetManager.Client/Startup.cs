namespace BetManager.Client
{
    using BetManager.Data;
    using Functionality;
    using System;

    class Startup
    {
        static void Main(string[] args)
        {
            var Executor = new CommandParser();
            var command = Console.ReadLine();

            Executor.Execute(command);

        }
    }
}
