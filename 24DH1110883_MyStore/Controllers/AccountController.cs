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

                // Lưu session và tự động đăng nhập
                Session["Username"] = customer.Username;
                Session["UserRole"] = customer.UserRole;
                Session["CustomerID"] = customer.CustomerID;

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

        // Hiển thị thông tin profile của Customer (dựa vào Session["CustomerID"] hoặc fallback theo Username)
        [Authorize]
        public ActionResult ProfileInfo()
        {
            // ưu tiên lấy CustomerID từ session (được lưu khi login/register)
            if (Session["CustomerID"] != null)
            {
                int customerId;
                if (int.TryParse(Session["CustomerID"].ToString(), out customerId))
                {
                    var customer = db.Customers.Find(customerId);
                    if (customer != null)
                        return View(customer);
                }
            }

            // fallback: nếu session CustomerID bị mất nhưng vẫn còn Username trong session => tìm theo Username
            if (Session["Username"] != null)
            {
                string username = Session["Username"].ToString();
                var customer = db.Customers.FirstOrDefault(c => c.Username == username);
                if (customer != null)
                {
                    // đảm bảo session CustomerID được thiết lập lại
                    Session["CustomerID"] = customer.CustomerID;
                    return View(customer);
                }
            }

            // Nếu không tìm được customer => chuyển tới trang đăng nhập hoặc tạo profile
            return RedirectToAction("Login", "Account");
        }
    }
}