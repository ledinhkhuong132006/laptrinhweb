using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using _24DH1110883_MyStore.Models;

namespace _24DH1110883_MyStore.Controllers
{
    [Authorize] // Chỉ user đã đăng nhập mới thao tác được (Create vẫn có [AllowAnonymous])
    public class CustomerController : Controller
    {
        private MyStoreEntities2 db = new MyStoreEntities2();

        // GET: Customer
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
        [AllowAnonymous]
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
                var exists = !string.IsNullOrEmpty(customer.Username) && db.Customers.Any(c => c.Username == customer.Username);
                if (exists)
                {
                    ModelState.AddModelError("Username", "Username đã tồn tại.");
                    return View(customer);
                }

                db.Customers.Add(customer);
                db.SaveChanges();

                // Cập nhật session nếu có đăng nhập
                Session["CustomerID"] = customer.CustomerID;
                if (!string.IsNullOrEmpty(customer.Username))
                    Session["Username"] = customer.Username;

                if (Session["Username"] != null || User.Identity.IsAuthenticated)
                    return RedirectToAction("ProfileInfo", "Account");
                return RedirectToAction("Login", "Account");
            }

            return View(customer);
        }

        // GET: Customer/Edit/5
        public ActionResult Edit(int? id)
        {
            // Lấy id từ route hoặc session
            if (id == null)
            {
                if (Session["CustomerID"] != null && int.TryParse(Session["CustomerID"].ToString(), out int sid))
                    id = sid;
            }

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Customer customer = db.Customers.Find(id);
            if (customer == null) return HttpNotFound();

            // Kiểm tra quyền: chỉ owner (Session CustomerID) hoặc Admin mới được sửa
            bool isAdmin = (Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin");
            if (!isAdmin)
            {
                if (Session["CustomerID"] == null || !int.TryParse(Session["CustomerID"].ToString(), out int sessionId) || sessionId != customer.CustomerID)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }

            return View(customer);
        }

        // POST: Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerID,CustomerName,CustomerPhone,CustomerEmail,CustomerAddress,Username")] Customer customer)
        {
            // Kiểm tra quyền trước khi cập nhật
            bool isAdmin = (Session["UserRole"] != null && Session["UserRole"].ToString() == "Admin");
            if (!isAdmin)
            {
                if (Session["CustomerID"] == null || !int.TryParse(Session["CustomerID"].ToString(), out int sessionId) || sessionId != customer.CustomerID)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }

            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            var dbCustomer = db.Customers.Find(customer.CustomerID);
            if (dbCustomer == null) return HttpNotFound();

            // Nếu username thay đổi, kiểm tra trùng (nếu bạn cho phép đổi username)
            if (!string.Equals(dbCustomer.Username, customer.Username, StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(customer.Username) && db.Customers.Any(c => c.Username == customer.Username && c.CustomerID != customer.CustomerID))
                {
                    ModelState.AddModelError("Username", "Username đã tồn tại, vui lòng chọn tên khác.");
                    return View(customer);
                }
                dbCustomer.Username = customer.Username;
                Session["Username"] = dbCustomer.Username;
            }

            // Cập nhật các trường cho phép
            dbCustomer.CustomerName = customer.CustomerName;
            dbCustomer.CustomerPhone = customer.CustomerPhone;
            dbCustomer.CustomerEmail = customer.CustomerEmail;
            dbCustomer.CustomerAddress = customer.CustomerAddress;

            db.Entry(dbCustomer).State = EntityState.Modified;
            db.SaveChanges();

            // Cập nhật lại session CustomerID (dù thông thường không đổi)
            Session["CustomerID"] = dbCustomer.CustomerID;

            return RedirectToAction("ProfileInfo", "Account");
        }

        // GET: Customer/Delete/5
        [Authorize(Roles = "Admin")]
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