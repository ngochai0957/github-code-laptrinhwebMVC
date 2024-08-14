namespace DemoDiemDanh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDiemDanh : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.diemdanh");
            AlterColumn("dbo.diemdanh", "madd", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.diemdanh", "madd");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.diemdanh");
            AlterColumn("dbo.diemdanh", "madd", c => c.Long(nullable: false));
            AddPrimaryKey("dbo.diemdanh", "madd");
        }
    }
}
