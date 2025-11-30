
using _24DH1110883_MyStore.Models;
using _24DH1110883_MyStore.Models.ViewModel;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace _24DH1110883_MyStore.Controllers
{
    public class AccountController : Controller
    {
        private MyStoreEntities2 db = new MyStoreEntities2();

        // lay khach hang hien tai  =====
        private Customer GetCurrentCustomer()
        {
            // Ưu tiên khách hàng CustomerID 
            if (Session["CustomerID"] != null && int.TryParse(Session["CustomerID"].ToString(), out int customerId))
            {
                var customerById = db.Customers.Find(customerId);
                if (customerById != null) return customerById;
            }

            // Fallback theo Username
            if (Session["Username"] != null)
            {
                string username = Session["Username"].ToString();
                var customerByUsername = db.Customers.FirstOrDefault(c => c.Username == username);
                if (customerByUsername != null)
                {
                    Session["CustomerID"] = customerByUsername.CustomerID; // sync lại Session
                    return customerByUsername;
                }
            }

            return null;
        }

        //  Register
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = db.Customers.SingleOrDefault(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại!");
                    return View(model);
                }

                
                var customer = new Customer
                {
                    Username = model.Username,
                    Password = model.Password,
                    UserRole = "Customer",

                    CustomerName = model.CustomerName,
                    CustomerEmail = model.CustomerEmail,
                    CustomerPhone = model.CustomerPhone,
                    CustomerAddress = model.CustomerAddress
                };

                db.Customers.Add(customer);
                db.SaveChanges();

                // Đăng nhập và lưu khách hàng
                Session["Username"] = customer.Username;
                Session["UserRole"] = customer.UserRole;
                Session["CustomerID"] = customer.CustomerID;

                FormsAuthentication.SetAuthCookie(customer.Username, false);
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        //  Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Customers.SingleOrDefault(u =>
                    u.Username == model.Username &&
                    u.Password == model.Password &&
                    u.UserRole == "Customer");

                if (user != null)
                {
                    Session["Username"] = user.Username;
                    Session["UserRole"] = user.UserRole;
                    Session["CustomerID"] = user.CustomerID;

                    FormsAuthentication.SetAuthCookie(user.Username, false);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
            }

            return View(model);
        }

        //  Logout 
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        //  Xem thông tin profile 
        [Authorize]
        public ActionResult ProfileInfo()
        {
            var customer = GetCurrentCustomer();
            if (customer == null)
                return RedirectToAction("Login", "Account");

            return View(customer);
        }

        //  Sửa thông tin profile
        [Authorize]
        public ActionResult EditProfile()
        {
            var customer = GetCurrentCustomer();
            if (customer == null)
                return RedirectToAction("Login", "Account");

            var vm = new EditProfileVM
            {
                CustomerName = customer.CustomerName,
                CustomerEmail = customer.CustomerEmail,
                CustomerPhone = customer.CustomerPhone,
                CustomerAddress = customer.CustomerAddress
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(EditProfileVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var customer = GetCurrentCustomer();
            if (customer == null)
                return RedirectToAction("Login", "Account");

            // Cập nhật
            customer.CustomerName = model.CustomerName;
            customer.CustomerEmail = model.CustomerEmail;
            customer.CustomerPhone = model.CustomerPhone;
            customer.CustomerAddress = model.CustomerAddress;

            db.SaveChanges();

            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("ProfileInfo");
        }

        // Đổi mật khẩu 
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View(new ChangePasswordVM());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var customer = GetCurrentCustomer();
            if (customer == null)
                return RedirectToAction("Login", "Account");

            // Kiểm tra mật khẩu hiện tại
            if (customer.Password != model.CurrentPassword)
            {
                ModelState.AddModelError("CurrentPassword", "Mật khẩu hiện tại không đúng.");
                return View(model);
            }

            if (model.NewPassword != model.ConfirmNewPassword)
            {
                ModelState.AddModelError("ConfirmNewPassword", "Mật khẩu xác nhận không khớp.");
                return View(model);
            }

           
            customer.Password = model.NewPassword;
            db.SaveChanges();

            TempData["Success"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("ProfileInfo");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
