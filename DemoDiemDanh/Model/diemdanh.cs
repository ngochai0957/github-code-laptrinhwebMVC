namespace DemoDiemDanh.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("diemdanh")]
    public partial class diemdanh
    {
        [Key]
        public long madd { get; set; }

        [StringLength(255)]
        public string masv { get; set; }

        [Required]
        [StringLength(255)]
        public string nhomhp { get; set; }

        public string ngaydd { get; set; }

        [StringLength(50)]
        public string tinhtrang { get; set; }
    }
}
