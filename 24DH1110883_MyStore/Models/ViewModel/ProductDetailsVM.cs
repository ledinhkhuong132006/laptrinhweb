using _24DH1110883_MyStore.Models;
using PagedList;
using System.Collections.Generic;

namespace _24DH1110883_MyStore.Models.ViewModel
{
    public class ProductDetailsVM
    {
        public Product product { get; set; }
        public int quantity { get; set; } = 1;

        // Estimated value computed from quantity and product price
        public decimal estimatedValue { get; set; }

        // Pagination helpers
        public int PageNumber { get; set; } // current page
        public int PageSize { get; set; } = 3; // items per page

        // Related and top products as paged lists (controller uses ToPagedList)
        public IPagedList<Product> RelatedProducts { get; set; }
        public IPagedList<Product> TopProducts { get; set; }
    }
}