using System;
using System.Collections.Generic;
using recruitment_website.Models.Candidate;

namespace recruitment_website.Models.Application
{
    public class ApplicantStatusFilterViewModel
    {
        public string Value { get; set; }      // rỗng = "Tất cả"
        public string Label { get; set; }
        public int Count { get; set; }
        public bool IsActive { get; set; }
    }

    public class ApplicantListItemViewModel
    {
        public long ApplicationId { get; set; }
        public string CandidateName { get; set; }
        public string Headline { get; set; }
        public string City { get; set; }
        public decimal TotalExperienceYears { get; set; }
        public string Status { get; set; }
        public string StatusLabel { get; set; }
        public DateTime AppliedAt { get; set; }
        public bool HasNote { get; set; }
    }

    public class JobApplicantsViewModel
    {
        public long JobId { get; set; }
        public string JobTitle { get; set; }
        public int TotalCount { get; set; }
        public string CurrentStatus { get; set; }
        public List<ApplicantStatusFilterViewModel> StatusFilters { get; set; } = new List<ApplicantStatusFilterViewModel>();
        public List<ApplicantListItemViewModel> Items { get; set; } = new List<ApplicantListItemViewModel>();
    }

    public class ApplicantSkillViewModel
    {
        public string Name { get; set; }
        public byte Proficiency { get; set; }
        public decimal YearsExperience { get; set; }
    }

    public class ApplicantNextStatusViewModel
    {
        public string Value { get; set; }
        public string Label { get; set; }
        public bool IsDanger { get; set; }     // true cho "rejected"
    }

    public class ApplicantHistoryRowViewModel
    {
        public string FromLabel { get; set; }  // rỗng ở dòng đầu tiên
        public string ToLabel { get; set; }
        public DateTime ChangedAt { get; set; }
    }

    public class ApplicationReviewViewModel
    {
        public long Id { get; set; }
        public long JobId { get; set; }
        public string JobTitle { get; set; }
        public DateTime AppliedAt { get; set; }
        public string Status { get; set; }
        public string StatusLabel { get; set; }
        public string CoverLetter { get; set; }
        public string CvTitle { get; set; }

        public string CandidateName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Headline { get; set; }
        public string Summary { get; set; }
        public decimal TotalExperienceYears { get; set; }
        public string HighestEducationLabel { get; set; }
        public int? ExpectedSalary { get; set; }

        public List<ApplicantSkillViewModel> Skills { get; set; } = new List<ApplicantSkillViewModel>();
        public List<ExperienceItemViewModel> Experiences { get; set; } = new List<ExperienceItemViewModel>();
        public List<EducationItemViewModel> Educations { get; set; } = new List<EducationItemViewModel>();

        public List<ApplicantNextStatusViewModel> NextStatuses { get; set; } = new List<ApplicantNextStatusViewModel>();
        public string RecruiterNote { get; set; }
        public List<ApplicantHistoryRowViewModel> History { get; set; } = new List<ApplicantHistoryRowViewModel>();
    }
}