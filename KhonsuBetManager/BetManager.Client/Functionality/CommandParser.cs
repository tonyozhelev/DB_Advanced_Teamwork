using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetManager.Client.Functionality
{
    public class CommandParser
    {
        public string Execute(string command)
        {
            string output = "";
            if (command == "Exit")
            {
                output = this.Exit();
            }

            return output;
        }
        private string Exit()
        {
            Environment.Exit(0);

            return "You have exited the program!";
        }
    }
}
