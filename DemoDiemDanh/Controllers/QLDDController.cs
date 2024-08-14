using DemoDiemDanh.Model;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using QRCoder;
using System.Drawing;

namespace DemoDiemDanh.Controllers
{
    public class QLDDController : Controller
    {
        private Model1 db = new Model1();
        // GET: QLDD
        public ActionResult diemdanh()
        {
            // Lấy magv từ Session
            var magv = Session["magv"] as string;

            if (!string.IsNullOrEmpty(magv))
            {
                using (var db = new Model1())
                {
                    // Truy vấn tên giảng viên bằng magv
                    var tengv = db.giangviens
                        .Where(g => g.magv == magv)
                        .Select(g => g.tengv)
                        .FirstOrDefault();

                    if (tengv == null)
                    {
                        var email = db.giangviens
                        .Where(g => g.magv == magv)
                        .Select(g => g.email)
                        .FirstOrDefault();
                        ViewBag.TitleName = email;
                    }
                    else
                    {
                        ViewBag.TitleName = tengv;
                    }
                    // Lưu tên giảng viên vào ViewBag

                }
            }

            return View();
        }

        [HttpGet]
        public JsonResult GetHocPhanByGiangVien()
        {
            try
            {
                // Lấy magv từ Session
                var magv = Session["magv"] as string;

                // In ra magv để kiểm tra
                Console.WriteLine("magv: " + magv);

                // Bước 1: Lấy danh sách hocphanqly
                var hocphanqlyList = db.giangviens
                    .Where(gv => gv.magv == magv)
                    .Select(gv => gv.hocphanqly)
                    .FirstOrDefault();

                // In ra hocphanqlyList để kiểm tra
                Console.WriteLine("hocphanqlyList: " + hocphanqlyList);

                if (hocphanqlyList == null)
                {
                    return Json(new List<HocPhanDetail>(), JsonRequestBehavior.AllowGet);
                }

                // Bước 2: Tách hocphanqly thành mảng
                var hocphanIds = hocphanqlyList.Split(',');

                // In ra hocphanIds để kiểm tra
                Console.WriteLine("hocphanIds: " + string.Join(", ", hocphanIds));

                var hocPhanDetails = new List<HocPhanDetail>();

                foreach (var maHp in hocphanIds)
                {
                    // Bước 3: Lấy danh sách các học phần theo mahp
                    var hocPhans = db.hocphans
                        .Where(hp => hp.mahp == maHp.Trim())
                        .ToList();

                    // In ra hocPhans để kiểm tra
                    Console.WriteLine("hocPhans for maHp " + maHp + ": " + hocPhans.Count);

                    foreach (var hocPhan in hocPhans)
                    {
                        if (hocPhan != null)
                        {
                            var thuChars = hocPhan.thu.ToCharArray();
                            var tietParts = hocPhan.tiethoc.Split('-');
                            var nhomHPs = hocPhan.nhomhp.Split(',');

                            for (int i = 0; i < thuChars.Length; i++)
                            {
                                foreach (var nhomhp in nhomHPs)
                                {
                                    if (i < tietParts.Length)
                                    {
                                        hocPhanDetails.Add(new HocPhanDetail
                                        {
                                            MaHocPhan = hocPhan.mahp,
                                            TenHocPhan = hocPhan.tenhp,
                                            TenPhong = hocPhan.tenphong,
                                            Thu = thuChars[i].ToString(),
                                            Tiethoc = tietParts[i],
                                            NhomHP = nhomhp
                                        });

                                        // In ra chi tiết học phần để kiểm tra
                                        Console.WriteLine("Added HocPhanDetail: " + hocPhan.tenhp + ", " + nhomhp);
                                    }
                                }
                            }
                        }
                    }
                }

                return Json(hocPhanDetails, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log lỗi ra console hoặc file log
                Console.WriteLine("Exception: " + ex.Message);
                // Trả về thông báo lỗi
                return Json(new { success = false, message = "Có lỗi xảy ra trong quá trình xử lý dữ liệu." }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpGet]
        public JsonResult GetStudentsByNhomHP(string nhomhp)
        {
            if (string.IsNullOrEmpty(nhomhp))
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }

            var students = db.sinhviens
                .Where(sv => sv.hocphan == nhomhp)
                .Select(sv => new
                {
                    sv.masv,
                    sv.tensv,
                    sv.lop,
                    sv.ngaysinh,
                    sv.gioitinh
                    // Thêm các thuộc tính khác nếu cần
                })
                .ToList();

            return Json(students, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult diemdanhsv(List<Attendance> attendanceData, string nhomhp)
        {
            DateTime ngaydd = DateTime.Now;
            Random random = new Random();
            int part1 = random.Next(10000, 100000); // 5 chữ số đầu tiên
            int part2 = random.Next(10000, 100000); // 5 chữ số tiếp theo
            string madd = part1.ToString() + part2.ToString();
            string ngay = ngaydd.ToString("dd/MM/yyyy");
            if (attendanceData != null && attendanceData.Count > 0)
            {
                foreach (var attendance in attendanceData)
                {
                    var masv = attendance.masv;
                    var tinhtrang = attendance.tinhtrang;

                    var diemdanhsv = new diemdanh
                    {
                        madd = long.Parse(madd),  // Chuyển đổi chuỗi thành kiểu long
                        masv = masv,
                        nhomhp = nhomhp,
                        ngaydd = ngay,
                        tinhtrang = tinhtrang
                    };
                    db.diemdanhs.Add(diemdanhsv);
                }

                db.SaveChanges();
            }
            else
            {
                // Log nếu không có dữ liệu attendanceData
                System.Diagnostics.Debug.WriteLine("attendanceData is null or empty");
            }

            return Json(new { success = true });
        }

        [HttpGet]
        public JsonResult CheckNgayNhomHP(string ngay, string nhomhp)
        {
            

            // Chuyển đổi chuỗi ngày thành DateTime
            DateTime ngayDateTime;
            string ngaydd = "";
            if (DateTime.TryParse(ngay, out ngayDateTime))
            {
                // Định dạng ngày thành dd/MM/yyyy
                ngaydd = ngayDateTime.ToString("dd/MM/yyyy");

            }
            var exists = db.diemdanhs.Any(a => a.ngaydd == ngaydd && a.nhomhp == nhomhp);
            
            return Json(new { exists }, JsonRequestBehavior.AllowGet);
           
        }

        public class Attendance
        {
            public string masv { get; set; }
            public string tinhtrang { get; set; }
        }

        // Hiển thị bảng điểm danh

        public ActionResult chitietdiemdanh(string nhomhp)
        {
            if (string.IsNullOrEmpty(nhomhp))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Lấy danh sách mã số sinh viên từ bảng diemdanh dựa vào nhomhp
            var masvs = db.diemdanhs
                .Where(dd => dd.nhomhp == nhomhp)
                .Select(dd => dd.masv)
                .Distinct()
                .ToList();

            // Lấy thông tin sinh viên dựa vào danh sách mã số sinh viên
            var sinhViens = db.sinhviens
                .Where(sv => masvs.Contains(sv.masv))
                .ToList();

            // Lấy thông tin điểm danh
            var attendance = db.diemdanhs
                .Where(dd => masvs.Contains(dd.masv) && dd.nhomhp == nhomhp)
                .ToList();

            // Tạo ViewModel
            var viewModel = new DetailViewModel
            {
                SinhViens = sinhViens,
                Attendance = attendance
            };

            return View(viewModel);
        }
        [HttpPost]
        public JsonResult SaveAttendance(List<AttendanceViewModel> attendanceData, string nhomhp)
        {
            string ngay = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                if (attendanceData == null || !attendanceData.Any())
                {
                    return Json(new { success = false, message = "No attendance data provided." });
                }

                foreach (var item in attendanceData)
                {
                    // Tìm bản ghi diemdanh hiện có dựa trên masv, nhomhp và ngaydd
                    var existingAttendance = db.diemdanhs.FirstOrDefault(dd => dd.masv == item.masv && dd.nhomhp == nhomhp && dd.ngaydd == ngay);

                    if (existingAttendance != null)
                    {
                        // Cập nhật tinhtrang nếu bản ghi đã tồn tại
                        existingAttendance.tinhtrang = item.tinhtrang;
                    }
                    else
                    {
                        // Thêm bản ghi mới nếu không tìm thấy
                        diemdanh dd = new diemdanh
                        {
                            masv = item.masv,
                            tinhtrang = item.tinhtrang,
                            ngaydd = ngay,
                            nhomhp = nhomhp
                        };
                        db.diemdanhs.Add(dd);
                    }
                }

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public class AttendanceViewModel
        {
            public string masv { get; set; }
            public string tinhtrang { get; set; }
        }



    }
}