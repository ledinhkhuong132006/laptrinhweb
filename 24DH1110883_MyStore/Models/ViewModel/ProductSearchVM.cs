using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _24DH1110883_MyStore.Models;
using _24DH1110883_MyStore.Models.ViewModel;
using PagedList.Mvc;

namespace _24DH1110883_MyStore.Models.ViewModel
{
    public class ProductSearchVM
    {
        public string SearchTerm { get; set; }
        // các tiêu chí search theo giá
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        // thứ tự sắp xếp
        public string SortOrder { get; set; }
        // phân trang
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        // danh sách sản phẩm phân trang
        public PagedList.IPagedList<Product> Products { get; set; }
        // danh sách thỏa diều kiện tìm kiếm
        public List<Product> AllProducts { get; set; }

    }
}