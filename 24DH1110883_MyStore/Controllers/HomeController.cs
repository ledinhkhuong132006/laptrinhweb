using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _24DH1110883_MyStore.Models;
using _24DH1110883_MyStore.Models.ViewModel;
using PagedList;
using System.Web.UI;

namespace _24DH1110883_MyStore.Controllers
{

    public class HomeController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();
        // get
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
    }
}