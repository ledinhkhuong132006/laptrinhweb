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
        private List<CartItem> GetCart()
        {
            List<CartItem> cart = Session["Cart"] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                Session["Cart"] = cart;
            }
            return cart;
        }

        // Action 1: Thêm sản phẩm vào giỏ
        public ActionResult AddToCart(int id)
        {
            // Lấy giỏ hàng hiện tại
            var cart = GetCart();

            // Tìm xem sản phẩm này đã có trong giỏ chưa
            var item = cart.FirstOrDefault(s => s.ProductID == id);

            if (item != null)
            {
                // Nếu có rồi thì tăng số lượng lên 1
                item.Quantity++;
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
                        UnitPrice = p.ProductPrice,
                        Quantity = 1
                    };
                    cart.Add(item);
                }
            }

            // Lưu lại vào Session
            Session["Cart"] = cart;

            // Chuyển hướng đến trang Giỏ hàng để khách xem
            return RedirectToAction("Index");
        }

        // Action 2: Xem giỏ hàng
        public ActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        // Action 3: Xóa sản phẩm khỏi giỏ
        public ActionResult RemoveFromCart(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(s => s.ProductID == id);
            if (item != null)
            {
                cart.Remove(item);
            }
            Session["Cart"] = cart;
            return RedirectToAction("Index");
        }

        // Action 4: Cập nhật số lượng (dùng cho trang giỏ hàng)
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = GetCart();
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