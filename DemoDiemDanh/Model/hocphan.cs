namespace DemoDiemDanh.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("hocphan")]
    public partial class hocphan
    {
        [StringLength(255)]
        public string mahp { get; set; }

        [StringLength(255)]
        public string tenhp { get; set; }

        [Key]
        [StringLength(255)]
        public string nhomhp { get; set; }

        public int tinchi { get; set; }

        [StringLength(50)]
        public string tiethoc { get; set; }

        [StringLength(255)]
        public string tenphong { get; set; }

        [StringLength(50)]
        public string thu { get; set; }
    }
}
