using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _24DH1110883_MyStore.Areas.Admin.Model.ViewModel
{
    public class CustomerListVM
    {
        public int UserID { get; set; } // Mã khách hàng
        public string TenKhachHang { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }

        public int TongDonHang { get; set; } // Đã mua bao nhiêu đơn
        public decimal TongTienDaChi { get; set; } // Tổng tiền đã cúng cho shop
    }
}