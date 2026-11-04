using System;
using System.Collections.Generic;
using System.Linq;
using recruitment_website.Constants;

namespace recruitment_website.Models.Job
{
    /* Một kỹ năng yêu cầu của tin. */
    public class JobSkillItemViewModel
    {
        public string Name { get; set; }
        public string Importance { get; set; }
        public string ImportanceLabel { get { return SkillImportance.Label(Importance); } }
    }

    /* Trang chi tiết tin (Job/Details): nội dung tin + thông tin công ty đăng tin. */
    public class JobDetailViewModel
    {
        public JobDetailViewModel()
        {
            Skills = new List<JobSkillItemViewModel>();
        }

        // ---- Tin ----
        public long Id { get; set; }
        public string Title { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string Requirements { get; set; }
        public string Benefits { get; set; }

        public string EmploymentType { get; set; }
        public string Level { get; set; }
        public string WorkMode { get; set; }
        public string City { get; set; }
        public string Address { get; set; }

        public int? SalaryMin { get; set; }
        public int? SalaryMax { get; set; }
        public bool SalaryNegotiable { get; set; }
        public string Currency { get; set; }

        public decimal MinExperienceYears { get; set; }
        public string MinEducation { get; set; }
        public short Headcount { get; set; }

        public string Status { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public int ViewCount { get; set; }

        public List<JobSkillItemViewModel> Skills { get; set; }

        // ---- Công ty đăng tin ----
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogoUrl { get; set; }
        public string CompanyIndustry { get; set; }
        public string CompanySize { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyWebsite { get; set; }
        public string CompanyDescription { get; set; }
        public bool CompanyIsVerified { get; set; }

        // ---- Quyền xem ----
        /// <summary>True nếu người đang xem thuộc công ty đăng tin (được sửa / tạm dừng / đóng).</summary>
        public bool IsOwner { get; set; }

        // ---- Dành cho ứng viên (module C: Ứng tuyển / Lưu tin) ----
        /// <summary>Đã đăng nhập (bất kể role).</summary>
        public bool IsLoggedIn { get; set; }
        /// <summary>Người xem là ứng viên.</summary>
        public bool IsCandidate { get; set; }
        /// <summary>Ứng viên đã nộp đơn cho tin này.</summary>
        public bool HasApplied { get; set; }
        /// <summary>Id đơn đã nộp (link tới Application/Details), null nếu chưa nộp.</summary>
        public long? ApplicationId { get; set; }
        /// <summary>Ứng viên đã lưu tin này.</summary>
        public bool IsSaved { get; set; }
        /// <summary>Hồ sơ thiếu SĐT hoặc thành phố.</summary>
        public bool ProfileIncomplete { get; set; }
        /// <summary>Ứng viên chưa có CV nào.</summary>
        public bool HasNoCv { get; set; }

        // ---- Hiển thị ----
        public string SalaryText { get { return JobDisplayHelper.Salary(SalaryMin, SalaryMax, Currency); } }
        public string PostedAgo { get { return JobDisplayHelper.PostedAgo(PublishedAt); } }
        public string CompanyInitial { get { return JobDisplayHelper.Initial(CompanyName); } }
        public string EmploymentTypeLabel { get { return Constants.EmploymentType.Label(EmploymentType); } }
        public string LevelLabel { get { return JobLevel.Label(Level); } }
        public string WorkModeLabel { get { return Constants.WorkMode.Label(WorkMode); } }
        public string MinEducationLabel { get { return string.IsNullOrEmpty(MinEducation) ? "Không yêu cầu" : EducationLevel.Label(MinEducation); } }
        public string StatusLabel { get { return JobStatus.Label(Status); } }
        public bool IsPublished { get { return Status == JobStatus.Published; } }
        /// <summary>Tin còn nhận hồ sơ: đã công khai và chưa hết hạn (khớp điều kiện của Application/Apply).</summary>
        public bool IsOpenForApply
        {
            get { return Status == JobStatus.Published && (!ExpiresAt.HasValue || ExpiresAt.Value >= DateTime.Now); }
        }

        public string MinExperienceText
        {
            get { return MinExperienceYears <= 0 ? "Không yêu cầu" : MinExperienceYears.ToString("0.##") + " năm"; }
        }

        public IEnumerable<JobSkillItemViewModel> RequiredSkills
        {
            get { return Skills.Where(s => s.Importance == SkillImportance.Required); }
        }

        public IEnumerable<JobSkillItemViewModel> OptionalSkills
        {
            get { return Skills.Where(s => s.Importance != SkillImportance.Required); }
        }

        /// <summary>Chỉ trả về link http/https hợp lệ (chặn javascript: ...), null nếu không dùng được.</summary>
        public string CompanyWebsiteUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CompanyWebsite)) return null;
                string w = CompanyWebsite.Trim();
                if (w.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || w.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) return w;
                if (w.Contains(":")) return null;
                return "https://" + w;
            }
        }
    }
}