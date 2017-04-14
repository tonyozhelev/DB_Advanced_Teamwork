namespace BetManager.Data.Migrations
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BetManager.Data.BetManagerContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "BetManager.Data.BetManagerContext";
        }

        protected override void Seed(BetManager.Data.BetManagerContext context)
        {
            context.Users.AddOrUpdate(u => u.Login, new User {
                Login = "admin",
                Password = "admin",
                Balance = 0m,
                Email = "admin@admin.com",
                IsAdmin = 1
            });

            context.SaveChanges();

            context.Admins.AddOrUpdate(a => a.UserId, new Admin {
                UserId = context.Users.Where(u => u.Login == "admin").First().Id
            });

            context.SaveChanges();

        }
    }
}
