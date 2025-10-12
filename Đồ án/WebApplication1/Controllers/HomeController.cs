using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace tranggiaohang.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult trangchu()
        {
            return View();
        }
        public ActionResult trangdangnhap()
        {
            return View();
        }
        public ActionResult tranghome()
        {
            return View();
        }
        public ActionResult trangsanpham1()
        {
            return View();
        }
        public ActionResult trangsanpham2()
        {
            return View();
        }
        public ActionResult tranggiohang()
        {
            return View();
        }
        public ActionResult tranggiao()
        {
            return View();
        }
        public ActionResult trangthanhtoan()
        {
            return View();
        }
        public ActionResult trangxacnhan()
        {
            return View();
        }
        public ActionResult tranglichsu()
        {
            return View();
        }
    }
}