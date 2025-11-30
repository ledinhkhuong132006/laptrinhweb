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
        public ActionResult trangdienthoai()
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
        public ActionResult trangsanpham3()
        {
            return View();
        }
        public ActionResult trangsanpham4()
        {
            return View();
        }
        public ActionResult trangsanpham5()
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
       
        public ActionResult tranglaptop()
        {
            return View();
        }
        public ActionResult laptop1()
        {
            return View();
        }
        public ActionResult laptop2()
        {
            return View();
        }
        public ActionResult laptop3()
        {
            return View();
        }
        public ActionResult laptop4()
        {
            return View();
        }
        public ActionResult laptop5()
        {
            return View();
        }
        public ActionResult trangphukien()
        {
            return View();
        }
        public ActionResult phukien1()
        {
            return View();
        }
        public ActionResult phukien2()
        {
            return View();
        }
        public ActionResult phukien3()
        {
            return View();
        }
        public ActionResult phukien4()
        {
            return View();
        }
        public ActionResult phukien5()
        {
            return View();
        }
        public ActionResult tranglienhe()
        {
            return View();
        }
        public ActionResult thongtin()
        {
            return View();
        }
        public ActionResult trangsanpham()
        {
            return View();
        }

    }

}