using DemoDiemDanh.Model;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoDiemDanh.Controllers
{
    public class QLGVController : Controller
    {
        private Model1 db = new Model1();
        // GET: Teachers
        
        public ActionResult giangvien()
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
            var gv = db.giangviens.Where(g => g.magv == magv).FirstOrDefault();

            if (gv != null)
            {
                if (string.IsNullOrEmpty(gv.anhgv))
                {
                    gv.anhgv = "/Image/avtdefault.jpg";
                }
            }

            return View(gv);
        }
        public ActionResult giangvienforadmin()
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

            List<giangvien> dsgv = db.giangviens.ToList();
            
            return View(dsgv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            // Xóa session
            Session.Clear();
            Session.Abandon();

            // Chuyển hướng đến trang đăng nhập
            return RedirectToAction("dangnhap", "DangNhap");
        }

        // Action để hiển thị trang đăng nhập (nếu cần)
        public ActionResult DangNhap()
        {
            return View();
        }

        // Action để hiển thị trang đăng nhập (nếu cần)
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult themgiangvien()
        {
            return View();
        }
        [HttpPost]
        public ActionResult themgiangvien(string email, string matkhau)
        {
            // Validate the input
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(matkhau))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin.";
                return RedirectToAction("giangvienforadmin");
            }
            Random random = new Random();
            int part1 = random.Next(10000, 100000); // 5 chữ số đầu tiên
            int part2 = random.Next(10000, 100000); // 5 chữ số tiếp theo
            string magv = part1.ToString() + part2.ToString();
            var giangvien = new giangvien
            {
                magv = magv,
                email = email,
                matkhau = matkhau
            };

            db.giangviens.Add(giangvien);
            db.SaveChanges();
            TempData["Message"] = "Thêm giảng viên thành công.";
            return RedirectToAction("giangvienforadmin");
        }

        [HttpPost]
        public ActionResult luuthongtin(giangvien model)
        {
            // Kiểm tra phía server
            if (string.IsNullOrEmpty(model.tengv) || string.IsNullOrEmpty(model.email) ||
                string.IsNullOrEmpty(model.gioitinh) || string.IsNullOrEmpty(model.hocvi))
            {
                // Nếu có lỗi, quay lại form với thông báo lỗi
                ModelState.AddModelError("", "Vui lòng điền đầy đủ thông tin");
                return View(model);
            }

            // Tìm giảng viên dựa vào magv
            var giangVien = db.giangviens.SingleOrDefault(gv => gv.magv == model.magv);
            if (giangVien != null)
            {
                // Cập nhật thông tin
                giangVien.tengv = model.tengv;
                giangVien.email = model.email;
                giangVien.gioitinh = model.gioitinh;
                giangVien.hocvi = model.hocvi;
                

                // Lưu thay đổi vào database
                db.SaveChanges();
            }
            TempData["Message"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("giangvien"); // Chuyển hướng về trang Index hoặc trang phù hợp
        }

        [HttpPost]
        public ActionResult doimk(string matkhau, string mkmoi, string xacnhanmk)
        {
            // Lấy magv từ Session
            var magv = Session["magv"] as string;

            if (mkmoi == xacnhanmk)
            {
                var giangvien = db.giangviens.SingleOrDefault(g => g.magv == magv);

                if (giangvien != null)
                {
                    if (giangvien.matkhau == matkhau)
                    {
                        giangvien.matkhau = mkmoi;
                        db.SaveChanges();
                        TempData["Message"] = "Đổi mật khẩu thành công";
                        return RedirectToAction("giangvien");
                    }
                    else
                    {
                        TempData["Error"] = "Mật khẩu hiện tại không đúng.";
                    }
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy giảng viên.";
                }
            }
            else
            {
                TempData["Error"] = "Xác nhận mật khẩu mới không khớp.";
            }
            return RedirectToAction("giangvien");
        }



    }
}