namespace DemoDiemDanh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDatabaseSchema : DbMigration
    {
        public override void Up()
        {
            // Xóa khóa ngoại từ bảng giangvien
            DropForeignKey("dbo.sinhvien", "hocphan", "dbo.hocphan");
        }
        
        public override void Down()
        {
            
          
            
           
            DropTable("dbo.hocphan");
        }
    }
}
