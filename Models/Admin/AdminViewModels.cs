using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace recruitment_website.Models.Admin
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int PendingCompanies { get; set; }
        public int PendingJobs { get; set; }
        public int TotalSkills { get; set; }
        public int TotalJobCategories { get; set; }
    }

    public class AdminUserItemViewModel
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public System.DateTime? LastLoginAt { get; set; }
        public bool CanChangeStatus { get; set; }
    }

    public class AdminUsersViewModel
    {
        public string Keyword { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public IList<AdminUserItemViewModel> Users { get; set; }
    }

    public class AdminCompaniesViewModel
    {
        public bool? Verified { get; set; }
        public IList<AdminCompanyItemViewModel> Companies { get; set; }
    }

    public class AdminCompanyItemViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string TaxCode { get; set; }
        public string City { get; set; }
        public bool IsVerified { get; set; }
        public int JobCount { get; set; }
    }

    public class AdminSkillsViewModel
    {
        public IList<AdminSkillItemViewModel> Skills { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        public string Keyword { get; set; }
    }

    public class AdminSkillItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int CandidateUsageCount { get; set; }
        public int JobUsageCount { get; set; }
    }

    public class AdminSkillFormViewModel
    {
        public int? Id { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Tên kỹ năng")]
        public string Name { get; set; }

        [Display(Name = "Nhóm kỹ năng")]
        public int? CategoryId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
    }

    public class AdminCategoriesViewModel
    {
        public IList<AdminCategoryItemViewModel> SkillCategories { get; set; }
        public IList<AdminCategoryItemViewModel> JobCategories { get; set; }
    }

    public class AdminCategoryItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UsageCount { get; set; }
    }

    public class AdminJobsViewModel
    {
        public IList<AdminJobItemViewModel> Jobs { get; set; }
    }

    public class AdminJobItemViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public int ApplicationCount { get; set; }
    }
}
