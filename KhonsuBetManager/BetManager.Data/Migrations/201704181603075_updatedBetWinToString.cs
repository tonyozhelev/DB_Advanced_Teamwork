namespace BetManager.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedBetWinToString : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bets", "Win", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bets", "Win");
        }
    }
}
