using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using _24DH1110883_MyStore.Models;

namespace _24DH1110883_MyStore.Controllers
{
    [Authorize] // Chỉ user đã đăng nhập mới thao tác được (bỏ hoặc điều chỉnh nếu cần)
    public class CustomerController : Controller
    {
        private MyStoreEntities2 db = new MyStoreEntities2();

        // GET: Customer
        // Có thể cho phép xem danh sách cho admin bằng role check nếu cần
        public ActionResult Index()
        {
            var customers = db.Customers.ToList();
            return View(customers);
        }

        // GET: Customer/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Customer customer = db.Customers.Find(id);
            if (customer == null) return HttpNotFound();
            return View(customer);
        }

        // GET: Customer/Create
        // Dùng để tạo hồ sơ nếu AccountController.ProfileInfo redirect tới đây
        [AllowAnonymous] // cho phép người chưa có session vào tạo (Account.Register cũng tạo Customer)
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Create([Bind(Include = "CustomerName,CustomerPhone,CustomerEmail,CustomerAddress,Username,Password")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                // Nếu tạo từ trang đăng ký, có thể đã có username, kiểm tra trùng
                var exists = db.Customers.Any(c => c.Username == customer.Username);
                if (exists)
                {
                    ModelState.AddModelError("Username", "Username đã tồn tại.");
                    return View(customer);
                }

                // LƯU Ý: Nên hash password; hiện giữ như project hiện tại cho tương thích
                db.Customers.Add(customer);
                db.SaveChanges();

                // Nếu đã đăng nhập, chuyển về profile; nếu chưa, redirect về login
                if (Session["Username"] != null || User.Identity.IsAuthenticated)
                    return RedirectToAction("ProfileInfo", "Account");
                return RedirectToAction("Login", "Account");
            }

            return View(customer);
        }

        // GET: Customer/Edit/5
        public ActionResult Edit(int? id)
        {
            // Nếu không truyền id, dùng session CustomerID để chỉnh chính mình
            if (id == null)
            {
                if (Session["CustomerID"] != null)
                {
                    id = Convert.ToInt32(Session["CustomerID"]);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }

            Customer customer = db.Customers.Find(id);
            if (customer == null) return HttpNotFound();

            // Chỉ cho phép user chỉnh hồ sơ của chính họ (hoặc admin)
            var usernameInSession = Session["Username"]?.ToString();
            if (!User.IsInRole("Admin") && usernameInSession != null && usernameInSession != customer.Username && !User.IsInRole("Admin"))
            {
                // không cho phép
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(customer);
        }

        // POST: Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerID,CustomerName,CustomerPhone,CustomerEmail,CustomerAddress,Username")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                // Không cho phép sửa password ở đây (hoặc xử lý riêng)
                var dbCustomer = db.Customers.Find(customer.CustomerID);
                if (dbCustomer == null) return HttpNotFound();

                dbCustomer.CustomerName = customer.CustomerName;
                dbCustomer.CustomerPhone = customer.CustomerPhone;
                dbCustomer.CustomerEmail = customer.CustomerEmail;
                dbCustomer.CustomerAddress = customer.CustomerAddress;
                // Nếu muốn cho đổi username, kiểm tra trùng trước khi gán
                // dbCustomer.Username = customer.Username;

                db.Entry(dbCustomer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ProfileInfo", "Account");
            }
            return View(customer);
        }

        // GET: Customer/Delete/5
        [Authorize(Roles = "Admin")] // chỉ admin mới xóa user (tuỳ project)
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Customer customer = db.Customers.Find(id);
            if (customer == null) return HttpNotFound();
            return View(customer);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}