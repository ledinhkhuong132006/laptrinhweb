using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _24DH1110883_MyStore.Models;
using _24DH1110883_MyStore.Models.ViewModel;
using PagedList.Mvc;

namespace _24DH1110883_MyStore.Models.ViewModel
{
    public class HomeProductVM

    {
        // tiêu chí để select sản phẩm hiển thị trên trang chủ
        // hoặc lại sản phẩm
        public string SearchTerm { get; set; }
        // cac thuoc tinh ho tro phan trang 
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        // danh sach noi bat
                public List<Product> FeaturedProducts { get; set; }
        // danh sach san pham da phan trang
                public PagedList.IPagedList<Product> NewProducts { get; set; }
    }
}