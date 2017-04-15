using BetManager.Data;
using BetManager.Models;
using System;
using System.Linq;


namespace BetManager.Client.Functionality
{
    public class UserFunc
    {
        public static string RegisterNewUser(string[] input)
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

            ValidateUser(newUser);


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


        public static string LoginUser(string[] input)
        {
            if (input.Length != 2)
            {
                throw new ArgumentException("Invalid input! Login command should be in the following format:\nlogin [username] [password]");
            }
            if (Authenticator.IsAuthenticated())
            {
                throw new InvalidOperationException("You are already logged in");
            }

            var userName = input[0];
            var pass = input[1];

            using (var context = new BetManagerContext())
            {
                var user = context.Users.Where(x => x.Login == userName && x.Password == pass).FirstOrDefault();

                if (user == null)
                {
                    throw new ArgumentException("Invalid Username/Password");
                }

                Authenticator.Login(user);
            }

            return $"User {userName} logged in successfully!";
        }


        public static string LogoutUser()
        {
            if (!Authenticator.IsAuthenticated())
            {
                throw new ArgumentException("You should login first!");
            }

            var username = Authenticator.GetCurrentUser().Login;
            Authenticator.Logout();

            return $"User {username} logged out!";
        }

        public static string ChangePass(string[] commandArgs)
        {
            if (commandArgs.Length != 2)
            {
                throw new InvalidOperationException("Invalid command! Change password command should be in the following format:\n changepass [old password] [new password]");
            }
            if (!Authenticator.IsAuthenticated())
            {
                throw new ArgumentException("You should login first!");
            }

            var oldPassword = commandArgs[0];
            var newPassword = commandArgs[1];

            var user = Authenticator.GetCurrentUser();

            if (oldPassword != user.Password)
            {
                throw new ArgumentException("Your old password is not correct!");
            }

            ValidateUser(user);

            user.Password = newPassword;
            using (var context = new BetManagerContext())
            {
                context.Users.Where(x => x.Id == user.Id).First().Password = newPassword;
                context.SaveChanges();
            }

            return $"User {user.Login} sucessfuly changed his password!";
        }

        private static void ValidateUser(User user)
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
