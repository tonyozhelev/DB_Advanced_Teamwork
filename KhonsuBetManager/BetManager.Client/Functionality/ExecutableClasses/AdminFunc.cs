namespace BetManager.Client.Functionality.ExecutableClasses
{
    using Data;
    using Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    public class AdminFunc
    {
        public static string PromoteUser(string[] input)
        {
            if (!Authenticator.IsOwner())
            {
                throw new InvalidOperationException("Invalid operation! Type HELP for list of commands!");
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
                throw new InvalidOperationException("Invalid operation! Type HELP for list of commands!");
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
                throw new InvalidOperationException("Invalid operation! Type HELP for list of commands!");
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
            if (!decimal.TryParse(coef1str, out coef1) || coef1 <= 0m)
            {
                throw new ArgumentException("Coef should be a number with a floating point. Please start over with the addmatch command");
            }
            Console.WriteLine("Enter coef for X:");
            var coef2str = Console.ReadLine();
            var coef2 = 0m;
            if (!decimal.TryParse(coef2str, out coef2) || coef2 <= 0m)
            {
                throw new ArgumentException("Coef should be a positive number with a floating point. Please start over with the addmatch command");
            }
            Console.WriteLine("Enter coef for 2:");
            var coef3str = Console.ReadLine();
            var coef3 = 0m;
            if (!decimal.TryParse(coef3str, out coef3) || coef3 <= 0m)
            {
                throw new ArgumentException("Coef should be a positive number with a floating point. Please start over with the addmatch command");
            }
            DateTime start;
            Console.WriteLine("Enter start date and time in the following format dd-mm-yy hh:mm using a 24 hour format:");
            if (!DateTime.TryParseExact(Console.ReadLine(), "d-M-y H:m", CultureInfo.InvariantCulture, DateTimeStyles.None, out start))
            {
                throw new ArgumentException("The date time should be in this exat format: dd-mm-yy hh:mm using a 24 hour format. Please start over with the addmatch command");
            }
            DateTime end = start.AddMinutes(90);

            var newMatchId = 0;
            var newMatch = new Models.Match
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
            using (var context = new BetManagerContext())
            {
                context.Matches.Add(newMatch);
                context.SaveChanges();
                newMatchId = newMatch.Id;
            }

            return $"Added match [{team1} vs {team2}] with Id: {newMatchId}";
        }

        public static string UpdateMatchResult(string[] input)
        {
            if (!Authenticator.IsAdmin())
            {
                throw new InvalidOperationException("Invalid operation! Type HELP for list of commands!");
            }
            if (input.Length != 0)
            {
                throw new InvalidOperationException("Invalid operation! Please type only updateresults and follow the instructions.");
            }
            Console.WriteLine("Please enter [matchid] [score]. The score should be in the following format [Team1]:[Team2]. Type done when finished updating.");
            var matchToUpd = Console.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            while (matchToUpd[0] != "done")
            {
                int matchId = 0;
                int score1 = -1;
                int score2 = -1;
                if (matchToUpd.Length != 2)
                {
                    throw new ArgumentException("Invalid operation. Make sure that your input does not contain any excess spacing.");
                }
                if (!matchToUpd[1].Contains(":"))
                {
                    throw new ArgumentException("Invalid operation! Score should be in the following format [Team1]:[Team2].");
                }
                if (matchToUpd.Length != 2)
                {
                    throw new InvalidOperationException("Invalid operation! Please follow the instructions exactly!");
                }
                if (!int.TryParse(matchToUpd[0], out matchId))
                {
                    throw new ArgumentException("Invalit operation! The match ID should be a whole number.");
                }
                if (!int.TryParse(matchToUpd[1].Split(':')[0], out score1) || !int.TryParse(matchToUpd[1].Split(':')[1], out score2))
                {
                    throw new ArgumentException("Invalit operation! The match result should be two whole numbers with [:] in between.");
                }



                using (var context = new BetManagerContext())
                {
                    var matchFromContext = context.Matches.Where(m => m.Id == matchId).FirstOrDefault();
                    if (matchFromContext == null)
                    {
                        throw new ArgumentException($"No match with ID {matchId} exists! Please enter new command.");
                    }
                    if (matchFromContext.Result != 0)
                    {
                        throw new ArgumentException($"This match already has a result! If you want to change the result of a match please contact one of the owners so that the DB and Users are updated accordingly. Please be extra carefull when updating match scores!!!");
                    }
                    Console.WriteLine("Are you sure this is the correct result. Please note that there might be serious reprocutions from entering a wrong score. Please type Y to continue or anything else to break.");


                    var agree = Console.ReadLine();
                    if (agree == "Y")
                    {
                        matchFromContext.Score = matchToUpd[1];
                        if (score1 > score2)
                        {
                            matchFromContext.Result = 1;
                        }
                        else if (score1 < score2)
                        {
                            matchFromContext.Result = 2;
                        }
                        else
                        {
                            matchFromContext.Result = 3;
                        }

                        context.SaveChanges();

                        UpdateUserBets(context, matchFromContext);

                        
                    }
                    else
                    {
                        Console.WriteLine("You didn't add the match to the database.");
                    }
                }
                
                
                Console.WriteLine("Please enter [matchid] [score]. The score should be in the following format [Team1]:[Team2]. Type done when finished updating.");
                matchToUpd = Console.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            }
            return "You have sucessfully updated the designated matches!";
        }

        private static void UpdateUserBets(BetManagerContext context, Models.Match matchFromContext)
        {
            var resToPass = new string[] { "1", "2", "x" };
            foreach (var bet in context.Bets.Where(x => x.MatchesBets.Any(m =>m.MatchId == matchFromContext.Id)))
            {
                bet.MatchesBets.Where(x => x.MatchId == matchFromContext.Id).First().Result = resToPass[matchFromContext.Result - 1];
                if (bet.MatchesBets.All(x => x.BetPrediction == x.Result))
                {
                    bet.Win = "Y";
                    context.Users.Where(u => u.Id == bet.UserId).First().Balance += bet.Coef * bet.Ammount;
                }
                else if (bet.MatchesBets.Any(x => x.Result != x.BetPrediction) && bet.MatchesBets.All(x => x.Result != null))
                {
                    bet.Win = "N";
                }
            }

            context.SaveChanges();
        }

        public static string CreateJsonTemplate(string[] input)
        {
            Regex checkFileName = new Regex("[a-zA-Z0-9]{3,}");
            if (!Authenticator.IsAdmin())
            {
                throw new InvalidOperationException("Invalid operation! Type HELP for list of commands!");
            }
            if (input.Length != 1)
            {
                throw new InvalidOperationException("Invalid operation! The createtemplate command should be in the following format\ncreatetemplate [filename]");
            }
            if (!checkFileName.IsMatch(input[0]))
            {
                throw new ArgumentException("Your file name should contain at least 3 symbols. Only Alphanumerics are allowed.");
            }
            string jsonTemplate = "[\n{\nTeam1:\"\"\nTeam2:\"\"\nLeague:\"\"\ncoef1:\"\"\ncoef2:\"\"\ncoef3:\"\"\nstart:\"\"\n},\n{\nTeam1:\"\"\nTeam2:\"\"\nLeague:\"\"\ncoef1:\"\"\ncoef2:\"\"\ncoef3:\"\"\nstart:\"\"\n}\n]";
            System.IO.File.WriteAllText($"../../../{input[0]}.json", jsonTemplate);
            return $"You've created template {input[0]}.json. Please fill the file and add the matches!";
        }


        internal static string AddFromJson(string[] input)
        {
            int count = 0;
            if (!Authenticator.IsAdmin())
            {
                throw new InvalidOperationException("Invalid operation! Type HELP for list of commands!");
            }
            if (input.Length != 1)
            {
                throw new ArgumentException("Invalid command. The addfromjson command should be in the following format:\naddfromjson [filename without extention]");
            }

            var arrayOfMatchText = JsonConvert.DeserializeObject<MatchText[]>(System.IO.File.ReadAllText($"../../../{input[0]}.json"));
            var matchList = new List<Models.Match>();
            if (arrayOfMatchText.Length == 0)
            {
                throw new ArgumentException("There are no matches entered in the file");
            }
            foreach (var match in arrayOfMatchText)
            {
                count++;
                DateTime start;
                if (!DateTime.TryParseExact(match.Start, "d-M-y H:m", CultureInfo.InvariantCulture, DateTimeStyles.None, out start))
                {
                    throw new ArgumentException($"The date time should be in this exat format: dd-mm-yy hh:mm using a 24 hour format. Please fix the value for entity {count} in the file");
                }
                if (matchList.Where(m => m.Team1 == match.Team1 && m.Team2 == match.Team2 && DateTime.Compare(m.Start,start)==0).Count() != 0)
                {
                    throw new ArgumentException($"Match {count} is a double. Please verify your file");
                }
                matchList.Add(AddMatch(match.Team1, match.Team2, match.League, match.Coef1, match.Coef2, match.Coef3, match.Start, count));
            }

            using (var context = new BetManagerContext())
            {
                context.Matches.AddRange(matchList);
                context.SaveChanges();
            }
            
            return $"You have added {count} matches to the database!";
        }


        public static Models.Match AddMatch(string t1, string t2, string lg, string cf1, string cfx, string cf2, string startDate, int count)
        {
            var team1 = t1;
            var team2 = t2;
            var league = lg;
            var coef1str = cf1;
            var coef1 = 0m;
            if (!decimal.TryParse(coef1str, out coef1) || coef1 <= 0m)
            {
                throw new ArgumentException($"Coef should be a number with a floating point. Please fix the value for entity {count} in the file");
            }
            var coef2str = cfx;
            var coef2 = 0m;
            if (!decimal.TryParse(coef2str, out coef2) || coef2 <= 0m)
            {
                throw new ArgumentException($"Coef should be a number with a floating point. Please fix the value for entity {count} in the file");
            }
            var coef3str = cf2;
            var coef3 = 0m;
            if (!decimal.TryParse(coef3str, out coef3) || coef3 <= 0m)
            {
                throw new ArgumentException($"Coef should be a number with a floating point. Please fix the value for entity {count} in the file");
            }
            DateTime start;
            if (!DateTime.TryParseExact(startDate, "d-M-y H:m", CultureInfo.InvariantCulture, DateTimeStyles.None, out start))
            {
                throw new ArgumentException($"The date time should be in this exat format: dd-mm-yy hh:mm using a 24 hour format. Please fix the value for entity {count} in the file");
            }
            DateTime end = start.AddMinutes(90);

            var newMatch = new Models.Match
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

            return newMatch;
        }
    }

    internal class MatchText
    {
        public string Team1 { get; set; }
        public string Team2 { get; set; }
        public string League { get; set; }
        public string Coef1 { get; set; }
        public string Coef2 { get; set; }
        public string Coef3 { get; set; }
        public string Start { get; set; }

    }
}


