using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _24DH1110883_MyStore.Areas.Admin.Model.ViewModel
{
    public class CustomerListVM
    {
        public int UserID { get; set; }
        public string TenKhachHang { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }

        public int TongDonHang { get; set; } 
        public decimal TongTienDaChi { get; set; }
    }
}