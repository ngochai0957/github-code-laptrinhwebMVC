using DemoDiemDanh.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoDiemDanh.Controllers
{
    public class TrangChuController : Controller
    {
        // GET: TrangChu
        public ActionResult trangchu()
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
    }
}