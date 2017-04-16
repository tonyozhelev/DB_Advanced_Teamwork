namespace BetManager.Client.Functionality.ExecutableClasses
{
    using Data;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class AdminFunc
    {
        public static string PromoteUser(string[] input)
        {
            if (!Authenticator.IsOwner())
            {
                throw new InvalidOperationException("No such command!");
            }

            if (input.Length > 1)
            {
                throw new ArgumentException("Invalid command! Promote command should be in the following format:\npromote [username]");
            }

            var username = input[0];

            using (var context = new BetManagerContext())
            {
                var userToPromote = context.Users.Where(u => u.Login == username).FirstOrDefault();

                if (userToPromote.Login != username)
                {
                    throw new ArgumentException("Invalid username!");
                }
                if (userToPromote.IsAdmin == 1)
                {
                    throw new InvalidOperationException($"User {username} is already an Admin");
                }

                userToPromote.IsAdmin = 1;

                context.Admins.Add(new Admin
                {
                    UserId = context.Users.Where(u => u.Login == username).First().Id,
                    Owner = 0
                });

                context.SaveChanges();
            }
            return $"User {username} has been updated to Admin!";
        }
    }
}
