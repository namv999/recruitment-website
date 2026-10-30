using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using recruitment_website.Constants;
using recruitment_website.DAL;
using recruitment_website.Filters;
using recruitment_website.Models.Admin;
using recruitment_website.Services;

namespace recruitment_website.Controllers
{
    [AuthorizeRole(UserRole.Admin)]
    public class AdminController : BaseController
    {
        private const int PageSize = 20;
        private readonly recruitment_dbEntities _db = new recruitment_dbEntities();

        [HttpGet]
        public ActionResult Index()
        {
            var model = new AdminDashboardViewModel
            {
                TotalUsers = _db.users.Count(u => u.role == UserRole.Candidate || u.role == UserRole.Employer),
                ActiveUsers = _db.users.Count(u => (u.role == UserRole.Candidate || u.role == UserRole.Employer)
                    && u.status == UserStatus.Active),
                PendingCompanies = _db.companies.Count(c => !c.is_verified),
                PendingJobs = _db.jobs.Count(j => j.status == JobStatus.Pending),
                TotalSkills = _db.skills.Count(),
                TotalJobCategories = _db.job_categories.Count()
            };
            return View(model);
        }

        [HttpGet]
        public ActionResult Users(string keyword, string role, string status, int page = 1)
        {
            var query = _db.users.AsNoTracking()
                .Where(u => u.role == UserRole.Candidate || u.role == UserRole.Employer);
            if (!string.IsNullOrWhiteSpace(keyword)) query = query.Where(u => u.email.Contains(keyword.Trim()));
            if (role == UserRole.Candidate || role == UserRole.Employer)
                query = query.Where(u => u.role == role);
            if (status == UserStatus.Active || status == UserStatus.Inactive)
                query = query.Where(u => u.status == status);

            int total = query.Count();
            int totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
            int currentPage = Math.Min(Math.Max(1, page), totalPages);
            var model = new AdminUsersViewModel
            {
                Keyword = keyword,
                Role = role,
                Status = status,
                Page = currentPage,
                TotalItems = total,
                TotalPages = totalPages,
                Users = query.OrderByDescending(u => u.created_at).ThenBy(u => u.id)
                    .Skip((currentPage - 1) * PageSize).Take(PageSize)
                    .Select(u => new AdminUserItemViewModel
                    {
                        Id = u.id, Email = u.email, Role = u.role, Status = u.status,
                        CreatedAt = u.created_at, LastLoginAt = u.last_login_at,
                        CanChangeStatus = u.role != UserRole.Admin && u.id != (CurrentUserId ?? 0)
                    }).ToList()
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ToggleUserStatus(long id)
        {
            var user = _db.users.Find(id);
            if (user == null) return HttpNotFound();
            if (user.role == UserRole.Admin || id == CurrentUserId)
            {
                TempData["Error"] = "Không thể khóa tài khoản quản trị viên hoặc chính tài khoản đang đăng nhập.";
                return RedirectToAction("Users");
            }
            if (user.status != UserStatus.Active && user.status != UserStatus.Inactive)
            {
                TempData["Error"] = "Trạng thái tài khoản hiện tại không thể thay đổi tại đây.";
                return RedirectToAction("Users");
            }

            user.status = user.status == UserStatus.Active ? UserStatus.Inactive : UserStatus.Active;
            user.updated_at = DateTime.Now;
            _db.SaveChanges();
            TempData["Success"] = user.status == UserStatus.Active ? "Đã mở khóa tài khoản." : "Đã khóa tài khoản.";
            return RedirectToAction("Users");
        }

        [HttpGet]
        public ActionResult Companies(bool? verified)
        {
            var query = _db.companies.AsNoTracking().AsQueryable();
            if (verified.HasValue) query = query.Where(c => c.is_verified == verified.Value);
            return View(new AdminCompaniesViewModel
            {
                Verified = verified,
                Companies = query.OrderBy(c => c.is_verified).ThenBy(c => c.name)
                    .Select(c => new AdminCompanyItemViewModel
                    {
                        Id = c.id, Name = c.name, TaxCode = c.tax_code, City = c.city,
                        IsVerified = c.is_verified, JobCount = c.jobs.Count()
                    }).ToList()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ToggleCompanyVerification(long id)
        {
            var company = _db.companies.Find(id);
            if (company == null) return HttpNotFound();
            company.is_verified = !company.is_verified;
            company.updated_at = DateTime.Now;
            if (!company.is_verified)
            {
                foreach (var job in _db.jobs.Where(j => j.company_id == id && j.status == JobStatus.Published))
                {
                    job.status = JobStatus.Pending;
                    job.updated_at = DateTime.Now;
                }
            }
            _db.SaveChanges();
            TempData["Success"] = company.is_verified ? "Đã xác minh công ty." : "Đã gỡ xác minh công ty.";
            return RedirectToAction("Companies");
        }

        [HttpGet]
        public ActionResult Skills(string keyword)
        {
            IQueryable<skills> query = _db.skills.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(keyword)) query = query.Where(s => s.name.Contains(keyword.Trim()));
            var model = new AdminSkillsViewModel
            {
                Keyword = keyword,
                Categories = SkillCategoryOptions(),
                Skills = query.OrderBy(s => s.name).Select(s => new AdminSkillItemViewModel
                {
                    Id = s.id, Name = s.name,
                    CategoryId = s.category_id,
                    CategoryName = s.skill_categories == null ? "Chưa phân nhóm" : s.skill_categories.name,
                    CandidateUsageCount = s.candidate_skills.Count(), JobUsageCount = s.job_skills.Count()
                }).ToList()
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveSkill(AdminSkillFormViewModel model)
        {
            if (model.CategoryId.HasValue && !_db.skill_categories.Any(c => c.id == model.CategoryId.Value))
                ModelState.AddModelError("CategoryId", "Nhóm kỹ năng không tồn tại.");
            string name = (model.Name ?? "").Trim();
            if (_db.skills.Any(s => s.name.ToLower() == name.ToLower() && (!model.Id.HasValue || s.id != model.Id.Value)))
                ModelState.AddModelError("Name", "Tên kỹ năng đã tồn tại.");
            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return RedirectToAction("Skills");
            }

            var skill = model.Id.HasValue ? _db.skills.Find(model.Id.Value) : new skills();
            if (skill == null) return HttpNotFound();
            skill.name = name;
            skill.category_id = model.CategoryId;
            skill.slug = UniqueSkillSlug(name, model.Id);
            if (!model.Id.HasValue) _db.skills.Add(skill);
            _db.SaveChanges();
            TempData["Success"] = model.Id.HasValue ? "Đã cập nhật kỹ năng." : "Đã thêm kỹ năng.";
            return RedirectToAction("Skills");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DeleteSkill(int id)
        {
            var skill = _db.skills.Find(id);
            if (skill == null) return HttpNotFound();
            if (_db.candidate_skills.Any(cs => cs.skill_id == id)
                || _db.job_skills.Any(js => js.skill_id == id)
                || _db.skill_aliases.Any(sa => sa.skill_id == id))
            {
                TempData["Error"] = "Kỹ năng đang được hồ sơ, tin tuyển dụng hoặc bí danh sử dụng nên không thể xóa.";
                return RedirectToAction("Skills");
            }
            _db.skills.Remove(skill);
            _db.SaveChanges();
            TempData["Success"] = "Đã xóa kỹ năng.";
            return RedirectToAction("Skills");
        }

        [HttpGet]
        public ActionResult Categories()
        {
            return View(new AdminCategoriesViewModel
            {
                SkillCategories = _db.skill_categories.AsNoTracking().OrderBy(c => c.name)
                    .Select(c => new AdminCategoryItemViewModel { Id = c.id, Name = c.name, UsageCount = c.skills.Count() }).ToList(),
                JobCategories = _db.job_categories.AsNoTracking().OrderBy(c => c.name)
                    .Select(c => new AdminCategoryItemViewModel { Id = c.id, Name = c.name, UsageCount = c.jobs.Count() }).ToList()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveSkillCategory(int? id, string name)
        {
            name = (name ?? "").Trim();
            if (name.Length == 0 || name.Length > 100) return InvalidCategory("Tên nhóm kỹ năng phải có từ 1 đến 100 ký tự.");
            if (_db.skill_categories.Any(c => c.name.ToLower() == name.ToLower() && (!id.HasValue || c.id != id.Value)))
                return InvalidCategory("Tên nhóm kỹ năng đã tồn tại.");
            var category = id.HasValue ? _db.skill_categories.Find(id.Value) : new skill_categories();
            if (category == null) return HttpNotFound();
            category.name = name;
            if (!id.HasValue) _db.skill_categories.Add(category);
            _db.SaveChanges();
            TempData["Success"] = "Đã lưu nhóm kỹ năng.";
            return RedirectToAction("Categories");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DeleteSkillCategory(int id)
        {
            var category = _db.skill_categories.Find(id);
            if (category == null) return HttpNotFound();
            if (_db.skills.Any(s => s.category_id == id)) return InvalidCategory("Nhóm đang có kỹ năng; hãy chuyển hoặc xóa kỹ năng trước.");
            _db.skill_categories.Remove(category);
            _db.SaveChanges();
            TempData["Success"] = "Đã xóa nhóm kỹ năng.";
            return RedirectToAction("Categories");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveJobCategory(int? id, string name)
        {
            name = (name ?? "").Trim();
            if (name.Length == 0 || name.Length > 150) return InvalidCategory("Tên ngành nghề phải có từ 1 đến 150 ký tự.");
            if (_db.job_categories.Any(c => c.name.ToLower() == name.ToLower() && (!id.HasValue || c.id != id.Value)))
                return InvalidCategory("Tên ngành nghề đã tồn tại.");
            var category = id.HasValue ? _db.job_categories.Find(id.Value) : new job_categories();
            if (category == null) return HttpNotFound();
            category.name = name;
            category.slug = UniqueJobCategorySlug(name, id);
            if (!id.HasValue) _db.job_categories.Add(category);
            _db.SaveChanges();
            TempData["Success"] = "Đã lưu danh mục ngành nghề.";
            return RedirectToAction("Categories");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DeleteJobCategory(int id)
        {
            var category = _db.job_categories.Find(id);
            if (category == null) return HttpNotFound();
            if (_db.jobs.Any(j => j.category_id == id)) return InvalidCategory("Danh mục đang được tin tuyển dụng sử dụng nên không thể xóa.");
            _db.job_categories.Remove(category);
            _db.SaveChanges();
            TempData["Success"] = "Đã xóa danh mục ngành nghề.";
            return RedirectToAction("Categories");
        }

        [HttpGet]
        public ActionResult PendingJobs()
        {
            var model = new AdminJobsViewModel
            {
                Jobs = _db.jobs.AsNoTracking().Where(j => j.status == JobStatus.Pending)
                    .OrderBy(j => j.created_at).Select(j => new AdminJobItemViewModel
                    {
                        Id = j.id, Title = j.title, CompanyName = j.companies.name,
                        CreatedAt = j.created_at, ApplicationCount = j.applications.Count()
                    }).ToList()
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ApproveJob(long id)
        {
            var job = _db.jobs.Find(id);
            if (job == null) return HttpNotFound();
            if (job.status != JobStatus.Pending)
            {
                TempData["Error"] = "Tin không còn ở trạng thái chờ duyệt.";
                return RedirectToAction("PendingJobs");
            }
            if (!job.companies.is_verified)
            {
                TempData["Error"] = "Cần xác minh công ty trước khi duyệt tin.";
                return RedirectToAction("PendingJobs");
            }
            job.status = JobStatus.Published;
            job.published_at = DateTime.Now;
            job.updated_at = DateTime.Now;
            _db.SaveChanges();
            TempData["Success"] = "Đã duyệt và đăng tin tuyển dụng.";
            return RedirectToAction("PendingJobs");
        }

        private SelectList SkillCategoryOptions(int? selected = null)
        {
            return new SelectList(_db.skill_categories.AsNoTracking().OrderBy(c => c.name).ToList(), "id", "name", selected);
        }

        private string UniqueSkillSlug(string name, int? currentId)
        {
            string baseSlug = SlugHelper.ToSlug(name, 90);
            string candidate = baseSlug;
            int suffix = 2;
            while (_db.skills.Any(s => s.slug == candidate && (!currentId.HasValue || s.id != currentId.Value)))
            {
                string ending = "-" + suffix++;
                candidate = baseSlug.Substring(0, Math.Min(baseSlug.Length, 100 - ending.Length)) + ending;
            }
            return candidate;
        }

        private string UniqueJobCategorySlug(string name, int? currentId)
        {
            string baseSlug = SlugHelper.ToSlug(name, 90);
            string candidate = baseSlug;
            int suffix = 2;
            while (_db.job_categories.Any(c => c.slug == candidate && (!currentId.HasValue || c.id != currentId.Value)))
            {
                string ending = "-" + suffix++;
                candidate = baseSlug.Substring(0, Math.Min(baseSlug.Length, 100 - ending.Length)) + ending;
            }
            return candidate;
        }

        private ActionResult InvalidCategory(string message)
        {
            TempData["Error"] = message;
            return RedirectToAction("Categories");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
