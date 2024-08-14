namespace DemoDiemDanh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDatabase : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.sinhvien", "diemdanh_madd", "dbo.diemdanh");
            DropIndex("dbo.sinhvien", new[] { "diemdanh_madd" });
            DropColumn("dbo.sinhvien", "diemdanh_madd");
        }
        
        public override void Down()
        {
            AddColumn("dbo.sinhvien", "diemdanh_madd", c => c.Long());
            CreateIndex("dbo.sinhvien", "diemdanh_madd");
            AddForeignKey("dbo.sinhvien", "diemdanh_madd", "dbo.diemdanh", "madd");
        }
    }
}
