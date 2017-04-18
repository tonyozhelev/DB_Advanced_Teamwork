namespace BetManager.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBetPredictionToMatchesBets : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MatchesBets", "BetPrediction", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MatchesBets", "BetPrediction");
        }
    }
}
