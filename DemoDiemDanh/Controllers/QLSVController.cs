using DemoDiemDanh.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;

namespace DemoDiemDanh.Controllers
{
    public class QLSVController : Controller
    {
        private Model1 db = new Model1();

        public ActionResult sinhvien()
        {
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

            //int pageSize = 20; // Number of items per page
            //int pageNumber = (page ?? 1); // Current page number
            var sinhviens = db.sinhviens.OrderBy(s => s.masv).ToList();

            //ViewBag.PageNumber = pageNumber;
            //ViewBag.PageSize = pageSize;

            return View(sinhviens);
        }
        public ActionResult sinhvienadd()
        {
            string nhomhp = Request.QueryString["nhomhp"];
            ViewBag.Mahp = nhomhp;
            var magv = Session["magv"] as string;
            if (!string.IsNullOrEmpty(magv))
            {
                using (var db = new Model1())
                {
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
                }
            }

            var sinhviens = db.sinhviens.OrderBy(s => s.masv).ToList();
            return View(sinhviens);
        }


        public ActionResult themsinhvien()
        {
            return View();
        }
        [HttpPost]
        public ActionResult themsinhvien(string Tensv, DateTime Ngaysinh, string Gioitinh, string Lop)
        {
            string ns = Ngaysinh.ToString("dd/MM/yyyy");
            // Validate the input
            if (string.IsNullOrEmpty(Tensv) || string.IsNullOrEmpty(ns) || string.IsNullOrEmpty(Gioitinh) || string.IsNullOrEmpty(Lop))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin.";
                return RedirectToAction("sinhvien");
            }

            // Extract the last two digits from the class code
            string prefix = Lop.Length >= 8 ? Lop.Substring(6, 2) : ""; // Get the last two digits from ĐHCNTTXX
            if (string.IsNullOrEmpty(prefix))
            {
                ViewBag.Error = "Lớp không hợp lệ.";
                return View();
            }

            // Generate student ID with the prefix
            Random random = new Random();
            int randomNumber = random.Next(1000, 9999); // Generate a random number between 1000 and 9999
            string Masv = "00" + prefix + "41" + randomNumber;

            var sinhvien = new sinhvien
            {
                masv = Masv,
                tensv = Tensv,
                ngaysinh = ns,
                gioitinh = Gioitinh,
                lop = Lop
            };
            db.sinhviens.Add(sinhvien);
            db.SaveChanges();
            TempData["Message"] = "Thêm sinh viên thành công.";
            return RedirectToAction("sinhvien");
        }

        public ActionResult GetNamhocOptions()
        {
            // Lấy tất cả lớp học từ cơ sở dữ liệu
            var classes = db.sinhviens
                .Select(s => s.lop)
                .Distinct()
                .ToList();

            // Xử lý dữ liệu trong bộ nhớ
            var namhocOptions = classes
                .Select(lop => lop.Substring(6, 2)) // Lấy năm từ lớp học
                .Distinct()
                .OrderByDescending(namhoc => namhoc)
                .Select(namhoc => new
                {
                    Value = namhoc,
                    Text = $"Khóa {namhoc}"
                })
                .ToList();

            return Json(namhocOptions, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetClassesByNamhoc(int namhoc)
        {
            // Lấy tất cả các lớp học từ cơ sở dữ liệu và phân loại theo năm khóa
            var prefix = $"ĐHCNTT{namhoc}";
            var classes = db.sinhviens
                .Where(s => s.lop.StartsWith(prefix))
                .Select(s => s.lop)
                .Distinct()
                .OrderBy(lop => lop)
                .ToList();

            // Chuyển đổi danh sách lớp học thành danh sách tùy chọn cho dropdown
            var options = classes.Select(lop => new
            {
                Value = lop,
                Text = lop
            }).ToList();

            return Json(options, JsonRequestBehavior.AllowGet);
        }



        public ActionResult GetStudentsByLop(string lop)
        {
            var students = db.sinhviens
                             .Where(s => s.lop == lop)
                             .ToList();
            return PartialView("_StudentTable", students);
        }

        [HttpPost]
        public ActionResult DeleteSV(string[] selectedIds)
        {
            if (selectedIds != null && selectedIds.Length > 0)
            {
                foreach (var masv in selectedIds)
                {
                    var sinhVienToDelete = db.sinhviens.FirstOrDefault(s => s.masv == masv);

                    if (sinhVienToDelete != null)
                    {
                        db.sinhviens.Remove(sinhVienToDelete);
                    }
                }

                db.SaveChanges();
            }

            return Json(new { success = true });
        }
        [HttpPost]
        public ActionResult addsvhp(string[] selectedIds, string nhomhp)
        {
            if (selectedIds != null && selectedIds.Length > 0)
            {
                foreach (var masv in selectedIds)
                {
                    var sinhVienToDelete = db.sinhviens.FirstOrDefault(s => s.masv == masv);



                    if (sinhVienToDelete != null)
                    {
                        string newHocPhanEntry = $"{nhomhp}";

                        if (string.IsNullOrEmpty(sinhVienToDelete.hocphan))
                        {
                            sinhVienToDelete.hocphan = newHocPhanEntry;
                        }
                        else
                        {
                            sinhVienToDelete.hocphan += $", {newHocPhanEntry}";
                        }

                        // Save changes to the database
                        db.SaveChanges();
                    }
                }

                db.SaveChanges();
            }

            return Json(new { success = true });
        }
        

        [HttpPost]
        public ActionResult EditSV(List<sinhvien> editedStudents)
        {
            try
            {
                foreach (var student in editedStudents)
                {
                    var existingStudent = db.sinhviens.Find(student.masv);
                    if (existingStudent != null)
                    {
                        existingStudent.tensv = student.tensv;
                        existingStudent.ngaysinh = student.ngaysinh;
                        existingStudent.gioitinh = student.gioitinh;
                        existingStudent.lop = student.lop;
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
    }
}
