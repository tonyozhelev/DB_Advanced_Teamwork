
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
                case "help":
                    output = PrintHelp();
                    break;
                case "createtemplate":
                    output = AdminFunc.CreateJsonTemplate(commandArgs);
                    break;
                case "addjson":
                    output = AdminFunc.AddFromJson(commandArgs);
                    break;
                default:
                    output = "Type HELP for a list of commands;";
                    break;
            }

            return output;
        }


        private string Exit()
        {
            Environment.Exit(0);

            return string.Empty;
        }

        public string PrintIntro()
        {
            return @"
____________$$$$$$$$$$$$$               
________$$$$$$$ _________$$$            
_____$$$$$$$$$ ___________ $$$$         ██╗    ██╗███████╗██╗      ██████╗ ██████╗ ███╗   ███╗███████╗    ████████╗ ██████╗                                                                
____$$_$$$____$$ ________ $___$$        ██║    ██║██╔════╝██║     ██╔════╝██╔═══██╗████╗ ████║██╔════╝    ╚══██╔══╝██╔═══██╗                                                               
___$___$________$ ______ $______$$      ██║ █╗ ██║█████╗  ██║     ██║     ██║   ██║██╔████╔██║█████╗         ██║   ██║   ██║                                                               
__$___$ _________ $$$$$$$$_______$$     ██║███╗██║██╔══╝  ██║     ██║     ██║   ██║██║╚██╔╝██║██╔══╝         ██║   ██║   ██║                                                               
_$___$$ ________ $$$$$$$$$________$$    ╚███╔███╔╝███████╗███████╗╚██████╗╚██████╔╝██║ ╚═╝ ██║███████╗       ██║   ╚██████╔╝                                                               
$$___$ _________ $$$$$$$$$$________$   	 ╚══╝╚══╝ ╚══════╝╚══════╝ ╚═════╝ ╚═════╝ ╚═╝     ╚═╝╚══════╝       ╚═╝    ╚═════╝                                                                
$___$$ _______$$$$$$$$$$$$$$_______$   	                                                                                                                                                   
$__$$$$$ __ $$$___$$$$$$$__ $$$$___$$  	██╗  ██╗██╗  ██╗ ██████╗ ███╗   ██╗███████╗██╗   ██╗    ██████╗ ███████╗████████╗    ███╗   ███╗ █████╗ ███╗   ██╗ █████╗  ██████╗ ███████╗██████╗ 
$$$$$$$$$$$ _______ $$$________$$$$$   	██║ ██╔╝██║  ██║██╔═══██╗████╗  ██║██╔════╝██║   ██║    ██╔══██╗██╔════╝╚══██╔══╝    ████╗ ████║██╔══██╗████╗  ██║██╔══██╗██╔════╝ ██╔════╝██╔══██╗
$_$$$$$$$ __________ $__________$$$$   	█████╔╝ ███████║██║   ██║██╔██╗ ██║███████╗██║   ██║    ██████╔╝█████╗     ██║       ██╔████╔██║███████║██╔██╗ ██║███████║██║  ███╗█████╗  ██████╔╝
$_$$$$$$$ __________ $__________$$$$   	██╔═██╗ ██╔══██║██║   ██║██║╚██╗██║╚════██║██║   ██║    ██╔══██╗██╔══╝     ██║       ██║╚██╔╝██║██╔══██║██║╚██╗██║██╔══██║██║   ██║██╔══╝  ██╔══██╗
_$_$$$$$$ _________ $$__________ $$$    ██║  ██╗██║  ██║╚██████╔╝██║ ╚████║███████║╚██████╔╝    ██████╔╝███████╗   ██║       ██║ ╚═╝ ██║██║  ██║██║ ╚████║██║  ██║╚██████╔╝███████╗██║  ██║  
__$_$$__$$ ________ $_________$$_$$     ╚═╝  ╚═╝╚═╝  ╚═╝ ╚═════╝ ╚═╝  ╚═══╝╚══════╝ ╚═════╝     ╚═════╝ ╚══════╝   ╚═╝       ╚═╝     ╚═╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝ ╚═════╝ ╚══════╝╚═╝  ╚═╝  
___$$_____$$$ ___ $$$$$____$$$___$        
____$$ _____ $$$$$$$$$$$$$$$___$$       TYPE [HELP] FOR A LIST OF COMMANDS
_____$$$ _____ $$$$$$$$$$____$$           
_______$$$$ __ $$$$$$$$$__$$$             
__________$$$$$ _____ $$$$                
______________$$$$$$$";
        }

        public string PrintHelp()
        {
            var helpPrint = @"MENU:
1. REGISTER [USERNAME] [PASSWORD] [E-MAIL] - Register a new user;
2. LOGIN [USERNAME] [PASSWORD] - User login;
3. LOGOUT - User logout;
4. CHANGEPASS [OLD PASSWORD] [NEW PASSWORD] - Change password;
5. EXIT - Exit the program;
";

            if (Authenticator.IsAuthenticated())
            {
                helpPrint += @"
USER FUNCTIONS:
1. DEPOSIT [CARD NUMBER] [MONEY AMMOUNT] - Deposit money from your credit card;
2. WITHDRAW [CARD NUMBER] [MONEY AMMOUNT] - Withdraw money to your credit card;
3. VIEWMATCHES [PAST/FUTURE] - View a list of past or present matches;
4. PLACEBET [SINGLE/MULTI] - Place a single bet or multiple bet;
5. LISTBETS - List of all your bets;
6. VIEWUSERINFO - View your info;
";
            }

            if (Authenticator.IsAuthenticated() && Authenticator.IsAdmin())
            {
                helpPrint += @"
ADMIN FUNCTIONS:
1. ADDMATCH - Add a new match in the system. Please follow the instruction strictly;
2. UPDATERESULTS - Update results of matches. Please follow the instruction strictly;
3. VIEWUSERINFO - View info about all users. Info will be saved into an Users.xml file for your convenience;
4. CREATETEMPLATE [FILENAME WITHOUT EXTENTION] - Creates a json file template to fill matches;
5. ADDJSON [FILENAME WITHOUT EXTENTION] - Adds matches from json file;
";
            }

            if (Authenticator.IsAuthenticated() && Authenticator.IsOwner())
            {
                helpPrint += @"
OWNER FUNCTIONS:
1. PROMOTE [USERNAME] - Promote the selected user to admin;
2. DEMOTE [USERNAME] - Remove admin status from user;
";
            }
            return helpPrint;
        }


    }
}
