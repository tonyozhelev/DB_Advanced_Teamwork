namespace BetManager.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accountings",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        Transaction = c.String(nullable: false),
                        UserId = c.Int(nullable: false),
                        Ammount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Notes = c.String(),
                        DateOfTransaction = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Login = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsAdmin = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Bets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Coef = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Ammount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Matches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Team1 = c.String(nullable: false),
                        Team2 = c.String(nullable: false),
                        League = c.String(),
                        Coef1 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CoefX = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Coef2 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        Result = c.Int(nullable: false),
                        Score = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Admins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.MatchBets",
                c => new
                    {
                        Match_Id = c.Int(nullable: false),
                        Bet_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Match_Id, t.Bet_Id })
                .ForeignKey("dbo.Matches", t => t.Match_Id, cascadeDelete: true)
                .ForeignKey("dbo.Bets", t => t.Bet_Id, cascadeDelete: true)
                .Index(t => t.Match_Id)
                .Index(t => t.Bet_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Admins", "UserId", "dbo.Users");
            DropForeignKey("dbo.Accountings", "UserId", "dbo.Users");
            DropForeignKey("dbo.Bets", "UserId", "dbo.Users");
            DropForeignKey("dbo.MatchBets", "Bet_Id", "dbo.Bets");
            DropForeignKey("dbo.MatchBets", "Match_Id", "dbo.Matches");
            DropIndex("dbo.MatchBets", new[] { "Bet_Id" });
            DropIndex("dbo.MatchBets", new[] { "Match_Id" });
            DropIndex("dbo.Admins", new[] { "UserId" });
            DropIndex("dbo.Bets", new[] { "UserId" });
            DropIndex("dbo.Accountings", new[] { "UserId" });
            DropTable("dbo.MatchBets");
            DropTable("dbo.Admins");
            DropTable("dbo.Matches");
            DropTable("dbo.Bets");
            DropTable("dbo.Users");
            DropTable("dbo.Accountings");
        }
    }
}
