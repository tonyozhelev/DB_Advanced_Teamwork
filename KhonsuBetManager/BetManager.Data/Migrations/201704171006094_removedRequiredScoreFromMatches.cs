namespace BetManager.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedRequiredScoreFromMatches : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Matches", "Score", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Matches", "Score", c => c.String(nullable: false));
        }
    }
}
