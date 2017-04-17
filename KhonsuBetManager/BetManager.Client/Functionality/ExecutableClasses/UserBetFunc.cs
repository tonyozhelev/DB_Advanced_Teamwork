﻿using BetManager.Data;
using BetManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BetManager.Client.Functionality.ExecutableClasses
{
    public class UserBetFunc
    {
        public static string DepositMoney(string[] input)
        {
            if (!Authenticator.IsAuthenticated())
            {
                throw new InvalidOperationException("You should login first!");
            }
            if (Authenticator.IsAdmin() && !Authenticator.IsOwner())
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
    }
}
