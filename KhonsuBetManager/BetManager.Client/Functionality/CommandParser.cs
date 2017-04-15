using BetManager.Data;
using BetManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BetManager.Client.Functionality
{
    public class CommandParser
    {
        public string Execute(string command)
        {
            string output = string.Empty;
            string[] commandArgs = command.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            string commandName = commandArgs.Length != 0 ? commandArgs[0].ToLower() : string.Empty;

            commandArgs = commandArgs.Skip(1).ToArray();

            switch (commandName)
            {
                case "exit":
                    output = this.Exit();
                    break;
                case "register":
                    output = UserFunc.RegisterNewUser(commandArgs);
                    break;
                case "login":
                    output = UserFunc.LoginUser(commandArgs);
                    break;
                case "logout":
                    output = UserFunc.LogoutUser();
                    break;
                case "changepass":
                    output = UserFunc.ChangePass(commandArgs);
                    break;
                default:
                    break;
            }

            return output;
        }


        private string Exit()
        {
            Environment.Exit(0);

            return string.Empty;
        }
    }
}
