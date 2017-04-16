namespace BetManager.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMatchBetsMappingWithResult : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MatchBets", "Match_Id", "dbo.Matches");
            DropForeignKey("dbo.MatchBets", "Bet_Id", "dbo.Bets");
            DropIndex("dbo.MatchBets", new[] { "Match_Id" });
            DropIndex("dbo.MatchBets", new[] { "Bet_Id" });
            CreateTable(
                "dbo.MatchesBets",
                c => new
                    {
                        BetId = c.Int(nullable: false),
                        MatchId = c.Int(nullable: false),
                        Result = c.String(),
                    })
                .PrimaryKey(t => new { t.BetId, t.MatchId })
                .ForeignKey("dbo.Bets", t => t.BetId, cascadeDelete: true)
                .ForeignKey("dbo.Matches", t => t.MatchId, cascadeDelete: true)
                .Index(t => t.BetId)
                .Index(t => t.MatchId);
            
            AddColumn("dbo.Matches", "Bet_Id", c => c.Int());
            CreateIndex("dbo.Matches", "Bet_Id");
            AddForeignKey("dbo.Matches", "Bet_Id", "dbo.Bets", "Id");
            DropTable("dbo.MatchBets");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.MatchBets",
                c => new
                    {
                        Match_Id = c.Int(nullable: false),
                        Bet_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Match_Id, t.Bet_Id });
            
            DropForeignKey("dbo.Matches", "Bet_Id", "dbo.Bets");
            DropForeignKey("dbo.MatchesBets", "MatchId", "dbo.Matches");
            DropForeignKey("dbo.MatchesBets", "BetId", "dbo.Bets");
            DropIndex("dbo.MatchesBets", new[] { "MatchId" });
            DropIndex("dbo.MatchesBets", new[] { "BetId" });
            DropIndex("dbo.Matches", new[] { "Bet_Id" });
            DropColumn("dbo.Matches", "Bet_Id");
            DropTable("dbo.MatchesBets");
            CreateIndex("dbo.MatchBets", "Bet_Id");
            CreateIndex("dbo.MatchBets", "Match_Id");
            AddForeignKey("dbo.MatchBets", "Bet_Id", "dbo.Bets", "Id", cascadeDelete: true);
            AddForeignKey("dbo.MatchBets", "Match_Id", "dbo.Matches", "Id", cascadeDelete: true);
        }
    }
}
