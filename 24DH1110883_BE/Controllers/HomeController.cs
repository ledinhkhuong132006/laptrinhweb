using _24DH1110883_MyStore.Models;
using _24DH1110883_MyStore.Models.ViewModel;
using PagedList;
using PagedList.Mvc;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity; // Quan trọng để dùng .Include()

namespace _24DH1110883_MyStore.Controllers
{
    public class HomeController : Controller
    {
        private MyStoreEntities2 db = new MyStoreEntities2();

        // GET: Home/Index
        public ActionResult Index(string searchTerm, int? page)
        {
            var model = new HomeProductVM();
            var products = db.Products.AsQueryable();

            //  tìm sản phẩm 
            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchTerm = searchTerm;
                products = products.Where(p => p.ProductName.Contains(searchTerm) ||
                                               p.ProductDescription.Contains(searchTerm) ||
                                               p.Category.CategoryName.Contains(searchTerm));
            }

            // phân trang 
            int pageNumber = page ?? 1;
            int pageSize = 6; // sản phẩm mỗi trang 

            // top 10 sản phảm bán chạy
            model.FeaturedProducts = products.OrderByDescending(p => p.OrderDetails.Count()).Take(10).ToList();

          
            model.NewProducts = products.OrderByDescending(p => p.ProductID).ToPagedList(pageNumber, pageSize);

            return View(model);
        }

        // GET: Home/ProductDetails/5
      
        public ActionResult ProductDetails(int? id, int? quantity, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // .Tìm sản phẩm theo ID
            var product = db.Products.Include(p => p.Category).FirstOrDefault(x => x.ProductID == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            
            var model = new ProductDetailsVM();
            model.product = product;
            model.quantity = quantity ?? 1; 
            model.estimatedValue = model.quantity * product.ProductPrice; // tam tính

            // du lieu topproduct và related products
            int pageNumber = page ?? 1;
            int pageSize = model.PageSize; 

            // Lấy các sản phẩm liên quan (
            model.RelatedProducts = db.Products
                .Where(x => x.CategoryID == product.CategoryID && x.ProductID != product.ProductID)
                .OrderBy(x => x.ProductID)
                .ToPagedList(pageNumber, pageSize);

            // top sản phẩm
            model.TopProducts = db.Products
                .OrderByDescending(x => x.OrderDetails.Count())
                .Take(10)
                .ToPagedList(pageNumber, pageSize);

    
            return View(model);
        }
    }
}