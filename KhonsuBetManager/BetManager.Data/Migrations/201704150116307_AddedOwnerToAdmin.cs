namespace BetManager.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOwnerToAdmin : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Admins", "Owner", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Admins", "Owner");
        }
    }
}
