using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using recruitment_website.Constants;
using recruitment_website.DAL;
using recruitment_website.Models.Job;
using recruitment_website.Models.Shared;

namespace recruitment_website.Services
{
    public class JobService : IJobService
    {
        private const int MaxKeywordLength = 100;
        private const int MaxSkillsPerCard = 5;

        // Sửa tin đang công khai thì đưa về "chờ duyệt" để Admin duyệt lại (tránh qua mặt khâu duyệt bằng cách sửa sau khi được duyệt).
        // Muốn cho sửa tin đang công khai mà không cần duyệt lại: đổi thành false.
        private const bool RequireReapprovalOnEdit = true;

        private readonly recruitment_dbEntities _db;

        public JobService(recruitment_dbEntities db)
        {
            _db = db;
        }

        public void Search(JobSearchViewModel model)
        {
            NormalizeFilter(model);

            // 1. Query gốc: chỉ tin đang công khai và chưa hết hạn
            var now = DateTime.Now;
            IQueryable<jobs> query = _db.jobs
                .AsNoTracking()
                .Where(j => j.status == JobStatus.Published
                         && (j.expires_at == null || j.expires_at >= now));

            // 2. Nối điều kiện lọc — chỉ nối khi có giá trị
            if (!string.IsNullOrEmpty(model.Keyword))
            {
                string keyword = model.Keyword;
                query = query.Where(j => j.title.Contains(keyword)); // EF6 dịch thành LIKE '%...%'
            }

            if (model.CategoryId.HasValue)
            {
                int categoryId = model.CategoryId.Value;
                query = query.Where(j => j.category_id == categoryId);
            }

            if (!string.IsNullOrEmpty(model.City))
            {
                string city = model.City;
                query = query.Where(j => j.city == city);
            }

            if (!string.IsNullOrEmpty(model.Level))
            {
                string level = model.Level;
                query = query.Where(j => j.level == level);
            }

            if (!string.IsNullOrEmpty(model.EmploymentType))
            {
                string employmentType = model.EmploymentType;
                query = query.Where(j => j.employment_type == employmentType);
            }

            // Lương: khoảng lương của tin giao với khoảng người dùng chọn.
            // Tin chỉ có 1 đầu (min hoặc max) thì dùng đầu đó; tin không ghi lương (thỏa thuận) bị loại khi đang lọc lương.
            if (model.SalaryMin.HasValue)
            {
                int salaryMin = model.SalaryMin.Value;
                query = query.Where(j => (j.salary_max ?? j.salary_min) >= salaryMin);
            }

            if (model.SalaryMax.HasValue)
            {
                int salaryMax = model.SalaryMax.Value;
                query = query.Where(j => (j.salary_min ?? j.salary_max) <= salaryMax);
            }

            // 3. Đếm + phân trang
            model.TotalItems = query.Count();

            if (model.Page > model.TotalPages) model.Page = model.TotalPages;
            if (model.Page < 1) model.Page = 1;

            var rows = query
                .OrderByDescending(j => j.published_at)
                .ThenByDescending(j => j.id)
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .Select(j => new
                {
                    j.id,
                    j.title,
                    CompanyName = j.companies.name,
                    CompanyLogoUrl = j.companies.logo_url,
                    j.city,
                    j.employment_type,
                    j.level,
                    j.work_mode,
                    j.salary_min,
                    j.salary_max,
                    j.currency,
                    j.published_at
                })
                .ToList();

            // 4. Kỹ năng của các tin trong trang (1 query riêng, tránh N+1)
            var jobIds = rows.Select(r => r.id).ToList();
            var skillRows = jobIds.Count == 0
                ? new List<JobSkillName>()
                : _db.job_skills
                    .AsNoTracking()
                    .Where(js => jobIds.Contains(js.job_id))
                    .OrderByDescending(js => js.weight)
                    .Select(js => new JobSkillName { JobId = js.job_id, Name = js.skills.name })
                    .ToList();

            model.Jobs = rows.Select(r => new JobCardViewModel
            {
                Id = r.id,
                Title = r.title,
                CompanyName = r.CompanyName,
                CompanyLogoUrl = r.CompanyLogoUrl,
                City = r.city,
                EmploymentType = r.employment_type,
                Level = r.level,
                WorkMode = r.work_mode,
                SalaryMin = r.salary_min,
                SalaryMax = r.salary_max,
                Currency = r.currency,
                PublishedAt = r.published_at,
                SkillNames = skillRows
                    .Where(s => s.JobId == r.id)
                    .Select(s => s.Name)
                    .Take(MaxSkillsPerCard)
                    .ToList()
            }).ToList();

            // 5. Dữ liệu cho dropdown bộ lọc
            LoadFilterOptions(model);
        }

