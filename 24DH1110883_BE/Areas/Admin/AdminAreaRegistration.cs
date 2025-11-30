using System.Web.Mvc;

namespace _24DH1110883_MyStore.Areas.Admin.Controllers
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                // Thêm controller = "Home" vào dòng dưới đây
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "_24DH1110883_MyStore.Areas.Admin.Controllers" }
            );
        }
    }
}