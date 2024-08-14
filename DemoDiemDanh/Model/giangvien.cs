namespace DemoDiemDanh.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("giangvien")]
    public partial class giangvien
    {
        [Key]
        [StringLength(255)]
        public string magv { get; set; }

        [StringLength(255)]
        public string tengv { get; set; }

        [StringLength(255)]
        public string gioitinh { get; set; }

        [StringLength(255)]
        public string email { get; set; }

        [StringLength(255)]
        public string anhgv { get; set; }

        [StringLength(255)]
        public string hocvi { get; set; }

        [Required]
        [StringLength(255)]
        public string matkhau { get; set; }

        [StringLength(255)]
        public string hocphanqly { get; set; }
    }
}
