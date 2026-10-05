using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using recruitment_website.Constants;
using recruitment_website.Models.Shared;

namespace recruitment_website.Models.Job
{
    /*
        Các trường chung của form Đăng tin / Sửa tin.
        JobCreateViewModel và JobEditViewModel kế thừa class này.

        Lưu ý: các danh sách dropdown (CategoryOptions, SkillSelector...) KHÔNG được POST về,
        nên khi ModelState không hợp lệ và return View(model) thì Controller phải nạp lại chúng.
    */
    public abstract class JobFormViewModel : IValidatableObject
    {
        protected JobFormViewModel()
        {
            EmploymentType = Constants.EmploymentType.FullTime;
            Level = JobLevel.Fresher;
            WorkMode = Constants.WorkMode.Onsite;
            Headcount = 1;
            MinExperienceYears = 0;
            SelectedSkillIds = new List<int>();
            CategoryOptions = new List<SelectListItem>();
            SkillSelector = new SkillSelectorViewModel();
        }

        // ---- Thông tin chung ----
        [Required(ErrorMessage = "Vui lòng nhập chức danh.")]
        [StringLength(255, ErrorMessage = "Chức danh tối đa 255 ký tự.")]
        [Display(Name = "Chức danh")]
        public string Title { get; set; }

        [Display(Name = "Ngành nghề")]
        public int? CategoryId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả công việc.")]
        [Display(Name = "Mô tả công việc")]
        public string Description { get; set; }

        [Display(Name = "Yêu cầu ứng viên")]
        public string Requirements { get; set; }

        [Display(Name = "Quyền lợi")]
        public string Benefits { get; set; }

        // ---- Điều kiện làm việc ----
        [Required(ErrorMessage = "Vui lòng chọn hình thức làm việc.")]
        [Display(Name = "Hình thức làm việc")]
        public string EmploymentType { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn cấp bậc.")]
        [Display(Name = "Cấp bậc")]
        public string Level { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nơi làm việc.")]
        [Display(Name = "Nơi làm việc")]
        public string WorkMode { get; set; }

        [StringLength(100, ErrorMessage = "Thành phố tối đa 100 ký tự.")]
        [Display(Name = "Thành phố")]
        public string City { get; set; }

        [StringLength(500, ErrorMessage = "Địa chỉ tối đa 500 ký tự.")]
        [Display(Name = "Địa chỉ làm việc")]
        public string Address { get; set; }

        // ---- Lương ----
        [Range(0, int.MaxValue, ErrorMessage = "Lương tối thiểu không hợp lệ.")]
        [Display(Name = "Lương tối thiểu (VND)")]
        public int? SalaryMin { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Lương tối đa không hợp lệ.")]
        [Display(Name = "Lương tối đa (VND)")]
        public int? SalaryMax { get; set; }

        [Display(Name = "Lương thỏa thuận")]
        public bool SalaryNegotiable { get; set; }

        // ---- Yêu cầu tuyển ----
        [Range(0, 50, ErrorMessage = "Số năm kinh nghiệm phải từ 0 đến 50.")]
        [Display(Name = "Kinh nghiệm tối thiểu (năm)")]
        public decimal MinExperienceYears { get; set; }

        [Display(Name = "Học vấn tối thiểu")]
        public string MinEducation { get; set; }

        [Range(1, 10000, ErrorMessage = "Số lượng cần tuyển phải từ 1 đến 10000.")]
        [Display(Name = "Số lượng cần tuyển")]
        public int Headcount { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Hạn nộp hồ sơ")]
        public DateTime? ExpiresAt { get; set; }

        // ---- Kỹ năng (job_skills) ----
        [Display(Name = "Kỹ năng yêu cầu")]
        public List<int> SelectedSkillIds { get; set; }

        // ---- Dữ liệu cho dropdown / selector (không POST) ----
        public List<SelectListItem> CategoryOptions { get; set; }
        public SkillSelectorViewModel SkillSelector { get; set; }

        public List<SelectListItem> EmploymentTypeOptions { get { return JobSelectLists.EmploymentTypes(EmploymentType); } }
        public List<SelectListItem> LevelOptions { get { return JobSelectLists.Levels(Level); } }
        public List<SelectListItem> WorkModeOptions { get { return JobSelectLists.WorkModes(WorkMode); } }
        public List<SelectListItem> EducationOptions { get { return JobSelectLists.Educations(MinEducation); } }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(EmploymentType) && !Constants.EmploymentType.All.Contains(EmploymentType))
                yield return new ValidationResult("Hình thức làm việc không hợp lệ.", new[] { "EmploymentType" });

            if (!string.IsNullOrEmpty(Level) && !JobLevel.All.Contains(Level))
                yield return new ValidationResult("Cấp bậc không hợp lệ.", new[] { "Level" });

            if (!string.IsNullOrEmpty(WorkMode) && !Constants.WorkMode.All.Contains(WorkMode))
                yield return new ValidationResult("Nơi làm việc không hợp lệ.", new[] { "WorkMode" });

            if (!string.IsNullOrEmpty(MinEducation) && !EducationLevel.All.Contains(MinEducation))
                yield return new ValidationResult("Học vấn không hợp lệ.", new[] { "MinEducation" });

            if (SalaryMin.HasValue && SalaryMax.HasValue && SalaryMax.Value < SalaryMin.Value)
                yield return new ValidationResult("Lương tối đa phải lớn hơn hoặc bằng lương tối thiểu.", new[] { "SalaryMax" });

            if (ExpiresAt.HasValue && ExpiresAt.Value.Date < DateTime.Today)
                yield return new ValidationResult("Hạn nộp hồ sơ không được nằm trong quá khứ.", new[] { "ExpiresAt" });
        }
    }
}
