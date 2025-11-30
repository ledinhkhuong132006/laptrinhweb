using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _24DH1110883_MyStore.Models;

namespace _24DH1110883_MyStore.Areas.Admin.Controllers
{
    public class CategoriesController : Controller
    {
        private MyStoreEntities2 db = new MyStoreEntities2();

        // GET: Admin/Categories
        // Lay du lieu tu bang  categori de hien thi ten 
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        // GET: Admin/Categories/Details/5
        // Detail lay chi tiet ban ghi co catergoryID = id
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); // ma loi 400: thieu gia tri truyen vao
            }
            Category category = db.Categories.Find(id);
            if (category == null)// ko tim thay ban ghi
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Admin/Categories/Create
        // load from create
        // [HttpGet] là phương thức mặc định , nên không cần khai báo
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Categories/Create
        // POST: luu du lieu nhap vao tu from create vao database 
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryID,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Admin/Categories/Edit/5
        //GET: lay du lieu tu 1 danh muc da co sao cho catergoryID = id
        public ActionResult Edit(int? id)
        {
          
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryID,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }
        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);

            // BƯỚC 1: KIỂM TRA RÀNG BUỘC
            // Nếu danh mục này đang chứa bất kỳ sản phẩm nào -> KHÔNG CHO XÓA
            if (category.Products.Any())
            {
                // Gửi thông báo lỗi ra màn hình
                // TempData là cách truyền dữ liệu tạm thời giữa các trang
                TempData["Message"] = "Không thể xóa danh mục này vì đang chứa sản phẩm! Vui lòng xóa sản phẩm trước.";
                return RedirectToAction("Index");
            }

            // BƯỚC 2: NẾU TRỐNG THÌ CHO XÓA
            db.Categories.Remove(category);
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
