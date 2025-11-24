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
            // 1. Kiểm tra giỏ hàng
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            // 2. Lấy thông tin khách hàng đang đăng nhập
            // (Sửa UseName thành User.Identity.Name cho chuẩn logic hệ thống)
            var user = db.Customers.SingleOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // 3. Tạo dữ liệu để hiển thị lên View
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

            // 2. Lấy thông tin khách hàng
            var user = db.Customers.SingleOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // 3. Nếu chọn thanh toán PayPal -> Chuyển hướng sang PayPalController
            if (model.PaymentMethod == "Paypal")
            {
                // Gọi sang controller PayPal mà bạn đã làm xong
                return RedirectToAction("PaymentWithPaypal", "PayPal", model);
            }

            // 4. Nếu thanh toán bình thường (COD hoặc Trả góp) -> Lưu luôn
            if (ModelState.IsValid)
            {
                // Tạo đơn hàng mới
                Order order = new Order();
                order.CustomerID = user.CustomerID;
                order.OrderDate = DateTime.Now;
                order.AddressDelivery = model.ShippingAddress;
                order.PaymentMethod = model.PaymentMethod;
                order.DeliveryMethod = model.ShippingMethod;
                order.PaymentStatus = "Chưa thanh toán";
                order.TotalAmount = cart.Sum(item => item.TotalPrice);

                // Cập nhật trạng thái thanh toán
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

                // Xóa giỏ hàng sau khi đặt thành công
                Session["Cart"] = null;

                // Chuyển hướng đến trang thông báo thành công
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
    }
}