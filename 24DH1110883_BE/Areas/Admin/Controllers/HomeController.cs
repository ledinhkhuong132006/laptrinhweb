using _24DH1110883_MyStore.ViewModels;
using _24DH1110883_MyStore.Models;
using _24DH1110883_MyStore.Models.ViewModel;
using Antlr.Runtime;

using PagedList ;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;


namespace _24DH1110883_MyStore.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private MyStoreEntities2 db = new MyStoreEntities2();
        // GET: Admin/Home
        public ActionResult Index()
        {

           

            var dataThongKe = db.OrderDetails
                .GroupBy(od => od.Product) // Nhóm theo Product (hoặc od.ProductID)
                .Select(g => new ProductStatisticVM
                {
                    TenSanPham = g.Key.ProductName, // Lấy tên từ Product
                    SoLuongDaBan = g.Sum(x => x.Quantity), // Cộng dồn số lượng bán
                    TongDoanhThu = g.Sum(x => x.Quantity * x.UnitPrice), // Cộng dồn tiền
                    HinhAnh = g.Key.ProductImage // Lấy ảnh
                })
                .OrderByDescending(x => x.SoLuongDaBan) // Sắp xếp giảm dần (bán chạy nhất lên đầu)
                .Take(5) // Chỉ lấy top 5 sản phẩm bán chạy nhất
                .ToList();

            return View(dataThongKe);
        }

        // GET: Admin/Home/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        // GET: Home/Product/Details/5
        public ActionResult ProductDetails(int? id, int? quantity, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Eager-load Category and OrderDetails so properties are available in the view
            var pro = db.Products
                        .Include(p => p.Category)
                        .Include(p => p.OrderDetails)
                        .SingleOrDefault(p => p.ProductID == id);
    
            if (pro == null)
            {
                return HttpNotFound();
            }

            // lấy tất cả các sản phẩm cùng danh mục
            var products = db.Products.Where(p => p.CategoryID == pro.CategoryID && p.ProductID != pro.ProductID).AsQueryable();

            ProductDetailsVM model = new ProductDetailsVM();

            // Đoạn code liên quan tới phân trang
            // Lấy số trang hiện tại (mặc định là trang 1 nếu không có giá trị)
            int pageNumber = page ?? 1;
            int pageSize = model.PageSize; // Số sản phẩm mỗi trang
            model.product = pro;
            model.RelatedProducts = products.OrderBy(p => p.ProductID).Take(8).ToPagedList(pageNumber, pageSize);
            model.TopProducts = products.OrderByDescending(p => p.OrderDetails.Count()).Take(8).ToPagedList(pageNumber, pageSize);

            if (quantity.HasValue)
            {
                model.quantity = quantity.Value;
            }

            return View(model);
        }


        // GET: Admin/Home/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Admin/Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Thêm tham số HttpPostedFileBase ImageFile để nhận file
        public ActionResult Create(Product product, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Xử lý lưu ảnh
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    // 1. Lấy tên file
                    string fileName = System.IO.Path.GetFileName(ImageFile.FileName);

                    // 2. Tạo đường dẫn lưu file (Server.MapPath trỏ tới thư mục thật trên server)
                    string uploadPath = Server.MapPath("~/images/products/");

                    // Kiểm tra xem thư mục có tồn tại không, nếu không thì tạo mới
                    if (!System.IO.Directory.Exists(uploadPath))
                    {
                        System.IO.Directory.CreateDirectory(uploadPath);
                    }

                    // 3. Nối tên file vào đường dẫn
                    string path = System.IO.Path.Combine(uploadPath, fileName);

                    // 4. Lưu file lên server
                    ImageFile.SaveAs(path);

                    // 5. Gán tên file vào đối tượng Product để lưu vào DB
                    product.ProductImage = fileName;
                }
                else
                {
                    // Gán ảnh mặc định nếu không upload
                    product.ProductImage = "no-image.png";
                }

                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Home/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Admin/Home/Edit/5
       
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,CategoryID,ProductName,ProductPrice,ProductImage,ProductDescription")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Home/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}