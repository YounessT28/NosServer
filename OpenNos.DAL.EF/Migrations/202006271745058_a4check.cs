﻿namespace OpenNos.DAL.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class a4check : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Character", "BattleTowerLastStage", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Character", "BattleTowerLastStage");
        }
    }
}
