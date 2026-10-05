using System;
using System.Web.Routing;

namespace recruitment_website.Models.Shared
{
    /*
        Model của partial Views/Shared/_Pagination.cshtml.
        RouteValues: các tham số lọc hiện tại cần giữ nguyên khi chuyển trang (vd Keyword, City...).
    */
    public class PaginationViewModel
    {
        private const int Window = 2; // số trang hiển thị mỗi bên trang hiện tại

        public PaginationViewModel()
        {
            Action = "Index";
            RouteValues = new RouteValueDictionary();
        }

        public int Page { get; set; }
        public int TotalPages { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public RouteValueDictionary RouteValues { get; set; }

        public bool HasPrevious { get { return Page > 1; } }
        public bool HasNext { get { return Page < TotalPages; } }
        public int StartPage { get { return Math.Max(1, Page - Window); } }
        public int EndPage { get { return Math.Min(TotalPages, Page + Window); } }

        public RouteValueDictionary RouteFor(int page)
        {
            var values = new RouteValueDictionary(RouteValues);
            values["Page"] = page;
            return values;
        }
    }
}
