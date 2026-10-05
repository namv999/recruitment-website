using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace recruitment_website.Models.Application
{
    public class ApplicationListItemViewModel
    {
        public long ApplicationId { get; set; }
        public long JobId { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public string CvTitle { get; set; }
        public string Status { get; set; }
        public string StatusLabel { get; set; }
        public DateTime AppliedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool CanWithdraw { get; set; }
    }

    public class MyApplicationsViewModel
    {
        public string StatusFilter { get; set; }
        public List<SelectListItem> StatusOptions { get; set; } = new List<SelectListItem>();
        public List<ApplicationListItemViewModel> Items { get; set; } = new List<ApplicationListItemViewModel>();
    }

    public class ApplicationHistoryItemViewModel
    {
        public string FromStatusLabel { get; set; }
        public string ToStatusLabel { get; set; }
        public DateTime ChangedAt { get; set; }
    }

    public class ApplicationDetailViewModel
    {
        public long ApplicationId { get; set; }
        public long JobId { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public string CvTitle { get; set; }
        public string CoverLetter { get; set; }
        public string Status { get; set; }
        public string StatusLabel { get; set; }
        public DateTime AppliedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool CanWithdraw { get; set; }
        public List<ApplicationHistoryItemViewModel> History { get; set; } = new List<ApplicationHistoryItemViewModel>();
    }
}