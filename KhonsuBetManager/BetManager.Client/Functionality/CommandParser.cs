using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetManager.Client.Functionality
{
    public class CommandParser
    {
        public void Execute(string command)
        {
            if (command == "Exit")
            {
                this.Exit();
            }
        }
        private string Exit()
        {
            Environment.Exit(0);

            return string.Empty;
        }
    }
}
