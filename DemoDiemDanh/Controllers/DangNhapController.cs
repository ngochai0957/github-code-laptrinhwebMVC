using DemoDiemDanh.Model;
using System.Linq;
using System.Web.Mvc;

namespace DemoDiemDanh.Controllers
{
    public class DangNhapController : Controller
    {
        public ActionResult dangnhap(string email, string mk)
        {
            ViewBag.Error = "";
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(mk))
            {
                // If either field is empty, do not show any error message
                return View();
            }

            Model1 db = new Model1();
            var user = db.giangviens.FirstOrDefault(x => x.email == email);

            if (user == null)
            {
                ViewBag.Error = "Email không đúng!!!";
                return View();
            }

            if (user.matkhau != mk)
            {
                ViewBag.Error = "Mật khẩu không đúng!!!";
                return View();
            }

            // Login successful
            Session["magv"] = user.magv;
            return RedirectToAction("trangchu", "TrangChu");
        }
    }
}