namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RageLevel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Character", "RageLevel", c => c.Short(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Character", "RageLevel");
        }
    }
}
