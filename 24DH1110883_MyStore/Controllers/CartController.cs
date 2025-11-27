using _24DH1110883_MyStore.Models;
using _24DH1110883_MyStore.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _24DH1110883_MyStore.Controllers
{
    public class CartController : Controller
    {
        private MyStoreEntities2 db = new MyStoreEntities2();

        // Hàm lấy giỏ hàng từ Session
        private List<CartItem> GetCartService()
        {
            List<CartItem> cart = Session["Cart"] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                Session["Cart"] = cart;
            }
            return cart;
        }

        // Action 1: Xem giỏ hàng
        public ActionResult Index()
        {
            var cart = GetCartService();
            // Tính tổng tiền cho đẹp (truyền qua ViewBag)
            ViewBag.TotalAmount = cart.Sum(item => item.TotalPrice);
            return View(cart);
        }

        // Action 2: Thêm sản phẩm vào giỏ (ĐÃ SỬA NÂNG CẤP)
       // Quan trọng: Phải là POST mới nhận được dữ liệu từ Form
        public ActionResult AddToCart(int id, int? quantity, string type)
        {
            // Lấy giỏ hàng hiện tại
            var cart = GetCartService();

            // Xử lý số lượng: Nếu không truyền thì mặc định là 1
            int sl = quantity ?? 1;

            // Tìm xem sản phẩm này đã có trong giỏ chưa
            var item = cart.FirstOrDefault(s => s.ProductID == id);

            if (item != null)
            {
                // Nếu có rồi thì cộng dồn số lượng khách chọn (thay vì chỉ +1)
                item.Quantity += sl;
            }
            else
            {
                // Nếu chưa có thì lấy thông tin từ DB và thêm mới vào giỏ
                var p = db.Products.Find(id);
                if (p != null)
                {
                    item = new CartItem
                    {
                        ProductID = p.ProductID,
                        ProductName = p.ProductName,
                        ProductImage = p.ProductImage,
                        // Thêm ?? 0 để tránh lỗi nếu giá bị null
                        UnitPrice = p.ProductPrice, 
                        Quantity = sl
                    };
                    cart.Add(item);
                }
            }

            // Lưu lại vào Session
            Session["Cart"] = cart;

            // --- XỬ LÝ CHUYỂN HƯỚNG ---
            
            // Nếu khách bấm nút "Mua ngay"
            if (type == "buynow")
            {
                return RedirectToAction("Checkout", "Order"); // Chuyển sang trang Thanh toán
            }

            // Nếu bấm "Thêm vào giỏ" bình thường -> Quay lại trang giỏ hàng
            return RedirectToAction("Index");
        }

        // Action 3: Xóa sản phẩm khỏi giỏ
        public ActionResult RemoveFromCart(int id)
        {
            var cart = GetCartService();
            var item = cart.FirstOrDefault(s => s.ProductID == id);
            if (item != null)
            {
                cart.Remove(item);
            }
            Session["Cart"] = cart;
            return RedirectToAction("Index");
        }

        // Action 4: Cập nhật số lượng
        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = GetCartService();
            var item = cart.FirstOrDefault(s => s.ProductID == id);
            if (item != null)
            {
                item.Quantity = quantity;
            }
            Session["Cart"] = cart;
            return RedirectToAction("Index");
        }
    }
}