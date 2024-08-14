namespace DemoDiemDanh.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("sinhvien")]
    public partial class sinhvien
    {
        [Key]
        [StringLength(255)]
        public string masv { get; set; }

        [Required]
        [StringLength(255)]
        public string tensv { get; set; }

        [Required]
        [StringLength(255)]
        public string ngaysinh { get; set; }

        [StringLength(255)]
        public string gioitinh { get; set; }

        [StringLength(255)]
        public string lop { get; set; }

        [StringLength(255)]
        public string hocphan { get; set; }
    }
}
