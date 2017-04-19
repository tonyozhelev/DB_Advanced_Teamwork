
namespace BetManager.Client.Functionality
{
using BetManager.Client.Functionality.ExecutableClasses;
using BetManager.Data;
using BetManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
                    output = UserFunc.LogoutUser(commandArgs);
                    break;
                case "changepass":
                    output = UserFunc.ChangePass(commandArgs);
                    break;
                case "promote":
                    output = AdminFunc.PromoteUser(commandArgs);
                    break;
                case "demote":
                    output = AdminFunc.DemoteUser(commandArgs);
                    break;
                case "addmatch":
                    output = AdminFunc.AddMatch(commandArgs);
                    break;
                case "deposit":
                    output = UserBetFunc.DepositMoney(commandArgs);
                    break;
                case "withdraw":
                    output = UserBetFunc.WithdrawMoney(commandArgs);
                    break;
                case "viewmatches":
                    output = UserBetFunc.ViewMatches(commandArgs);
                    break;
                case "placebet":
                    output = UserBetFunc.PlaceBets(commandArgs);
                    break;
                case "updateresults":
                    output = AdminFunc.UpdateMatchResult(commandArgs);
                    break;
                case "listbets":
                    output = UserBetFunc.ListBets(commandArgs);
                    break;
                case "viewuserinfo":
                    output = UserBetFunc.ViewUserInfo(commandArgs);
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
