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
                    output = this.RegisterNewUser(commandArgs);
                    break;
                default:
                    break;
            }

            return output;
        }

        private string RegisterNewUser(string[] input)
        {
            if (input.Length != 3)
            {
                throw new ArgumentException("Invalid input! Register command should be in the following format:\nregister [username] [password] [email]");
            }
            if (Authenticator.IsAuthenticated())
            {
                throw new InvalidOperationException("You should logout to create new user!");
            }

            string userName = input[0];
            string pwd = input[1];
            string email = input[2];

            User newUser = new User()
            {
                Login = userName,
                Password = pwd,
                Email = email,
                Balance = 0m,
                IsAdmin = 0,
            };

            this.ValidateUser(newUser);


            using (BetManagerContext context = new BetManagerContext())
            {
                if (context.Users.Any(u => u.Login == userName))
                {
                    throw new ArgumentException("Username already taken!");
                }

                if (context.Users.Any(u => u.Email == email))
                {
                    throw new ArgumentException("Email already taken!");
                }

                context.Users.Add(newUser);
                context.SaveChanges();
            }

            return $"User {userName} successfully registered!";
        }

        private string Exit()
        {
            Environment.Exit(0);

            return string.Empty;
        }


        private void ValidateUser(User user)
        {
            bool isValid = true;
            string errors = string.Empty;
            if (user == null)
            {
                errors = "User cannot be null!";

                // If user is null there is no way that we cant it's username or password
                // (they simply ain't set) so we return directly our result.
                throw new ArgumentException(errors);
            }

            Regex usernameRegex = new Regex(@"^[a-zA-Z]+[a-zA-Z0-9]{2,}$");
            if (!usernameRegex.IsMatch(user.Login))
            {
                isValid = false;
                errors += "Username not valid! Username should contain only letters and Numbers\n";
            }

            // For this regex check this article: http://stackoverflow.com/questions/1559751/regex-to-make-sure-that-the-string-contains-at-least-one-lower-case-char-upper.
            Regex passwordRegex = new Regex(@"^(?=[a-zA-Z0-9]*[A-Z])(?=[a-zA-Z0-9]*[a-z])(?=[a-zA-Z0-9]*[0-9])[a-zA-Z0-9]{6,}$");
            if (!passwordRegex.IsMatch(user.Password))
            {
                isValid = false;
                errors += "Password not valid! Password should contain at least one upper letter, one lower letter and one number! Password should be more than 6 characters long\n";
            }

            // Regex explanation:
            // ^ - string should start with
            // [a-zA-Z0-9]+[-|_|\.]? - any alphanumeric group (longer than 1 symbol) followed by "-" or "_" or "." exactly one time. Will match: "Dash1-"
            // ({upperPartHere})* - the upper match may happen zero or more times. Will match: "Dash1-Dot1-Hyphen1-"
            // [a-zA-Z0-9]+ - after upper match should follow at least one alphanumeric character. Will match: "Dash1-Dot1-Hyphen1".
            // The rest of the regex follows the same logic.
            Regex emailRegex = new Regex(@"^([a-zA-Z0-9]+[-|_|\.]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[-]?)*[a-zA-Z0-9]+\.([a-zA-Z0-9]+[-]?)*[a-zA-Z0-9]+$");
            if (!emailRegex.IsMatch(user.Email))
            {
                isValid = false;
                errors += "Email not valid!\n";
            }

            if (!isValid)
            {
                // Trim new lines left after string concatenation.
                throw new ArgumentException(errors.Trim());
            }
        }
    }
}
