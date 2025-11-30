using _24DH1110883_MyStore.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using _24DH1110883_MyStore.Models.ViewModel;

namespace _24DH1110883_MyStore.Models
{
    [Table("OrderDetails")]
    public class OrderDetails
    {
        [Key]
        public int ID { get; set; }

        public int OrderID { get; set; } // Khóa ngoại trỏ về bảng Order

        public int ProductID { get; set; } // Khóa ngoại trỏ về bảng Product

        public int Quantity { get; set; } // Số lượng mua

        public decimal UnitPrice { get; set; } // Giá tại thời điểm mua

        public decimal TotalPrice { get; set; } // Thành tiền (Quantity * Price)

        // Relationship
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}