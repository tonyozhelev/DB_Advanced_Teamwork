
namespace BetManager.Client.Functionality.ExecutableClasses
{
    using BetManager.Data;
    using BetManager.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class UserBetFunc
    {
        public static string DepositMoney(string[] input)
        {
            if (!Authenticator.IsAuthenticated())
            {
                throw new InvalidOperationException("You should login first!");
            }
            if (Authenticator.IsAdmin())
            {
                throw new InvalidOperationException("You are an admin, you can't gamble!");
            }

            if (input.Length != 2)
            {
                throw new ArgumentException("Invalid operation. The deposit command should be in the following format:\ndeposit [cardnumber] [ammount to deposit]\nPlease note that we currently only accept VISA cards.");
            }

            Regex visaCheck = new Regex("^(?:4[0-9]{12})(?:[0-9]{3})?$");
            string cardNumber = input[0];
            if (!visaCheck.IsMatch(cardNumber))
            {
                throw new ArgumentException("Invalid card number. Please use a VISA card.");
            }

            decimal moneyToDeposit;
            if (!decimal.TryParse(input[1], out moneyToDeposit) || moneyToDeposit <= 0)
            {
                throw new ArgumentException("Money should be a positive number with a floating point.");
            }


            using (var context = new BetManagerContext())
            {
                var currentUser = Authenticator.GetCurrentUser();
                context.Users.Where(u => u.Id == currentUser.Id).First().Balance += moneyToDeposit;
                context.Accounting.Add(new Accounting
                {
                    Ammount = moneyToDeposit,
                    DateOfTransaction = DateTime.Now,
                    Notes = $"deposit from card {cardNumber}",
                    Transaction = "deposit",
                    UserId = currentUser.Id
                });

                context.SaveChanges();
            }

            return $"You have sucessfully depositet ${moneyToDeposit} to your account!";
        }

        public static string WithdrawMoney(string[] input)
        {
            if (!Authenticator.IsAuthenticated())
            {
                throw new InvalidOperationException("You should login first!");
            }
            if (Authenticator.IsAdmin() && !Authenticator.IsOwner())
            {
                throw new InvalidOperationException("You are an admin, you can't withdraw monet! If you wan't a salary speak to the owner!");
            }
            if (input.Length != 2)
            {
                throw new ArgumentException("Invalid operation. The withdraw command should be in the following format:\nwithdraw [cardnumber] [ammount to withdraw]\nPlease note that we currently only accept VISA cards.");
            }

            Regex visaCheck = new Regex("^(?:4[0-9]{12})(?:[0-9]{3})?$");
            string cardNumber = input[0];
            if (!visaCheck.IsMatch(cardNumber))
            {
                throw new ArgumentException("Invalid card number. Please use a VISA card.");
            }

            decimal moneyToWithdraw;
            if (!decimal.TryParse(input[1], out moneyToWithdraw) || moneyToWithdraw <= 0)
            {
                throw new ArgumentException("Money should be a positive number with a floating point.");
            }

            var reason = "";
            var currentUser = Authenticator.GetCurrentUser();

            if (!Authenticator.IsOwner())
            {
                if (currentUser.Balance < moneyToWithdraw)
                {
                    throw new ArgumentException($"You don't have that much money in your account. Maximum allowed money to withdraw - ${currentUser.Balance}");
                }
                using (var context = new BetManagerContext())
                {
                    context.Users.Where(u => u.Id == currentUser.Id).FirstOrDefault().Balance -= moneyToWithdraw;
                    context.Accounting.Add(new Accounting
                    {
                        Ammount = moneyToWithdraw,
                        DateOfTransaction = DateTime.Now,
                        Transaction = "withdraw",
                        UserId = currentUser.Id,
                        Notes = $"user withdrew {moneyToWithdraw} from account",
                    });

                    context.SaveChanges();
                }
            }
            else
            {
                Console.WriteLine("Please write a reason for the withdraw request;");
                reason = Console.ReadLine();
                using (var context = new BetManagerContext())
                {
                    context.Accounting.Add(new Accounting
                    {
                        Ammount = moneyToWithdraw,
                        DateOfTransaction = DateTime.Now,
                        Transaction = "withdraw",
                        UserId = currentUser.Id,
                        Notes = $"Owner withdrew {moneyToWithdraw}. Reason: {reason}",
                    });

                    context.SaveChanges();
                }
            }


            return $"You have sucessfully withdrawn {moneyToWithdraw} from your account! Money will be sent to VISA card N {cardNumber}";
        }

        public static string ViewMatches(string[] input)
        {
            if (!Authenticator.IsAuthenticated())
            {
                throw new InvalidOperationException("You should login first!");
            }
            if (input.Length != 1)
            {
                throw new InvalidOperationException("Invalid command! Viewmatches command should be in the following format\nviewmatches [future/past]");
            }

            var matches = new List<Models.Match>();

            using (var context = new BetManagerContext())
            {
                if (input[0] == "future")
                {
                    matches = context.Matches.Where(m => DateTime.Compare(m.Start,DateTime.Now) > 0).ToList();
                }
                else
                {
                    matches = context.Matches.Where(m => DateTime.Compare(m.Start, DateTime.Now) != 1).ToList();
                }
            }

            foreach (var m in matches)
            {
                if (m.League != "")
                {
                    Console.Write(m.League + ": ");
                }
                Console.WriteLine($"ID: {m.Id} -- {m.Team1} VS {m.Team2} - starts on {m.Start}; Coef - 1:{m.Coef1} X:{m.CoefX} 2:{m.Coef2}");
            }
            return string.Empty;
        }

        public static string PlaceBets(string[] input)
        {

            string output = "";

            if (!Authenticator.IsAuthenticated())
            {
                throw new InvalidOperationException("You should login first!");
            }
            if (Authenticator.IsAdmin())
            {
                throw new InvalidOperationException("You're an admin, you can't gamble!");
            }
            if (input.Length != 1 || (input[0] != "single" && input[0] != "multi"))
            {
                throw new ArgumentException("Invalid command. The placebet command should be in the following format:\nplacebet [single/multi]");
            }


            if (input[0] == "single")
            {
                output = CreateSingleBet();
            }
            if (input[0] == "multi")
            {
                output = CreateMultiBet();
            }
            return output;
        }

        private static string CreateSingleBet()
        {
            Console.WriteLine("Please enter the following command:\n[matchId] [desired bet (1/X/2)] [ammount to bet]");
            string[] bet = Console.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            decimal ammountToBet = 0m;
            int betMatchId = 0;
            var userToCheck = Authenticator.GetCurrentUser();
            if (bet.Length != 3)
            {
                throw new InvalidOperationException("Invalid bet operation. Please follow the instructions exactly when placing your bet!");
            }
            if (!decimal.TryParse(bet[2], out ammountToBet) || ammountToBet <= 0)
            {
                throw new ArgumentException("Invalid operation! The bet ammount should be a positive floating point number");
            }

            if (!int.TryParse(bet[0], out betMatchId) || betMatchId < 0)
            {
                throw new ArgumentException("Invalid operation! The match Id should be a positive number");
            }
            if (bet[1].ToLower() != "x" && bet[1] != "1" && bet[1] != "2")
            {
                throw new ArgumentException("Invalid operation! The desired bet should be 1,X or 2.");
            }

            var betToReturn = new Bet { };

            using (var context = new BetManagerContext())
            {
                if (context.Matches.Where(m => m.Id == betMatchId).FirstOrDefault() == null)
                {
                    throw new ArgumentException("Invalid operation! No match exist with the selected Id. For a list of matches please use the viewmatches command.");
                }
                if (DateTime.Compare(context.Matches.Where(m => m.Id == betMatchId).FirstOrDefault().Start,DateTime.Today) != 1)
                {
                    throw new ArgumentException("Invalid operation! This match has already started");
                }
                if (userToCheck.Balance < ammountToBet)
                {
                    throw new ArgumentException($"You don't have sufficient funds. Money in your account: ${Authenticator.GetCurrentUser().Balance}");
                }
                if (bet[1].ToLower() == "x")
                {
                    betToReturn.Coef = context.Matches.Where(m => m.Id == betMatchId).FirstOrDefault().CoefX;
                }
                else if (bet[1] == "1")
                {
                    betToReturn.Coef = context.Matches.Where(m => m.Id == betMatchId).FirstOrDefault().Coef1;
                }
                else
                {
                    betToReturn.Coef = context.Matches.Where(m => m.Id == betMatchId).FirstOrDefault().Coef2;
                }
                betToReturn.UserId = userToCheck.Id;
                betToReturn.Ammount = ammountToBet;
                context.Bets.Add(betToReturn);

                context.Users.Where(u => u.Id == userToCheck.Id).FirstOrDefault().Balance -= ammountToBet;

                context.SaveChanges();

                context.MatchesBets.Add(new MatchesBets
                {
                    BetId = betToReturn.Id,
                    MatchId = betMatchId,
                    BetPrediction = bet[1]
                });
                context.SaveChanges();
            }

            return "You have placed your bet. If you want to review use the listbets command";
        }


        private static string CreateMultiBet()
        {
            Console.WriteLine("Please enter the following command:\n[matchId] [desired bet (1/X/2)] or type done when you made all bets on this sheet.");
            string[] bet = Console.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int betMatchId = 0;
            decimal betsTotalCoef = 1m;
            List<string> betPredictions = new List<string>();
            List<int> matchIdOfBet = new List<int>();
            decimal curBetCoef = 0m;
            decimal ammountToBet = 0m;
            var betToAdd = new Bet { };
            while (bet[0].ToLower() != "done")
            {
                if (bet.Length != 2)
                {
                    throw new InvalidOperationException("Invalid operation! Please enter the commands exactly as described! Please start over with the placebet command.");
                }
                if (!int.TryParse(bet[0], out betMatchId) || betMatchId < 0)
                {
                    throw new ArgumentException("Invalid operation! The match Id should be a positive number. You now need to start over with your multi bet.");
                }
                if (matchIdOfBet.Contains(betMatchId))
                {
                    throw new ArgumentException("You can't use the same match twice in a multi bet. You now need to start over with your multi bet.");
                }
                if (bet[1].ToLower() != "x" && bet[1] != "1" && bet[1] != "2")
                {
                    throw new ArgumentException("Invalid operation! The desired bet should be 1,X or 2. You now need to start over with your multi bet.");
                }

                using (var context = new BetManagerContext())
                {
                    if (context.Matches.Where(m => m.Id == betMatchId).FirstOrDefault() == null)
                    {
                        throw new ArgumentException("Invalid operation! No match exist with the selected Id. For a list of matches please use the viewmatches command.");
                    }
                    if (DateTime.Compare(context.Matches.Where(m => m.Id == betMatchId).FirstOrDefault().Start, DateTime.Today) != 1)
                    {
                        throw new ArgumentException("Invalid operation! This match has already started! Please start over with the placebet command.");
                    }
                    if (bet[1].ToLower() == "x")
                    {
                        curBetCoef = context.Matches.Where(m => m.Id == betMatchId).FirstOrDefault().CoefX;
                    }
                    else if (bet[1] == "1")
                    {
                        curBetCoef = context.Matches.Where(m => m.Id == betMatchId).FirstOrDefault().Coef1;
                    }
                    else
                    {
                        curBetCoef = context.Matches.Where(m => m.Id == betMatchId).FirstOrDefault().Coef2;
                    }
                }
                betsTotalCoef *= curBetCoef;
                matchIdOfBet.Add(betMatchId);
                betPredictions.Add(bet[1]);

                Console.WriteLine("Please enter the following command:\n[matchId] [desired bet (1/X/2)] or type done when you made all bets on this sheet.");
                bet = Console.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }

            Console.WriteLine("Now Enter the ammount you want to bet!");
            if (!decimal.TryParse(Console.ReadLine(), out ammountToBet) || ammountToBet <= 0)
            {
                throw new ArgumentException("Invalid operation! The bet ammount should be a positive floating point number");
            }


            using (var context = new BetManagerContext())
            {
                if (Authenticator.GetCurrentUser().Balance < ammountToBet)
                {
                    throw new ArgumentException($"You don't have sufficient funds. Money in your account: ${Authenticator.GetCurrentUser().Balance}");
                }

                var user = Authenticator.GetCurrentUser();
                betToAdd.Ammount = ammountToBet;
                betToAdd.Coef = betsTotalCoef;
                betToAdd.UserId = user.Id;
                context.Users.Where(u => u.Id == user.Id).First().Balance -= ammountToBet;
                context.Bets.Add(betToAdd);

                context.SaveChanges();

                for (int i = 0; i < matchIdOfBet.Count; i++)
                {
                    context.MatchesBets.Add(new MatchesBets
                    {
                        BetId = betToAdd.Id,
                        MatchId = matchIdOfBet[i],
                        BetPrediction = betPredictions[i],
                    });
                }

                context.SaveChanges();
            }

            return "You have placed your bet. If you want to review use the listbets command";
        }

        public static string ListBets(string[] input)
        {
            if (Authenticator.IsAdmin())
            {
                throw new InvalidOperationException("You're an admin. You can't gamble!");
            }
            if (!Authenticator.IsAuthenticated())
            {
                throw new InvalidOperationException("Please login first!");
            }
            var user = Authenticator.GetCurrentUser();
            var userBets = new List<Bet>();
            var pred = new string[] { "1", "x", "2" };
            var result = new string[] { "Winning", "Losing", "Pending" };
            var resultInDB = new string[] { "Y", "N", null };
            using (var context = new BetManagerContext())
            {
                foreach (var bet in context.Bets.Where(b => b.UserId == user.Id))
                {
                    userBets.Add(bet);
                }
                if (userBets.Count == 0)
                {
                    throw new InvalidOperationException("You haven't placed any bets");
                }
                foreach (var bet in userBets)
                {
                    Console.WriteLine($"Bet N {bet.Id} contains the following matches:");
                    foreach (var match in bet.MatchesBets)
                    {
                        var coefs = new decimal[] { match.Match.Coef1, match.Match.CoefX, match.Match.Coef2 };
                        Console.WriteLine($"{match.Match.Team1} vs {match.Match.Team2} startin on {match.Match.Start}");
                        Console.WriteLine($"Prediction => {match.BetPrediction}; Coef => {coefs[Array.IndexOf(pred, match.BetPrediction)]} Result => {match.Result}");
                    }
                    Console.WriteLine($"Ammount wagered => ${bet.Ammount}; This bet is {result[Array.IndexOf(resultInDB, bet.Win)]}");
                    if (bet.Win == "Y")
                    {
                        Console.WriteLine($"Ammount won ${bet.Ammount*bet.Coef}");
                    }
                }
            }
            return "End of List";
        }

        public static string ViewUserInfo(string[] input)
        {
            if (!Authenticator.IsAdmin())
            {
                PrintInfoUser(input);
            }

            if (Authenticator.IsAdmin())
            {
                PrintInfoAdmin(input);
            }
            return "Please type in your next command";
        }

        private static void PrintInfoUser(string[] input)
        {
            if (!Authenticator.IsAuthenticated())
            {
                throw new InvalidOperationException("Please login first!");
            }
            if (input.Length != 0)
            {
                throw new InvalidOperationException("To view info just type viewuserinfo");
            }
            else
            {
                var userToView = Authenticator.GetCurrentUser();
                using (var context = new BetManagerContext())
                {
                    var totalMatches = 0;
                    foreach (var bet in userToView.Bets)
                    {
                        totalMatches += bet.MatchesBets.Count();
                    }
                    Console.WriteLine($"User Info For User: {userToView.Login}");
                    Console.WriteLine($"Ballance: ${userToView.Balance}");
                    Console.WriteLine($"Email: {userToView.Email}");
                    Console.WriteLine($"Bets Count: {userToView.Bets.Count} On Total Matches {totalMatches}");
                    Console.WriteLine($"Bets Won: {userToView.Bets.Where(b => b.Win == "Y").ToList().Count} Lost: {userToView.Bets.Where(b => b.Win == "N").ToList().Count} Pending: {userToView.Bets.Where(b => b.Win == "").ToList().Count}");
                }
            }
        }

        private static void PrintInfoAdmin(string[] input)
        {
            var xmlDocument = new XDocument();
            using (var context = new BetManagerContext())
            {
                xmlDocument.Add(new XElement("users"));
                foreach (var user in context.Users)
                {
                    var userXML = new XElement("user", new XElement("email", user.Email), new XElement("username",user.Login));
                    xmlDocument.Root.Add(userXML);
                    var listBetsXML = new XElement("bets");
                    userXML.Add(listBetsXML);
                    
                    foreach (var bet in user.Bets)
                    {
                        var betXML = new XElement("bet", $"id={bet.Id}", $"ammount={bet.Ammount}", $"coef={bet.Coef}", $"win={bet.Win}");
                        listBetsXML.Add(betXML);
                        var listMatchesXML = new XElement("matches");
                        betXML.Add(listMatchesXML);
                        foreach (var match in bet.MatchesBets)
                        {
                            var matchXML = new XElement("match",
                                $"{match.Match.Team1} vs {match.Match.Team2}; League: {match.Match.League}; Start: {match.Match.Start}; Score: {match.Match.Score}");
                            listMatchesXML.Add(matchXML);
                        }
                    }
                }
            }

            xmlDocument.Save("../../Users.xml");
            Console.WriteLine("All User Info Printed in Users.xml");
        }
    }
}
