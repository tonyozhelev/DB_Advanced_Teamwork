namespace BetManager.Client.Functionality.ExecutableClasses
{
    using Data;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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

        public static string AddMatch(string[] input)
        {
            if (!Authenticator.IsAdmin())
            {
                throw new InvalidOperationException("Invalid operation");
            }
            if (input.Length != 0)
            {
                throw new ArgumentException("Too many arguments. Type \"addmatch\" if you want to add a match!");
            }

            Console.WriteLine("Enter Team 1:");
            var team1 = Console.ReadLine();
            Console.WriteLine("Enter Team 2:");
            var team2 = Console.ReadLine();
            Console.WriteLine("Enter League");
            var league = Console.ReadLine();
            Console.WriteLine("Enter coef for 1:");
            var coef1str = Console.ReadLine();
            var coef1 = 0m;
            if (!decimal.TryParse(coef1str, out coef1))
            {
                throw new ArgumentException("Coef should be a number with a floating point. Please start over with the addmatch command");
            }
            Console.WriteLine("Enter coef for X:");
            var coef2str = Console.ReadLine();
            var coef2 = 0m;
            if (!decimal.TryParse(coef1str, out coef2))
            {
                throw new ArgumentException("Coef should be a number with a floating point. Please start over with the addmatch command");
            }
            Console.WriteLine("Enter coef for 2:");
            var coef3str = Console.ReadLine();
            var coef3 = 0m;
            if (!decimal.TryParse(coef1str, out coef3))
            {
                throw new ArgumentException("Coef should be a number with a floating point. Please start over with the addmatch command");
            }
            DateTime start;
            Console.WriteLine("Enter start date and time in the following format dd-mm-yy hh:mm using a 24 hour format:");
            if (!DateTime.TryParseExact(Console.ReadLine(), "d-M-y H:m", CultureInfo.InvariantCulture, DateTimeStyles.None, out start))
            {
                throw new ArgumentException("The date time should be in this exat format: dd-mm-yy hh:mm. Please start over with the addmatch command");
            }
            DateTime end = start.AddMinutes(90);
            
            var newMatchId = 0;
            using (var context = new BetManagerContext())
            {
                var newMatch = new Match
                {
                    Team1 = team1,
                    Team2 = team2,
                    Coef1 = coef1,
                    CoefX = coef2,
                    Coef2 = coef3,
                    League = league,
                    Start = start,
                    End = end
                };
                context.Matches.Add(newMatch);
                context.SaveChanges();
                newMatchId = newMatch.Id;
            }

            return $"Added match [{team1} vs {team2}] with Id: {newMatchId}";
        }
    }
}


