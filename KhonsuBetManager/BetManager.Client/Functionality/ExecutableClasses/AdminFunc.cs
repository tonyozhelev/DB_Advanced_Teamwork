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
                var userToPromote = context.Users.FirstOrDefault(u => u.Login == username);

                if (userToPromote == null || userToPromote.Login != username)
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

        public static string DemoteUser(string[] input)
        {
            if (!Authenticator.IsOwner())
            {
                throw new InvalidOperationException("Invalid operation!");
            }

            if (input.Length > 1)
            {
                throw new ArgumentException("Invalid command! Demote command should be in the following format:\ndemote [username]");
            }

            var username = input[0];

            using (var context = new BetManagerContext())
            {
                var userToDemote = context.Users.Where(u => u.Login == username).FirstOrDefault();

                if (userToDemote == null || userToDemote.Login != username)
                {
                    throw new ArgumentException("Invalid username!");
                }
                if (userToDemote.IsAdmin == 0)
                {
                    throw new InvalidOperationException($"User {username} is not an Admin");
                }

                userToDemote.IsAdmin = 0;

                var adminToRemove = context.Admins.Where(a => a.UserId == userToDemote.Id).FirstOrDefault();
                context.Admins.Remove(adminToRemove);


                context.SaveChanges();
            }

            return $"User {username} has been removed from Admins!";
        }
    }
}
