namespace DemoDiemDanh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HocPhan : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.hocphan",
                c => new
                    {
                        mahp = c.String(nullable: false, maxLength: 255),
                        tenhp = c.String(maxLength: 255),
                        tinchi = c.Int(nullable: false, defaultValue: 0),
                        tiethoc = c.Int(nullable: false, defaultValue: 0),
                        tenphong = c.String(maxLength: 255),
                        thu = c.Int(nullable: false, defaultValue: 0),

                })
                .PrimaryKey(t => t.mahp);

            // Thêm cột hocphan vào bảng sinhvien
            AddColumn("dbo.sinhvien", "hocphan", c => c.String(maxLength: 255));

            // Thiết lập khóa ngoại
            AddForeignKey("dbo.giangvien", "hocphanqly", "dbo.hocphan", "mahp");
            AddForeignKey("dbo.sinhvien", "hocphan", "dbo.hocphan", "mahp");

            // Tạo chỉ mục (index) cho khóa ngoại nếu cần thiết
            CreateIndex("dbo.giangvien", "hocphanqly");
            CreateIndex("dbo.sinhvien", "hocphan");

        }
        
        public override void Down()
        {
            // Xóa chỉ mục (index) nếu cần rollback migration
            DropIndex("dbo.sinhvien", new[] { "hocphan" });
            DropIndex("dbo.giangvien", new[] { "hocphanqly" });

            // Xóa khóa ngoại
            DropForeignKey("dbo.sinhvien", "hocphan", "dbo.hocphan");
            DropForeignKey("dbo.giangvien", "hocphanqly", "dbo.hocphan");

            // Xóa cột hocphan từ bảng sinhvien
            DropColumn("dbo.sinhvien", "hocphan");

            // Xóa cột hocphanqly từ bảng giangvien
            DropColumn("dbo.giangvien", "hocphanqly");

            // Xóa bảng hocphan
            DropTable("dbo.hocphan");
        }
    }
}
