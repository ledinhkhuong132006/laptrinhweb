using _24DH1110883_MyStore.Areas.Admin.Model.ViewModel;
using _24DH1110883_MyStore.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace _24DH1110883_MyStore.Areas.Admin.Controllers
{
    public class CustomersController : Controller
    {
        private MyStoreEntities2 db = new MyStoreEntities2();

        // GET: Admin/Customers
        public ActionResult Index(string searchName)
        {
            // 1. Lấy danh sách khách hàng từ bảng Users
            // Lưu ý: Tùy bảng của bạn tên là Users hay Customers mà sửa lại nhé
            var query = db.Customers.AsQueryable();

            // 2. Nếu có tìm kiếm tên
            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(u => u.Username.Contains(searchName) || u.CustomerName.Contains(searchName));
            }

            // 3. Chọn các cột cần thiết và tính toán số liệu
            var result = query.Select(u => new CustomerListVM
            {
                UserID = u.CustomerID,
                TenKhachHang = u.CustomerName, // Hoặc u.Username
                SoDienThoai = u.CustomerPhone,
                Email = u.CustomerEmail,
                DiaChi = u.CustomerAddress,

                // Đếm số đơn hàng của user này
                TongDonHang = u.Orders.Count(),

                // Tính tổng tiền các đơn hàng (Nếu chưa mua gì thì mặc định là 0)
                TongTienDaChi = u.Orders.Sum(o => (decimal?)o.TotalAmount) ?? 0
            })
            .OrderByDescending(x => x.TongTienDaChi) // Sắp xếp ai mua nhiều nhất lên đầu (khách VIP)
            .ToList();

            return View(result);
        }
        // GET: Admin/Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            // 1. Tìm khách hàng theo ID
            // Lưu ý: Sửa db.Users thành db.Customers nếu bảng bạn tên khác
            var customer = db.Customers.Find(id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            // 2. Sắp xếp đơn hàng của khách đó (Mới nhất lên đầu) để hiển thị cho đẹp
            // (Dòng này tùy chọn, nếu bảng User có quan hệ với Order thì nó tự có list Orders)
            customer.Orders = customer.Orders.OrderByDescending(o => o.OrderDate).ToList();

            return View(customer);
        }
    }
}