using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace DemoDiemDanh.Model
{
    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model1")
        {
        }

        public virtual DbSet<diemdanh> diemdanhs { get; set; }
        public virtual DbSet<giangvien> giangviens { get; set; }
        public virtual DbSet<hocphan> hocphans { get; set; }
        public virtual DbSet<sinhvien> sinhviens { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