        public JobDetailViewModel GetDetail(long id, long? currentUserId, string currentUserRole)
        {
            var job = _db.jobs
                .AsNoTracking()
                .Include(j => j.companies)
                .Include(j => j.job_categories)
                .FirstOrDefault(j => j.id == id);

            if (job == null) return null;

            bool isOwner = currentUserId.HasValue
                && _db.company_members.Any(m => m.company_id == job.company_id && m.user_id == currentUserId.Value);
            bool isAdmin = currentUserRole == UserRole.Admin;

            bool isPublic = job.status == JobStatus.Published
                && (job.expires_at == null || job.expires_at >= DateTime.Now);

            // Tin chưa công khai (draft/pending/paused/closed/expired): chỉ chủ tin hoặc Admin xem được
            if (!isPublic && !isOwner && !isAdmin) return null;

            // Chỉ đếm lượt xem của người xem bình thường trên tin đang công khai.
            // Cập nhật bằng SQL trực tiếp để cộng dồn an toàn khi nhiều người xem cùng lúc và không đổi updated_at.
            int viewCount = job.view_count;
            if (isPublic && !isOwner && !isAdmin)
            {
                _db.Database.ExecuteSqlCommand("UPDATE jobs SET view_count = view_count + 1 WHERE id = @p0", id);
                viewCount++;
            }

            var skills = _db.job_skills
                .AsNoTracking()
                .Where(js => js.job_id == id)
                .Select(js => new { Name = js.skills.name, js.importance, js.weight })
                .ToList()
                .OrderBy(s => Array.IndexOf(SkillImportance.All, s.importance))
                .ThenByDescending(s => s.weight)
                .Select(s => new JobSkillItemViewModel { Name = s.Name, Importance = s.importance })
                .ToList();

            var company = job.companies;

            return new JobDetailViewModel
            {
                Id = job.id,
                Title = job.title,
                CategoryName = job.job_categories != null ? job.job_categories.name : null,
                Description = job.description,
                Requirements = job.requirements,
                Benefits = job.benefits,
                EmploymentType = job.employment_type,
                Level = job.level,
                WorkMode = job.work_mode,
                City = job.city,
                Address = job.address,
                SalaryMin = job.salary_min,
                SalaryMax = job.salary_max,
                SalaryNegotiable = job.salary_negotiable,
                Currency = job.currency,
                MinExperienceYears = job.min_experience_years,
                MinEducation = job.min_education,
                Headcount = job.headcount,
                Status = job.status,
                PublishedAt = job.published_at,
                ExpiresAt = job.expires_at,
                ViewCount = viewCount,
                Skills = skills,
                CompanyId = company.id,
                CompanyName = company.name,
                CompanyLogoUrl = company.logo_url,
                CompanyIndustry = company.industry,
                CompanySize = company.company_size,
                CompanyCity = company.city,
                CompanyWebsite = company.website,
                CompanyDescription = company.description,
                CompanyIsVerified = company.is_verified,
                IsOwner = isOwner
            };
        }

        public long? GetCompanyId(long userId)
        {
            return _db.company_members
                .Where(m => m.user_id == userId && m.member_role != CompanyMemberRole.Viewer)
                .Select(m => (long?)m.company_id)
                .FirstOrDefault();
        }

