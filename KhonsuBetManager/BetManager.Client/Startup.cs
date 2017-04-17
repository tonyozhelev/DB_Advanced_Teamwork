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
            
            while (true)
            {
                try
                {
                    string command = Console.ReadLine();
                    string output = Executor.Execute(command);
                    Console.WriteLine(output);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
