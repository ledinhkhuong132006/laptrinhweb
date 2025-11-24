using PayPal.Api;
using _24DH1110883_MyStore.Models;
using _24DH1110883_MyStore.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace _24DH1110883_MyStore.Controllers
{
    public class PayPalController : Controller
    {
        private MyStoreEntities2 db = new MyStoreEntities2();

        // Xử lý thanh toán
        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            // Lấy config từ Web.config
            APIContext apiContext = new APIContext(new OAuthTokenCredential(
                ConfigManager.Instance.GetProperties()["clientId"],
                ConfigManager.Instance.GetProperties()["clientSecret"]
            ).GetAccessToken());

            try
            {
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    // --- GIAI ĐOẠN 1: TẠO THANH TOÁN GỬI LÊN PAYPAL ---

                    // 1. Tạo đường dẫn để PayPal trả về sau khi khách bấm thanh toán
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/PayPal/PaymentWithPaypal?";
                    var guid = Convert.ToString((new Random()).Next(100000));

                    // 2. Tạo đơn hàng gửi lên PayPal
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);

                    // 3. Lấy link chuyển hướng
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    // Lưu ID vào session
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // --- GIAI ĐOẠN 2: KHÁCH ĐÃ THANH TOÁN THÀNH CÔNG ---
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                    // Nếu thanh toán không thành công
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }

                    // === LƯU ĐƠN HÀNG VÀO DATABASE (Code giống hệt OrderController) ===

                    // Lấy giỏ hàng
                    var cart = Session["Cart"] as List<CartItem>;

                    // Lấy user
                    var user = db.Customers.FirstOrDefault(u => u.Username == User.Identity.Name);

                    // Tạo đơn hàng mới
                    _24DH1110883_MyStore.Models.Order donHang = new _24DH1110883_MyStore.Models.Order();
                    donHang.CustomerID = user.CustomerID;
                    donHang.OrderDate = DateTime.Now;
                    donHang.TotalAmount = cart.Sum(x => x.Quantity * x.UnitPrice);
                    donHang.PaymentMethod = "Paypal";
                    donHang.PaymentStatus = "Đã thanh toán qua PayPal"; // Khác biệt duy nhất ở đây
                    donHang.DeliveryMethod = "Giao hàng nhanh";
                    donHang.AddressDelivery = user.CustomerAddress;

                    // Lưu chi tiết
                    foreach (var item in cart)
                    {
                        OrderDetail ct = new OrderDetail();
                        ct.ProductID = item.ProductID;
                        ct.Quantity = item.Quantity;
                        ct.UnitPrice = item.UnitPrice;
                       
                        donHang.OrderDetails.Add(ct);
                    }

                    db.Orders.Add(donHang);
                    db.SaveChanges();

                    // Xóa giỏ hàng
                    Session["Cart"] = null;

                    // Chuyển hướng sang trang cảm ơn bên OrderController
                    return RedirectToAction("OrderSuccess", "Order", new { id = donHang.OrderID });
                }
            }
            catch (Exception )
            {
                return View("FailureView");
            }
        }

        // HÀM PHỤ: Tạo thông tin gửi lên PayPal
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            var cart = Session["Cart"] as List<CartItem>;
            var itemList = new ItemList() { items = new List<Item>() };

            // Chuyển đổi giỏ hàng của mình sang giỏ hàng PayPal
            foreach (var item in cart)
            {
                itemList.items.Add(new Item()
                {
                    name = item.ProductName,
                    currency = "USD",
                    price = item.UnitPrice.ToString(), // Lưu ý: PayPal test thường dùng USD
                    quantity = item.Quantity.ToString(),
                    sku = item.ProductID.ToString()
                });
            }

            var payer = new Payer() { payment_method = "paypal" };

            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };

            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = cart.Sum(x => x.Quantity * x.UnitPrice).ToString()
            };

            var amount = new Amount()
            {
                currency = "USD",
                total = details.subtotal,
                details = details
            };

            var transactionList = new List<Transaction>();
            transactionList.Add(new Transaction()
            {
                description = "Thanh toan don hang",
                invoice_number = Convert.ToString((new Random()).Next(100000)),
                amount = amount,
                item_list = itemList
            });

            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            return this.payment.Create(apiContext);
        }

        // HÀM PHỤ: Thực thi thanh toán
        private Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            this.payment = new Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }

        // View báo lỗi (tạo file FailureView.cshtml trong Views/PayPal hoặc Shared)
        public ActionResult FailureView()
        {
            return Content("Thanh toán thất bại!");
        }
    }
}