        public bool CategoryExists(int categoryId)
        {
            return _db.job_categories.Any(c => c.id == categoryId);
        }

        public void LoadFormOptions(JobFormViewModel model)
        {
            model.CategoryOptions = _db.job_categories
                .AsNoTracking()
                .OrderBy(c => c.name)
                .ToList()
                .Select(c => new SelectListItem
                {
                    Value = c.id.ToString(),
                    Text = c.name,
                    Selected = model.CategoryId == c.id
                })
                .ToList();

            var skillOptions = _db.skills
                .AsNoTracking()
                .OrderBy(s => s.name)
                .Select(s => new SkillOptionViewModel
                {
                    Id = s.id,
                    Name = s.name,
                    CategoryName = s.skill_categories.name
                })
                .ToList();

            model.SkillSelector = new SkillSelectorViewModel
            {
                FieldName = "SelectedSkillIds",
                Options = skillOptions,
                SelectedIds = model.SelectedSkillIds ?? new List<int>()
            };
        }

        public long Create(JobCreateViewModel model, long userId)
        {
            long? companyId = GetCompanyId(userId);
            if (!companyId.HasValue)
                throw new InvalidOperationException("Tài khoản chưa thuộc công ty nào.");

            var now = DateTime.Now;

            var job = new jobs
            {
                company_id = companyId.Value,
                posted_by = userId,
                slug = GenerateUniqueSlug(model.Title),
                currency = "VND",
                status = JobStatus.Pending,   // luôn chờ Admin duyệt, không cho người đăng tự chọn trạng thái
                published_at = null,
                view_count = 0,
                created_at = now,
                updated_at = now
            };
            ApplyFormFields(job, model);

            foreach (int skillId in GetValidSkillIds(model.SelectedSkillIds))
            {
                job.job_skills.Add(new job_skills
                {
                    skill_id = skillId,
                    importance = SkillImportance.Required,
                    weight = 5,
                    min_proficiency = 1,
                    min_years = 0
                });
            }

            _db.jobs.Add(job);
            _db.SaveChanges(); // 1 lần SaveChanges = 1 transaction cho cả tin lẫn job_skills

            return job.id;
        }

        public JobEditViewModel GetForEdit(long id, long userId)
        {
            var job = ManageableJobs(userId)
                .Include(j => j.job_skills)
                .AsNoTracking()
                .FirstOrDefault(j => j.id == id);

            if (job == null) return null;

            var model = new JobEditViewModel
            {
                Id = job.id,
                CurrentStatus = job.status,
                WillBeReviewedAgain = NeedsReapproval(job.status),
                CategoryId = job.category_id,
                Title = job.title,
                Description = job.description,
                Requirements = job.requirements,
                Benefits = job.benefits,
                EmploymentType = job.employment_type,
                Level = job.level,
                WorkMode = job.work_mode,
                City = job.city,
                Address = job.address,
                SalaryMin = job.salary_min,
                SalaryMax = job.salary_max,
                SalaryNegotiable = job.salary_negotiable,
                MinExperienceYears = job.min_experience_years,
                MinEducation = job.min_education,
                Headcount = job.headcount,
                ExpiresAt = job.expires_at.HasValue ? (DateTime?)job.expires_at.Value.Date : null,
                SelectedSkillIds = job.job_skills.Select(js => js.skill_id).ToList()
            };

            LoadFormOptions(model);
            return model;
        }

