
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

        internal static string ViewMatches(string[] input)
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
                    matches = context.Matches.Where(m => m.Result == 0).ToList();
                }
                else
                {
                    matches = context.Matches.Where(m => m.Result != 0).ToList();
                }
            }

            foreach (var m in matches)
            {
                if (m.League != "")
                {
                    Console.Write(m.League + ": ");
                }
                Console.WriteLine($"{m.Team1} VS {m.Team2} - starts on {m.Start}; Coef - 1:{m.Coef1} X:{m.CoefX} 2:{m.Coef2}");
            }
            return string.Empty;
        }
    }
}
