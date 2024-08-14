namespace DemoDiemDanh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDiemDanh1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.diemdanh", "ngaydd", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.diemdanh", "ngaydd", c => c.DateTime(nullable: false));
        }
    }
}
