using System;
using System.Collections.Generic;

namespace recruitment_website.Models.Candidate
{
    public class CandidateProfileViewModel
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string GenderLabel { get; set; }
        public string City { get; set; }
        public string Headline { get; set; }
        public string Summary { get; set; }
        public decimal TotalExperienceYears { get; set; }
        public string HighestEducationLabel { get; set; }
        public int? ExpectedSalary { get; set; }
        public bool OpenToWork { get; set; }

        public List<ExperienceItemViewModel> Experiences { get; set; } = new List<ExperienceItemViewModel>();
        public List<EducationItemViewModel> Educations { get; set; } = new List<EducationItemViewModel>();
        public List<CandidateSkillRowViewModel> Skills { get; set; } = new List<CandidateSkillRowViewModel>();
    }

    public class ExperienceItemViewModel
    {
        public long Id { get; set; }
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public bool IsCurrent { get { return !EndDate.HasValue; } }
    }

    public class EducationItemViewModel
    {
        public long Id { get; set; }
        public string SchoolName { get; set; }
        public string Major { get; set; }
        public string DegreeLabel { get; set; }
        public short? StartYear { get; set; }
        public short? EndYear { get; set; }
    }
}