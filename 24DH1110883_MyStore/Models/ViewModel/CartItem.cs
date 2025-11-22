using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _24DH1110883_MyStore.Models.ViewModel
{
    public class CartItem
    {
        public int ProductID { get; set; }
        public string ProductImage { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        // Added Category so cart-related grouping/filters compile
        public string Category { get; set; }

        public decimal TotalPrice => UnitPrice * Quantity;
    }
}