using _24DH1110883_MyStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _24DH1110883_MyStore.Models // Nhớ đổi namespace theo bài bạn
{
    [Table("Orders")]
    public class Orders
    {
        [Key]
        public int OrderID { get; set; }

        [Display(Name = "Ngày đặt hàng")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Tên khách hàng")]
        public string CustomerName { get; set; }

        [Display(Name = "Số điện thoại")]
        public string CustomerPhone { get; set; }

        [Display(Name = "Địa chỉ giao hàng")]
        public string ShippingAddress { get; set; }

        [Display(Name = "Email")]
        public string CustomerEmail { get; set; }

        [Display(Name = "Tổng tiền")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Trạng thái thanh toán")]
        public string PaymentStatus { get; set; } // "Chưa thanh toán", "Đã thanh toán Paypal"

        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; } // "COD", "PayPal"

        [Display(Name = "Trạng thái giao hàng")]
        public string DeliveryStatus { get; set; } // "Đang xử lý", "Đang giao", "Đã giao"

        // Liên kết với người dùng (Nếu có chức năng đăng nhập)
        public string UserID { get; set; } // Hoặc int tùy vào bảng User của bạn

        // Quan hệ 1-nhiều với chi tiết đơn hàng
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
    }
}