        public string Update(JobEditViewModel model, long userId)
        {
            var job = ManageableJobs(userId)
                .Include(j => j.job_skills)
                .FirstOrDefault(j => j.id == model.Id);

            if (job == null) return null;
            if (!JobStatus.IsEditable(job.status)) return null;

            ApplyFormFields(job, model); // slug giữ nguyên để link cũ không hỏng

            if (NeedsReapproval(job.status))
            {
                job.status = JobStatus.Pending;
                job.published_at = null;
            }

            // Đồng bộ job_skills: xóa kỹ năng bị bỏ chọn, thêm kỹ năng mới, giữ nguyên kỹ năng cũ (giữ weight/importance đã có)
            var wanted = GetValidSkillIds(model.SelectedSkillIds);

            var toRemove = job.job_skills.Where(js => !wanted.Contains(js.skill_id)).ToList();
            foreach (var js in toRemove)
                _db.job_skills.Remove(js);

            var existingIds = job.job_skills.Select(js => js.skill_id).ToList();
            foreach (int skillId in wanted.Where(id => !existingIds.Contains(id)))
            {
                job.job_skills.Add(new job_skills
                {
                    skill_id = skillId,
                    importance = SkillImportance.Required,
                    weight = 5,
                    min_proficiency = 1,
                    min_years = 0
                });
            }

            job.updated_at = DateTime.Now;
            _db.SaveChanges();

            return job.status;
        }

        public List<JobManageItemViewModel> GetMyJobs(long userId)
        {
            var rows = ManageableJobs(userId)
                .AsNoTracking()
                .OrderByDescending(j => j.created_at)
                .ThenByDescending(j => j.id)
                .Select(j => new
                {
                    j.id,
                    j.title,
                    j.status,
                    j.headcount,
                    j.view_count,
                    ApplicationCount = j.applications.Count(),
                    j.created_at,
                    j.published_at,
                    j.expires_at
                })
                .ToList();

            return rows.Select(r => new JobManageItemViewModel
            {
                Id = r.id,
                Title = r.title,
                Status = r.status,
                Headcount = r.headcount,
                ViewCount = r.view_count,
                ApplicationCount = r.ApplicationCount,
                CreatedAt = r.created_at,
                PublishedAt = r.published_at,
                ExpiresAt = r.expires_at
            }).ToList();
        }

        public StatusChangeResult Pause(long id, long userId)
        {
            return ChangeStatus(id, userId, new[] { JobStatus.Published }, JobStatus.Paused);
        }

        public StatusChangeResult Resume(long id, long userId)
        {
            return ChangeStatus(id, userId, new[] { JobStatus.Paused }, JobStatus.Published);
        }

        public StatusChangeResult Close(long id, long userId)
        {
            return ChangeStatus(id, userId,
                new[] { JobStatus.Draft, JobStatus.Pending, JobStatus.Published, JobStatus.Paused },
                JobStatus.Closed);
        }

        private StatusChangeResult ChangeStatus(long id, long userId, string[] allowedFrom, string newStatus)
        {
            var job = ManageableJobs(userId).FirstOrDefault(j => j.id == id);
            if (job == null) return StatusChangeResult.NotFound;
            if (!allowedFrom.Contains(job.status)) return StatusChangeResult.InvalidState;

            job.status = newStatus;
            job.updated_at = DateTime.Now;
            _db.SaveChanges();

            return StatusChangeResult.Success;
        }

        // Tin đang công khai hoặc tạm dừng mà bị sửa nội dung thì phải được duyệt lại
        private static bool NeedsReapproval(string status)
        {
            return RequireReapprovalOnEdit && (status == JobStatus.Published || status == JobStatus.Paused);
        }

        // Các tin mà user được quyền quản lý: tin của công ty mà user là thành viên (trừ vai trò chỉ xem)
        private IQueryable<jobs> ManageableJobs(long userId)
        {
            return _db.jobs.Where(j => j.companies.company_members
                .Any(m => m.user_id == userId && m.member_role != CompanyMemberRole.Viewer));
        }

