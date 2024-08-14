using DemoDiemDanh.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace DemoDiemDanh
{
    public class DetailViewModel
    {
        public List<sinhvien> SinhViens { get; set; } = new List<sinhvien>();
        public List<diemdanh> Attendance { get; set; } = new List<diemdanh>();
    }
}