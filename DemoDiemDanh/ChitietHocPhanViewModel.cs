using DemoDiemDanh.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoDiemDanh
{
    public class ChitietHocPhanViewModel
    {
        public List<HocPhanDetail> HocPhanDetails { get; set; }
        public List<sinhvien> SinhViens { get; set; }
    }
}