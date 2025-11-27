using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace _24DH1110883_MyStore.ViewModels
{
    public class ProductStatisticVM
    {
        public string TenSanPham { get; set; }
        public int SoLuongDaBan { get; set; }
        public decimal TongDoanhThu { get; set; } // Nếu muốn hiển thị cả tiền thu được
        public string HinhAnh { get; set; } // Để hiển thị ảnh cho đẹp
    }
}