using System;
using System.Collections.Generic;

namespace _24DH1110883_MyStore.Models.ViewModel
{
    // View-model only — renamed to CustomerVM to avoid colliding with EF-generated Customer entity
    public class CustomerVM
    {
        public CustomerVM()
        {
            this.Orders = new HashSet<_24DH1110883_MyStore.Models.Order>();
        }

        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public string Username { get; set; }

        // Keep EF Order type fully-qualified to avoid namespace issues
        public ICollection<_24DH1110883_MyStore.Models.Order> Orders { get; set; }
        public _24DH1110883_MyStore.Models.User User { get; set; }
    }
}