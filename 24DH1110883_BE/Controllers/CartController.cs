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

        // Hàm lấy giỏ hàng từ session
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

        // Xem giỏ hàng
        public ActionResult Index()
        {
            var cart = GetCartService();
            // Tính tổng tiền 
            ViewBag.TotalAmount = cart.Sum(item => item.TotalPrice);
            return View(cart);
        }

        
       
        
        // Action 2: Thêm sản phẩm 
        [Authorize]
        public ActionResult AddToCart(int id, int? quantity, string type)
        {
            List<CartItem> cart;

            // Nếu mua ngay -> Tạo giỏ mới
            if (type == "buynow")
            {
                cart = new List<CartItem>();
            }
            else
            {
                // Nếu thêm thường -> Lấy giỏ cũ
                cart = Session["Cart"] as List<CartItem>;
                if (cart == null) cart = new List<CartItem>();
            }

            int sl = quantity ?? 1;
            var item = cart.FirstOrDefault(s => s.ProductID == id);

            if (item != null)
            {
                // Có rồi thì tăng số lượng
                item.Quantity += sl;

               
            }
            else
            {
                var p = db.Products.Find(id);
                if (p != null)
                {
                    item = new CartItem
                    {
                        ProductID = p.ProductID,
                        ProductName = p.ProductName,
                        ProductImage = p.ProductImage,
                        UnitPrice = p.ProductPrice,
                        Quantity = sl

                      
                    };
                    cart.Add(item);
                }
            }

            Session["Cart"] = cart;

            if (type == "buynow")
            {
                return RedirectToAction("Checkout", "Order");
            }

            return RedirectToAction("Index");
        }
        //  Xóa sản phẩm khỏi giỏ
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

        //  Cập nhật số lượng
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