namespace BetManager.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WTFMigration : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "LastLogin", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "LastLogin", c => c.DateTime(nullable: false));
        }
    }
}
