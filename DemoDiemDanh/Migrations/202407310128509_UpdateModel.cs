namespace DemoDiemDanh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
            "dbo.diemdanh",
            c => new
            {
                madd = c.Long(nullable: false),
                masv = c.String(maxLength: 255),
                nhomhp = c.String(nullable: false, maxLength: 255),
                ngaydd = c.DateTime(nullable: false),
                tinhtrang = c.String(maxLength: 50),
            })
            .PrimaryKey(t => t.madd);
        }

        public override void Down()
        {
            DropPrimaryKey("dbo.diemdanh");
            AlterColumn("dbo.diemdanh", "tinhtrang", c => c.String());
            AlterColumn("dbo.diemdanh", "nhomhp", c => c.String());
            AlterColumn("dbo.diemdanh", "masv", c => c.String());
            AlterColumn("dbo.diemdanh", "madd", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.diemdanh", "madd");
            
        }
    }
}
