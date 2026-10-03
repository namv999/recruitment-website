using System;

namespace recruitment_website.Models.SavedJob
{
    public class SavedJobItemViewModel
    {
        public long JobId { get; set; }
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public string City { get; set; }
        public string SalaryText { get; set; }
        public DateTime SavedAt { get; set; }
        public bool IsOpen { get; set; }
        public bool HasApplied { get; set; }
    }
}