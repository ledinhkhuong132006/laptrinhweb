using _24DH1110883_MyStore.Models;
using _24DH1110883_MyStore.Models.ViewModel;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace _24DH1110883_MyStore.Controllers
{
    public class AccountController : Controller
    {
        private MyStoreEntities2 db = new MyStoreEntities2();

        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                // 1. Kiểm tra Username trong bảng Customer (thay vì Users)
                var existingUser = db.Customers.SingleOrDefault(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại!");
                    return View(model);
                }

                // 2. Tạo bản ghi Customer mới (bao gồm cả User và Info)
                // KHÔNG tạo var user = new User nữa
                var customer = new Customer
                {
                    // Thông tin đăng nhập
                    Username = model.Username,
                    Password = model.Password, // Lưu ý mã hóa nếu cần
                    UserRole = "Customer",

                    // Thông tin cá nhân
                    CustomerName = model.CustomerName,
                    CustomerEmail = model.CustomerEmail,
                    CustomerPhone = model.CustomerPhone,
                    CustomerAddress = model.CustomerAddress
                };

                // 3. Chỉ thêm vào bảng Customers
                db.Customers.Add(customer);

                // 4. Lưu database
                db.SaveChanges();

                // Tự động đăng nhập (dùng Username từ customer vừa tạo)
                FormsAuthentication.SetAuthCookie(customer.Username, false);
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }
        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                // Tìm trong bảng Customers
                var user = db.Customers.SingleOrDefault(u =>
                    u.Username == model.Username &&
                    u.Password == model.Password &&
                    u.UserRole == "Customer");

                if (user != null)
                {
                    // Lưu session
                    Session["Username"] = user.Username;
                    Session["UserRole"] = user.UserRole;

                    // Lưu cả CustomerID để sau này dùng đặt hàng cho tiện
                    Session["CustomerID"] = user.CustomerID;

                    FormsAuthentication.SetAuthCookie(user.Username, false);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
            }

            return View(model);
        }
        // GET: Account/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}