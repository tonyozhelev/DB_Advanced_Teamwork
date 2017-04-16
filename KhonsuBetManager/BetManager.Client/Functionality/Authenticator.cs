namespace BetManager.Client.Functionality
{
    using Data;
    using Models;
    using System;
    using System.Linq;

    class Authenticator
    {
        private static User currentUser;

        public static bool IsAuthenticated()
        {
            return currentUser != null;
        }

        public static bool IsAdmin()
        {
            return currentUser.IsAdmin == 1;
        }

        public static bool IsOwner()
        {
            var isOwner = 0;
            using (var context = new BetManagerContext())
            {
                isOwner = context.Admins.FirstOrDefault(a => a.UserId == currentUser.Id).Owner;
            }
            return isOwner == 1;
        }

        public static void Logout()
        {
            if (!IsAuthenticated())
            {
                throw new InvalidOperationException("You should login first!");
            }

            currentUser = null;
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

            currentUser = user;
        }

        public static User GetCurrentUser()
        {
            return currentUser;
        }
    }
}
