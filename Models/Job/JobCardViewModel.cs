using System;
using System.Collections.Generic;
using recruitment_website.Constants;

namespace recruitment_website.Models.Job
{
    /* Một tin trong danh sách (partial _JobCard.cshtml). */
    public class JobCardViewModel
    {
        public JobCardViewModel()
        {
            SkillNames = new List<string>();
        }

        public long Id { get; set; }
        public string Title { get; set; }

        public string CompanyName { get; set; }
        public string CompanyLogoUrl { get; set; }

        public string City { get; set; }
        public string EmploymentType { get; set; }
        public string Level { get; set; }
        public string WorkMode { get; set; }

        public int? SalaryMin { get; set; }
        public int? SalaryMax { get; set; }
        public string Currency { get; set; }

        public DateTime? PublishedAt { get; set; }
        public List<string> SkillNames { get; set; }

        public string SalaryText { get { return JobDisplayHelper.Salary(SalaryMin, SalaryMax, Currency); } }
        public string PostedAgo { get { return JobDisplayHelper.PostedAgo(PublishedAt); } }
        public string CompanyInitial { get { return JobDisplayHelper.Initial(CompanyName); } }
        public string EmploymentTypeLabel { get { return Constants.EmploymentType.Label(EmploymentType); } }
        public string LevelLabel { get { return JobLevel.Label(Level); } }
        public string WorkModeLabel { get { return Constants.WorkMode.Label(WorkMode); } }
    }
}