        // Ghi các trường nhập từ form (dùng chung cho Create và Update)
        private static void ApplyFormFields(jobs job, JobFormViewModel model)
        {
            job.category_id = model.CategoryId;
            job.title = model.Title.Trim();
            job.description = model.Description.Trim();
            job.requirements = CleanText(model.Requirements);
            job.benefits = CleanText(model.Benefits);
            job.employment_type = model.EmploymentType;
            job.level = model.Level;
            job.work_mode = model.WorkMode;
            job.city = CleanText(model.City);
            job.address = CleanText(model.Address);
            job.salary_min = model.SalaryMin;
            job.salary_max = model.SalaryMax;
            job.salary_negotiable = model.SalaryNegotiable;
            job.min_experience_years = model.MinExperienceYears;
            job.min_education = CleanText(model.MinEducation);
            job.headcount = (short)model.Headcount;
            job.expires_at = model.ExpiresAt.HasValue ? (DateTime?)EndOfDay(model.ExpiresAt.Value) : null;
        }

        private string GenerateUniqueSlug(string title)
        {
            string baseSlug = SlugHelper.ToSlug(title, 240);
            string candidate = baseSlug;
            int suffix = 2;

            while (_db.jobs.Any(j => j.slug == candidate))
            {
                candidate = baseSlug + "-" + suffix;
                suffix++;
            }
            return candidate;
        }

        // Chỉ giữ lại các skill_id thực sự tồn tại, bỏ trùng
        private List<int> GetValidSkillIds(List<int> requested)
        {
            if (requested == null || requested.Count == 0) return new List<int>();

            var ids = requested.Distinct().ToList();
            return _db.skills
                .Where(s => ids.Contains(s.id))
                .Select(s => s.id)
                .ToList();
        }

        private static string CleanText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        // Hạn nộp chọn theo ngày -> hết hạn vào cuối ngày đó
        private static DateTime EndOfDay(DateTime date)
        {
            return date.Date.AddDays(1).AddSeconds(-1);
        }

        // Làm sạch dữ liệu từ query string: giá trị lạ bị bỏ, không ném lỗi
        private static void NormalizeFilter(JobSearchViewModel model)
        {
            model.Keyword = string.IsNullOrWhiteSpace(model.Keyword) ? null : model.Keyword.Trim();
            if (model.Keyword != null && model.Keyword.Length > MaxKeywordLength)
                model.Keyword = model.Keyword.Substring(0, MaxKeywordLength);

            model.City = string.IsNullOrWhiteSpace(model.City) ? null : model.City.Trim();

            if (!JobLevel.All.Contains(model.Level)) model.Level = null;
            if (!EmploymentType.All.Contains(model.EmploymentType)) model.EmploymentType = null;

            if (model.SalaryMin.HasValue && model.SalaryMin.Value < 0) model.SalaryMin = null;
            if (model.SalaryMax.HasValue && model.SalaryMax.Value < 0) model.SalaryMax = null;

            // Người dùng nhập ngược khoảng lương -> đổi chỗ
            if (model.SalaryMin.HasValue && model.SalaryMax.HasValue && model.SalaryMin.Value > model.SalaryMax.Value)
            {
                int tmp = model.SalaryMin.Value;
                model.SalaryMin = model.SalaryMax;
                model.SalaryMax = tmp;
            }
        }

        private void LoadFilterOptions(JobSearchViewModel model)
        {
            model.CategoryOptions = _db.job_categories
                .AsNoTracking()
                .OrderBy(c => c.name)
                .ToList()
                .Select(c => new SelectListItem
                {
                    Value = c.id.ToString(),
                    Text = c.name,
                    Selected = model.CategoryId == c.id
                })
                .ToList();

            var now = DateTime.Now;
            model.CityOptions = _db.jobs
                .AsNoTracking()
                .Where(j => j.status == JobStatus.Published
                         && (j.expires_at == null || j.expires_at >= now)
                         && j.city != null && j.city != "")
                .Select(j => j.city)
                .Distinct()
                .OrderBy(c => c)
                .ToList()
                .Select(c => new SelectListItem { Value = c, Text = c, Selected = c == model.City })
                .ToList();
        }

        private class JobSkillName
        {
            public long JobId { get; set; }
            public string Name { get; set; }
        }
    }
}
