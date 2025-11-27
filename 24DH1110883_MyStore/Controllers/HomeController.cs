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

            // Tìm kiếm sản phẩm dựa trên từ khóa
            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchTerm = searchTerm;
                products = products.Where(p => p.ProductName.Contains(searchTerm) ||
                                               p.ProductDescription.Contains(searchTerm) ||
                                               p.Category.CategoryName.Contains(searchTerm));
            }

            // Đoạn code liên quan tới phân trang
            int pageNumber = page ?? 1;
            int pageSize = 6; // Số sản phẩm mỗi trang

            // Lấy top 10 sản phẩm bán chạy nhất cho phần Featured
            model.FeaturedProducts = products.OrderByDescending(p => p.OrderDetails.Count()).Take(10).ToList();

            // Lấy danh sách sản phẩm mới nhất (hoặc tất cả) và phân trang
            model.NewProducts = products.OrderByDescending(p => p.ProductID).ToPagedList(pageNumber, pageSize);

            return View(model);
        }

        // GET: Home/ProductDetails/5
        // Thêm tham số quantity và page để xử lý logic tính tiền và phân trang sản phẩm liên quan
        public ActionResult ProductDetails(int? id, int? quantity, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // 1. Tìm sản phẩm theo ID
            var product = db.Products.Include(p => p.Category).FirstOrDefault(x => x.ProductID == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            // 2. Khởi tạo ViewModel (Bắt buộc phải dùng VM để khớp với View)
            var model = new ProductDetailsVM();
            model.product = product;
            model.quantity = quantity ?? 1; // Mặc định là 1 nếu chưa nhập
            model.estimatedValue = model.quantity * product.ProductPrice; // Tính tạm tính

            // 3. Chuẩn bị dữ liệu cho các Partial View (TopProduct và RelatedProduct)
            int pageNumber = page ?? 1;
            int pageSize = model.PageSize; // Lấy số trang mặc định từ VM (thường là 3 hoặc 4)

            // Lấy các sản phẩm liên quan (cùng danh mục, trừ chính nó)
            model.RelatedProducts = db.Products
                .Where(x => x.CategoryID == product.CategoryID && x.ProductID != product.ProductID)
                .OrderBy(x => x.ProductID)
                .ToPagedList(pageNumber, pageSize);

            // Lấy Top sản phẩm bán chạy (để hiển thị bên phải)
            model.TopProducts = db.Products
                .OrderByDescending(x => x.OrderDetails.Count())
                .Take(10)
                .ToPagedList(pageNumber, pageSize);

            // 4. Trả về ViewModel (Không được trả về 'product' trần)
            return View(model);
        }
    }
}