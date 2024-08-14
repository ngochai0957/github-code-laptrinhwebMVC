using DemoDiemDanh.Migrations;
using DemoDiemDanh.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DemoDiemDanh.Controllers
{
    public class QLHPController : Controller
    {
        private Model1 db = new Model1();
        // GET: QLHP
        public ActionResult hocphan()
        {
            // Lấy magv từ Session
            var magvv = Session["magv"] as string;

            if (!string.IsNullOrEmpty(magvv))
            {
                using (var db = new Model1())
                {
                    // Truy vấn tên giảng viên bằng magv
                    var tengv = db.giangviens
                        .Where(g => g.magv == magvv)
                        .Select(g => g.tengv)
                        .FirstOrDefault();

                    if (tengv == null)
                    {
                        var email = db.giangviens
                        .Where(g => g.magv == magvv)
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
            string magv = Session["magv"] as string;

            // Lấy chuỗi hocphanqly từ bảng giangvien
            var hocphanqlyString = db.giangviens
                                     .Where(g => g.magv == magv)
                                     .Select(g => g.hocphanqly)
                                     .FirstOrDefault();

            // Tách chuỗi hocphanqly thành danh sách các giá trị mahp
            var managedHocPhans = hocphanqlyString?.Split(',').Select(hp => hp.Trim()).ToList() ?? new List<string>();

            // Lấy tất cả các học phần
            var hocPhans = db.hocphans.ToList();

            ViewBag.ManagedHocPhans = managedHocPhans;
            ViewBag.Magv = magv;

            return View(hocPhans);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteHocPhan(string nhomhp)
        {
            try
            {
                var hocPhan = db.hocphans.SingleOrDefault(h => h.nhomhp == nhomhp);

                if (hocPhan != null)
                {
                    db.hocphans.Remove(hocPhan);

                    // Extract the part before the hyphen
                    var prefix = nhomhp.Split('-')[0];

                    // Check if mahp exists in other hocphan records
                    bool isMahpExists = db.hocphans.Any(h => h.mahp == hocPhan.mahp && h.nhomhp != nhomhp);

                    if (!isMahpExists)
                    {
                        // Get the giangvien records that contain the prefix in hocphanqly
                        var giangViens = db.giangviens.Where(gv => gv.hocphanqly.Contains(hocPhan.mahp)).ToList();

                        foreach (var gv in giangViens)
                        {
                            var hocPhanQlyList = gv.hocphanqly.Split(',').ToList();
                            foreach (var hp in hocPhanQlyList)
                            {
                                if (hp.Contains(hocPhan.mahp))
                                {
                                    hocPhanQlyList.Remove(hp);
                                    gv.hocphanqly = string.Join(",", hocPhanQlyList);
                                    db.Entry(gv).State = EntityState.Modified;
                                    break;
                                }
                            }
                        }
                    }

                    db.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Học phần không tồn tại" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }


        [HttpGet]
        public JsonResult GetHocPhan(string nhomhp)
        {
            var hocPhan = db.hocphans.FirstOrDefault(hp => hp.nhomhp == nhomhp);
            string nhom = nhomhp.TrimStart();
            var dashIndex = nhomhp.IndexOf('-');
            var nhomhpSuffix = dashIndex >= 0 ? nhom.Substring(dashIndex + 1) : string.Empty;

            if (hocPhan != null)
            {

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        mahp = hocPhan.mahp,
                        tenhp = hocPhan.tenhp,
                        tinchi = hocPhan.tinchi,
                        tenphong = hocPhan.tenphong,
                        nhomhp = nhomhpSuffix,
                        thu = hocPhan.thu,
                        tiethoc = hocPhan.tiethoc
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "Không tìm thấy học phần" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateHocPhan(HocPhanViewModel model)
        {
            try
            {
                var hocPhan = db.hocphans.FirstOrDefault(h => h.mahp == model.Mahp);
                if (hocPhan != null)
                {
                    hocPhan.tenhp = model.Tenhp;
                    hocPhan.tinchi = model.Tinchi;
                    hocPhan.tiethoc = model.Tiethoc;
                    hocPhan.tenphong = model.Tenphong; // Đảm bảo tên trường đúng
                    hocPhan.thu = model.Thu;

                    db.SaveChanges();
                    return RedirectToAction("Index"); // Chuyển hướng đến trang danh sách hoặc trang cần thiết
                }
                ViewBag.ErrorMessage = "Không tìm thấy học phần";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return RedirectToAction("hocphan"); // Trả về view hoặc modal nếu cần
        }



        [HttpPost]
        public ActionResult DeleteSVHP(string[] selectedIds, string nhomhp)
        {
            if (selectedIds != null && selectedIds.Length > 0)
            {
                foreach (var masv in selectedIds)
                {
                    var sinhVienToUpdate = db.sinhviens.SingleOrDefault(s => s.masv == masv);

                    if (sinhVienToUpdate != null)
                    {
                        var hocPhans = sinhVienToUpdate.hocphan.Split(',').ToList();

                        // Remove the nhomhp from the hocphan list
                        hocPhans.RemoveAll(hp => hp.Equals(nhomhp, StringComparison.OrdinalIgnoreCase));

                        // Update hocphan field
                        sinhVienToUpdate.hocphan = string.Join(",", hocPhans);

                        db.Entry(sinhVienToUpdate).State = EntityState.Modified;
                    }
                }

                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Không có sinh viên nào được chọn" });
        }


        public class HocPhanViewModel
        {
            public string Mahp { get; set; }
            public string Tenhp { get; set; }
            public string Nhomhp { get; set; }
            public int Tinchi { get; set; }
            public string Thu {  get; set; }
            public string Tenphong { get; set; }
            public string Tiethoc { get; set; }
            // Thêm các thuộc tính khác ở đây
        }
        // POST: QLHP/ThemHocPhan
        [HttpPost]

        public ActionResult ThemHocPhan(string mahp, string tenhp, string tinchi, string nhomhp, string tiethoc, string tenphong, string thu)
        {
            // Lấy mã giảng viên từ Session
            var magv = Session["magv"] as string;
            var selectgv = db.giangviens.Where(g => g.magv.Equals(magv)).FirstOrDefault();

            // Kiểm tra các tham số đầu vào có hợp lệ không
            if (!string.IsNullOrWhiteSpace(mahp) && !string.IsNullOrWhiteSpace(tenhp) &&
                !string.IsNullOrWhiteSpace(tinchi) &&
                !string.IsNullOrWhiteSpace(tiethoc) &&
                !string.IsNullOrWhiteSpace(tenphong) &&
                !string.IsNullOrWhiteSpace(thu))
            {
                // Chuyển đổi các tham số từ kiểu string sang kiểu dữ liệu phù hợp
                if (int.TryParse(tinchi, out int tinchiInt))
                    
                {
                    // Kiểm tra xem mã học phần đã tồn tại chưa
                    var existingHocPhan = db.hocphans.FirstOrDefault(hp => hp.mahp == mahp);

                    
                    string nhomhphan = mahp +'-'+ nhomhp;
                    // Tạo đối tượng HocPhan mới và thiết lập các thuộc tính
                    var newHocPhan = new hocphan
                    {
                        nhomhp = nhomhphan,
                        mahp = mahp,
                        tenhp = tenhp,
                        tinchi = tinchiInt,
                        tiethoc = tiethoc,
                        tenphong = tenphong,
                        thu = thu
                    };

                    // Thêm đối tượng HocPhan vào cơ sở dữ liệu
                    db.hocphans.Add(newHocPhan);
                    db.SaveChanges();

                    // Kiểm tra mã học phần mới đã được thêm vào cơ sở dữ liệu chưa
                    var addedHocPhan = db.hocphans.Where(h => h.mahp == mahp).FirstOrDefault();
                    if (addedHocPhan != null)
                    {
                        // Cập nhật trường hocphanqly của giảng viên
                        if (selectgv != null)
                        {
                            if (string.IsNullOrEmpty(selectgv.hocphanqly))
                            {
                                selectgv.hocphanqly = mahp;
                            }
                            else
                            {
                                var hocphanList = selectgv.hocphanqly.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(hp => hp.Trim()).ToList();

                                if (!hocphanList.Contains(mahp))
                                {
                                    hocphanList.Add(mahp);
                                    selectgv.hocphanqly = string.Join(", ", hocphanList);
                                }
                            }

                            db.SaveChanges();
                        }


                        // Chuyển hướng người dùng về trang Index hoặc trang khác
                        return RedirectToAction("hocphan");
                    }
                    else
                    {
                        // Mã học phần không được thêm vào cơ sở dữ liệu, xử lý lỗi
                        ModelState.AddModelError("", "Mã học phần không được thêm vào cơ sở dữ liệu.");
                    }
                }
                else
                {
                    // Thông báo lỗi nếu không thể chuyển đổi số tín chỉ, tiết học hoặc thứ
                    ModelState.AddModelError("", "Số tín chỉ, tiết học hoặc thứ không hợp lệ.");
                }
            }
            else
            {
                // Thông báo lỗi nếu các tham số đầu vào không hợp lệ
                ModelState.AddModelError("", "Các thông tin không được để trống.");
            }

            return RedirectToAction("hocphan");
        }
        // Action để hiển thị chi tiết học phần
        public ActionResult ChitietHocPhan(string nhomhp)
        {
            string magv = Session["magv"] as string;

            // Lấy chuỗi hocphanqly từ bảng giangvien
            var hocphanqlyString = db.giangviens
                                     .Where(g => g.magv == magv)
                                     .Select(g => g.hocphanqly)
                                     .FirstOrDefault();

            // Tách chuỗi hocphanqly thành danh sách các giá trị mahp
            var managedHocPhans = hocphanqlyString?.Split(',').Select(hp => hp.Trim()).ToList() ?? new List<string>();

            var hocPhan = db.hocphans
                .Where(hp => hp.nhomhp == nhomhp)
                .Select(hp => new
                {
                    MaHocPhan = hp.mahp,
                    TenHocPhan = hp.tenhp,
                    TenPhong = hp.tenphong,
                    Thu = hp.thu,
                    Tiet = hp.tiethoc,
                    NhomHP = nhomhp
                })
                .FirstOrDefault();

            if (hocPhan == null)
            {
                return HttpNotFound();
            }

            var thuChars = hocPhan.Thu.ToCharArray();
            var tietParts = hocPhan.Tiet.Split('-');

            var hocPhanDetails = new List<HocPhanDetail>();

            for (int i = 0; i < thuChars.Length; i++)
            {
                if (i < tietParts.Length)
                {
                    hocPhanDetails.Add(new HocPhanDetail
                    {
                        MaHocPhan = hocPhan.MaHocPhan,
                        TenHocPhan = hocPhan.TenHocPhan,
                        TenPhong = hocPhan.TenPhong,
                        Thu = thuChars[i].ToString(),
                        Tiethoc = tietParts[i],
                        NhomHP = nhomhp,
                    });
                }
            }
            ViewBag.MaHP = hocPhan.MaHocPhan;
            ViewBag.Nhomhp = hocPhan.NhomHP;
            ViewBag.ManagedHocPhans = managedHocPhans;

            var sinhviens = db.sinhviens
                .Where(sv => sv.hocphan.Contains(nhomhp))
                .ToList();

            var viewModel = new ChitietHocPhanViewModel
            {
                HocPhanDetails = hocPhanDetails,
                SinhViens = sinhviens
            };

            return View(viewModel);
        }


    }
}