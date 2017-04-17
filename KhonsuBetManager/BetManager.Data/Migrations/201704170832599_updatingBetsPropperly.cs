namespace BetManager.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatingBetsPropperly : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Matches", "Bet_Id", "dbo.Bets");
            DropIndex("dbo.Matches", new[] { "Bet_Id" });
            DropColumn("dbo.Matches", "Bet_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Matches", "Bet_Id", c => c.Int());
            CreateIndex("dbo.Matches", "Bet_Id");
            AddForeignKey("dbo.Matches", "Bet_Id", "dbo.Bets", "Id");
        }
    }
}
