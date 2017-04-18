namespace BetManager.Client.Functionality
{
    using Data;
    using Models;
    using System;
    using System.Linq;

    class Authenticator
    {
        private static int currentUserId;

        public static bool IsAuthenticated()
        {
            return currentUserId != 0;
        }

        public static bool IsAdmin()
        {
            if (!IsAuthenticated())
            {
                throw new InvalidOperationException("Invalid operation!");
            }
            return GetCurrentUser().IsAdmin == 1;
        }

        public static bool IsOwner()
        {
            if (!IsAuthenticated())
            {
                throw new InvalidOperationException("Invalid operation!");
            }
            var isOwner = 0;
            using (var context = new BetManagerContext())
            {
                if (context.Admins.Where(a => a.UserId == currentUserId).FirstOrDefault() != null)
                {
                    isOwner = context.Admins.Where(a => a.UserId == currentUserId).FirstOrDefault().Owner;
                }                
            }
            return isOwner == 1;
        }

        public static void Logout()
        {
            if (!IsAuthenticated())
            {
                throw new InvalidOperationException("You should login first!");
            }

            currentUserId = 0;
        }

        public static void Login(User user)
        {
            if (IsAuthenticated())
            {
                throw new InvalidOperationException("You should logout first!");
            }

            if (user == null)
            {
                throw new InvalidOperationException("User to log in is invalid!");
            }

            currentUserId = user.Id;
        }

        public static User GetCurrentUser()
        {
            var currentUser = new User();
            using (var context = new BetManagerContext())
            {
                currentUser = context.Users.Where(u => u.Id == currentUserId).First();
            }

            return currentUser;
        }
    }
}
