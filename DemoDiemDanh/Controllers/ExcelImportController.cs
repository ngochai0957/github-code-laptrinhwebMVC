using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using OfficeOpenXml;
using DemoDiemDanh.Model;
using System.Globalization;
using PagedList;
using System.Threading.Tasks;

namespace DemoDiemDanh.Controllers
{
    public class ExcelImportController : Controller
    {
        public ActionResult Index()
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

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault(); // Lấy sheet đầu tiên

                        if (worksheet != null)
                        {
                            int rowCount = worksheet.Dimension.Rows;

                            using (var context = new Model1()) // Thay YourDbContext bằng context của bạn
                            {
                                for (int row = 2; row <= rowCount; row++) // Bắt đầu từ dòng 2 (dòng tiêu đề thường là dòng 1)
                                {
                                    string Masv = worksheet.Cells[row, 1].Value?.ToString().Trim();
                                    string Hoten = worksheet.Cells[row, 2].Value?.ToString().Trim();
                                    string Ngaysinh = worksheet.Cells[row, 3].Value?.ToString().Trim();
                                    string Gioitinh = worksheet.Cells[row, 4].Value?.ToString().Trim();
                                    string Lop = worksheet.Cells[row, 5].Value?.ToString().Trim();
                                    if (!string.IsNullOrEmpty(Ngaysinh))
                                    {
                                        Ngaysinh = Ngaysinh.Replace("-", "/"); // Chuyển đổi thành "03/12/2003"

                                        // Kiểm tra và định dạng lại thành DateTime nếu cần thiết
                                        DateTime parsedDate;
                                        if (DateTime.TryParseExact(Ngaysinh, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                                        {
                                            Ngaysinh = parsedDate.ToString("dd/MM/yyyy");
                                        }
                                    }
                                    // Thêm vào CSDL
                                    sinhvien newEntity = new sinhvien
                                    {
                                        masv = Masv,
                                        tensv = Hoten,
                                        ngaysinh = Ngaysinh,
                                        gioitinh = Gioitinh,
                                        lop = Lop,
                                        // Các thuộc tính khác
                                    };
                                    context.sinhviens.Add(newEntity);
                                }

                                context.SaveChanges();
                            }
                        }
                        else
                        {
                            TempData["Error"] = "Không tìm thấy sheet trong file Excel.";
                        }
                    }

                    TempData["Message"] = "Import thành công!";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Lỗi: " + ex.Message;
                }
            }
            else
            {
                TempData["Error"] = "Vui lòng chọn file!";
            }

            return RedirectToAction("sinhvien", "QLSV");
        }

        [HttpPost]
        public ActionResult ChangeAvt(HttpPostedFileBase file)
        {
            // Lấy magv từ Session
            var magv = Session["magv"] as string;

            if (file != null && file.ContentLength > 0)
            {
                // Tạo tên tệp duy nhất và lưu trữ tệp
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Server.MapPath("~/Image"), fileName);

                try
                {
                    // Lưu tệp vào thư mục
                    file.SaveAs(filePath);
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi nếu có
                    TempData["Error"] = "Lỗi khi lưu tệp: " + ex.Message;
                    return RedirectToAction("giangvien", "QLGV");
                }

                // Lấy giảng viên theo id
                using (var context = new Model1())
                {
                    var giangVien = context.giangviens.FirstOrDefault(gv => gv.magv == magv);
                    if (giangVien != null)
                    {
                        giangVien.anhgv = "/Image/" + fileName;
                        context.Entry(giangVien).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }
                }
            }
            return RedirectToAction("giangvien", "QLGV");
        }

    }
}
