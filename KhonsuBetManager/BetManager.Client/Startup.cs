namespace BetManager.Client
{
    using BetManager.Data;
    using Functionality;
    using System;

    class Startup
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(190, 50);
            var Executor = new CommandParser();
            Console.WriteLine(Executor.PrintIntro()); 
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
