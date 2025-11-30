using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _24DH1110883_MyStore.Models;
using _24DH1110883_MyStore.Models.ViewModel;
using System.Data.Entity; // Thêm thư viện này để dùng .Include

namespace _24DH1110883_MyStore.Controllers
{
    public class OrderController : Controller
    {
        private MyStoreEntities2 db = new MyStoreEntities2();

        // GET: Order/Checkout
        [Authorize]
        public ActionResult Checkout()
        {
            // 1. Lấy giỏ hàng từ Session
            var cart = Session["Cart"] as List<CartItem>;

            // 2. Nếu giỏ trống thì về trang chủ 
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            // 3. Lấy thông tin khách hàng
            var user = db.Customers.SingleOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            
            var model = new CheckoutVM
            {
                CartItems = cart,
                TotalAmount = cart.Sum(item => item.TotalPrice),
                OrderDate = DateTime.Now,
                ShippingAddress = user.CustomerAddress,
                CustomerID = user.CustomerID,
                Username = user.Username
            };

            return View(model);
        }
        // POST: Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Checkout(CheckoutVM model)
        {
            // 1. Lấy giỏ hàng từ Session
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            // 2.  thông tin khách hàng
            var user = db.Customers.SingleOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }



            // 4. Nếu thanh toán bình thường thì lưu đơn hàng
            if (ModelState.IsValid)
            {
                // Tạo đơn hàng mới
                Order order = new Order();
                order.CustomerID = user.CustomerID;
                order.OrderDate = DateTime.Now;
                order.AddressDelivery = model.ShippingAddress;
                order.PaymentMethod = model.PaymentMethod;
                order.DeliveryMethod = model.ShippingMethod;
                order.PaymentStatus = "Đã thanh toán";
                order.OrderStatus = 0; // chờ xử lí
                order.TotalAmount = cart.Sum(item => item.TotalPrice);

                //  trạng thái thanh toán
                if (model.PaymentMethod == "Tiền mặt") order.PaymentStatus = "Thanh toán tiền mặt";
                if (model.PaymentMethod == "Mua trước trả sau") order.PaymentStatus = "Trả góp";

                // Lưu chi tiết đơn hàng
                foreach (var item in cart)
                {
                    OrderDetail detail = new OrderDetail();
                    detail.ProductID = item.ProductID;
                    detail.Quantity = item.Quantity;
                    detail.UnitPrice = item.UnitPrice;


                    order.OrderDetails.Add(detail);
                }

                db.Orders.Add(order);
                db.SaveChanges();

                // làm trống giỏ hàng khi đặt hàng xong 
                Session["Cart"] = null;

                
                return RedirectToAction("OrderSuccess", new { id = order.OrderID });
            }

            return View(model);
        }

        // GET: Order/OrderSuccess
        public ActionResult OrderSuccess(int id)
        {
            var order = db.Orders
                          .Include("Customer")
                          .Include("OrderDetails")
                          .Include("OrderDetails.Product")
                          .SingleOrDefault(o => o.OrderID == id);

            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }
        // GET: Order/History (Xem danh sách đơn hàng đã mua)
        [Authorize]
        public ActionResult History()
        {
            // 1. Lấy thông tin khách hàng 
            var user = db.Customers.SingleOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. Lấy danh sách đơn hàng của khách hàng đó
            var orders = db.Orders.Where(o => o.CustomerID == user.CustomerID)
                                  .OrderByDescending(o => o.OrderDate)
                                  .ToList();

            return View(orders);
        }

        // GET: Order/Details/5 (Xem chi tiết 1 đơn hàng cụ thể)
        [Authorize]
        public ActionResult Details(int id)
        {
           
            var user = db.Customers.SingleOrDefault(u => u.Username == User.Identity.Name);

            var order = db.Orders.Include("OrderDetails")
                                 .Include("OrderDetails.Product") 
                                 .FirstOrDefault(o => o.OrderID == id && o.CustomerID == user.CustomerID);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }
    }
}