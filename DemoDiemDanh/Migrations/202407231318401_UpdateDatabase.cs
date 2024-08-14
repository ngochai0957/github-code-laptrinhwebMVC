namespace DemoDiemDanh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDatabase : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.hocphan", "tiethoc", c => c.String(maxLength: 50));
            AlterColumn("dbo.hocphan", "thu", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.hocphan", "thu", c => c.Int(nullable: false));
            AlterColumn("dbo.hocphan", "tiethoc", c => c.Int(nullable: false));
        }
    }
}
