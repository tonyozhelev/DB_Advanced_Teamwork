namespace BetManager.Data
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class BetManagerContext : DbContext
    {
        public BetManagerContext()
            : base("name=BetManagerContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BetManagerContext, Migrations.Configuration>());
        }

        public virtual DbSet<Accounting> Accounting { get; set; }
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Bet> Bets { get; set; }
        public virtual DbSet<Match> Matches { get; set; }
    }
}