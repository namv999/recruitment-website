using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using recruitment_website.Models.Shared;

namespace recruitment_website.Models.Job
{
    /*
        Trang danh sách việc làm (Job/Index): điều kiện lọc (binding từ query string, GET) + kết quả + phân trang.
        Mọi điều kiện lọc đều tùy chọn — Controller chỉ nối Where khi có giá trị.
    */
    public class JobSearchViewModel
    {
        public const int DefaultPageSize = 10;

        public JobSearchViewModel()
        {
            Page = 1;
            Jobs = new List<JobCardViewModel>();
            CategoryOptions = new List<SelectListItem>();
            CityOptions = new List<SelectListItem>();
        }

        // ---- Điều kiện lọc ----
        [Display(Name = "Từ khóa")]
        public string Keyword { get; set; }

        [Display(Name = "Ngành nghề")]
        public int? CategoryId { get; set; }

        [Display(Name = "Thành phố")]
        public string City { get; set; }

        [Display(Name = "Cấp bậc")]
        public string Level { get; set; }

        [Display(Name = "Hình thức làm việc")]
        public string EmploymentType { get; set; }

        [Display(Name = "Lương từ (VND)")]
        public int? SalaryMin { get; set; }

        [Display(Name = "Lương đến (VND)")]
        public int? SalaryMax { get; set; }

        public int Page { get; set; }

        // ---- Kết quả ----
        public List<JobCardViewModel> Jobs { get; set; }
        public int TotalItems { get; set; }

        public int PageSize { get { return DefaultPageSize; } }
        public int TotalPages { get { return (int)Math.Ceiling(TotalItems / (double)PageSize); } }
        public bool HasPrevious { get { return Page > 1; } }
        public bool HasNext { get { return Page < TotalPages; } }

        public bool HasFilter
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Keyword) || CategoryId.HasValue
                    || !string.IsNullOrWhiteSpace(City) || !string.IsNullOrWhiteSpace(Level)
                    || !string.IsNullOrWhiteSpace(EmploymentType) || SalaryMin.HasValue || SalaryMax.HasValue;
            }
        }

        /// <summary>Thông tin cho partial _Pagination: giữ nguyên các điều kiện lọc khi chuyển trang.</summary>
        public PaginationViewModel Pagination
        {
            get
            {
                var route = new RouteValueDictionary();
                if (!string.IsNullOrWhiteSpace(Keyword)) route["Keyword"] = Keyword;
                if (CategoryId.HasValue) route["CategoryId"] = CategoryId.Value;
                if (!string.IsNullOrWhiteSpace(City)) route["City"] = City;
                if (!string.IsNullOrWhiteSpace(Level)) route["Level"] = Level;
                if (!string.IsNullOrWhiteSpace(EmploymentType)) route["EmploymentType"] = EmploymentType;
                if (SalaryMin.HasValue) route["SalaryMin"] = SalaryMin.Value;
                if (SalaryMax.HasValue) route["SalaryMax"] = SalaryMax.Value;

                return new PaginationViewModel
                {
                    Page = Page,
                    TotalPages = TotalPages,
                    Action = "Index",
                    Controller = "Job",
                    RouteValues = route
                };
            }
        }

        // ---- Dữ liệu cho dropdown ----
        public List<SelectListItem> CategoryOptions { get; set; }
        public List<SelectListItem> CityOptions { get; set; }
        public List<SelectListItem> LevelOptions { get { return JobSelectLists.Levels(Level); } }
        public List<SelectListItem> EmploymentTypeOptions { get { return JobSelectLists.EmploymentTypes(EmploymentType); } }
    }
}
