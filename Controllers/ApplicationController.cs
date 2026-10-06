using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using recruitment_website.Constants;
using recruitment_website.DAL;
using recruitment_website.Filters;
using recruitment_website.Models.Application;
using recruitment_website.Models.Candidate;

namespace recruitment_website.Controllers
{
    public class ApplicationController : BaseController
    {
        private readonly recruitment_dbEntities _db = new recruitment_dbEntities();

        private static readonly string[] AllStatuses =
        {
            ApplicationStatus.Applied, ApplicationStatus.Screening, ApplicationStatus.Shortlisted,
            ApplicationStatus.Interview, ApplicationStatus.Offer, ApplicationStatus.Hired,
            ApplicationStatus.Rejected, ApplicationStatus.Withdrawn
        };

        // =====================================================================
        // PHÍA ỨNG VIÊN
        // =====================================================================

        [HttpGet]
        [AuthorizeRole(UserRole.Candidate)]
        public ActionResult Apply(long jobId)
        {
            var candidate = GetCurrentCandidate();
            if (candidate == null) return RedirectToAction("Edit", "Candidate");

            var job = _db.jobs.Include(j => j.companies).FirstOrDefault(j => j.id == jobId);
            var guard = CheckCanApply(candidate, job);
            if (guard != null) return guard;

            var model = new ApplicationApplyViewModel { JobId = job.id };
            FillApplyViewModel(model, candidate, job);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(UserRole.Candidate)]
        public ActionResult Apply(ApplicationApplyViewModel model)
        {
            var candidate = GetCurrentCandidate();
            if (candidate == null) return RedirectToAction("Edit", "Candidate");

            var job = _db.jobs.Include(j => j.companies).FirstOrDefault(j => j.id == model.JobId);
            var guard = CheckCanApply(candidate, job);
            if (guard != null) return guard;

            // CV phải thuộc về chính ứng viên này, không tin giá trị client gửi lên
            cvs cv = null;
            if (model.CvId.HasValue)
            {
                long cvId = model.CvId.Value;
                long candidateId = candidate.id;
                cv = _db.cvs.FirstOrDefault(c => c.id == cvId && c.candidate_id == candidateId);
                if (cv == null) ModelState.AddModelError("CvId", "CV không hợp lệ.");
            }

            if (!ModelState.IsValid)
            {
                FillApplyViewModel(model, candidate, job);
                return View(model);
            }

            var now = DateTime.Now;
            var app = new applications
            {
                job_id = job.id,
                candidate_id = candidate.id,
                cv_id = cv.id,
                cover_letter = string.IsNullOrWhiteSpace(model.CoverLetter) ? null : model.CoverLetter.Trim(),
                status = ApplicationStatus.Applied,
                applied_at = now,
                updated_at = now
            };
            // Thêm qua navigation để đơn + dòng lịch sử đầu tiên lưu trong cùng 1 SaveChanges (atomic)
            app.application_status_history.Add(new application_status_history
            {
                from_status = null,
                to_status = ApplicationStatus.Applied,
                changed_by = CurrentUserId,
                changed_at = now
            });
            _db.applications.Add(app);

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                // Hầu như chỉ xảy ra khi bấm đúp: vi phạm UNIQUE(job_id, candidate_id)
                TempData["Error"] = "Không thể gửi đơn. Có thể bạn đã ứng tuyển tin này, hãy kiểm tra danh sách đơn.";
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Đã gửi hồ sơ ứng tuyển thành công.";
            return RedirectToAction("Details", new { id = app.id });
        }

        [AuthorizeRole(UserRole.Candidate)]
        public ActionResult Index(string status)
        {
            var candidate = GetCurrentCandidate();
            if (candidate == null) return RedirectToAction("Edit", "Candidate");

            long candidateId = candidate.id;
            var query = _db.applications.Where(a => a.candidate_id == candidateId);
            if (!string.IsNullOrEmpty(status))
                query = query.Where(a => a.status == status);

            var rows = query
                .OrderByDescending(a => a.applied_at)
                .Select(a => new
                {
                    ApplicationId = a.id,
                    JobId = a.job_id,
                    JobTitle = a.jobs.title,
                    CompanyName = a.jobs.companies.name,
                    CvTitle = a.cvs.title,
                    Status = a.status,
                    AppliedAt = a.applied_at,
                    UpdatedAt = a.updated_at
                })
                .ToList();

            var model = new MyApplicationsViewModel
            {
                StatusFilter = status,
                StatusOptions = BuildStatusOptions(status),
                Items = rows.Select(r => new ApplicationListItemViewModel
                {
                    ApplicationId = r.ApplicationId,
                    JobId = r.JobId,
                    JobTitle = r.JobTitle,
                    CompanyName = r.CompanyName,
                    CvTitle = r.CvTitle,
                    Status = r.Status,
                    StatusLabel = ApplicationStatus.Label(r.Status),
                    AppliedAt = r.AppliedAt,
                    UpdatedAt = r.UpdatedAt,
                    CanWithdraw = ApplicationStatus.CanWithdraw(r.Status)
                }).ToList()
            };
            return View(model);
        }

        [AuthorizeRole(UserRole.Candidate)]
        public ActionResult Details(long id)
        {
            var candidate = GetCurrentCandidate();
            if (candidate == null) return RedirectToAction("Edit", "Candidate");

            long candidateId = candidate.id;
            var app = _db.applications
                .Include(a => a.jobs.companies)
                .Include(a => a.cvs)
                .Include(a => a.application_status_history)
                .FirstOrDefault(a => a.id == id && a.candidate_id == candidateId);
            if (app == null) return HttpNotFound();

            var model = new ApplicationDetailViewModel
            {
                ApplicationId = app.id,
                JobId = app.job_id,
                JobTitle = app.jobs.title,
                CompanyName = app.jobs.companies.name,
                CvTitle = app.cvs.title,
                CoverLetter = app.cover_letter,
                Status = app.status,
                StatusLabel = ApplicationStatus.Label(app.status),
                AppliedAt = app.applied_at,
                UpdatedAt = app.updated_at,
                CanWithdraw = ApplicationStatus.CanWithdraw(app.status),
                // Cố ý KHÔNG đưa recruiter_note và note của lịch sử: đó là ghi chú nội bộ của nhà tuyển dụng
                History = app.application_status_history
                    .OrderByDescending(h => h.changed_at)
                    .Select(h => new ApplicationHistoryItemViewModel
                    {
                        FromStatusLabel = h.from_status == null ? "" : ApplicationStatus.Label(h.from_status),
                        ToStatusLabel = ApplicationStatus.Label(h.to_status),
                        ChangedAt = h.changed_at
                    }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(UserRole.Candidate)]
        public ActionResult Withdraw(long id)
        {
            var candidate = GetCurrentCandidate();
            if (candidate == null) return RedirectToAction("Edit", "Candidate");

            long candidateId = candidate.id;
            var app = _db.applications.FirstOrDefault(a => a.id == id && a.candidate_id == candidateId);
            if (app == null) return HttpNotFound();

            if (!ApplicationStatus.CanWithdraw(app.status))
            {
                TempData["Error"] = "Đơn này không thể rút ở trạng thái hiện tại.";
                return RedirectToAction("Details", new { id });
            }

            ApplyStatus(app, ApplicationStatus.Withdrawn, null);
            _db.SaveChanges();

            TempData["Success"] = "Đã rút đơn ứng tuyển.";
            return RedirectToAction("Index");
        }

        // =====================================================================
        // PHÍA NHÀ TUYỂN DỤNG
        // =====================================================================

        // GET: Application/ByJob?jobId=5&status=applied
        [AuthorizeRole(UserRole.Employer)]
        public ActionResult ByJob(long jobId, string status = null)
        {
            long? companyId = GetEmployerCompanyId();
            if (companyId == null) return RedirectNoCompany();

            // Tin của công ty khác -> 404 (không lộ là tin có tồn tại)
            var job = _db.jobs.FirstOrDefault(j => j.id == jobId && j.company_id == companyId.Value);
            if (job == null) return HttpNotFound();

            if (!AllStatuses.Contains(status)) status = null;

            var all = _db.applications.Where(a => a.job_id == jobId);
            var counts = all.GroupBy(a => a.status)
                            .Select(g => new { Status = g.Key, Count = g.Count() })
                            .ToList();

            var query = status == null ? all : all.Where(a => a.status == status);
            var items = query
                .OrderByDescending(a => a.applied_at)
                .Select(a => new
                {
                    a.id,
                    a.status,
                    a.applied_at,
                    HasNote = a.recruiter_note != null,
                    a.candidates.full_name,
                    a.candidates.headline,
                    a.candidates.city,
                    a.candidates.total_experience_years
                })
                .ToList()
                .Select(x => new ApplicantListItemViewModel
                {
                    ApplicationId = x.id,
                    CandidateName = x.full_name,
                    Headline = x.headline,
                    City = x.city,
                    TotalExperienceYears = x.total_experience_years,
                    Status = x.status,
                    StatusLabel = ApplicationStatus.Label(x.status),
                    AppliedAt = x.applied_at,
                    HasNote = x.HasNote
                }).ToList();

            var vm = new JobApplicantsViewModel
            {
                JobId = job.id,
                JobTitle = job.title,
                TotalCount = counts.Sum(c => c.Count),
                CurrentStatus = status ?? "",
                Items = items
            };
            vm.StatusFilters.Add(new ApplicantStatusFilterViewModel
            {
                Value = "",
                Label = "Tất cả",
                Count = vm.TotalCount,
                IsActive = status == null
            });
            foreach (var s in AllStatuses)
            {
                vm.StatusFilters.Add(new ApplicantStatusFilterViewModel
                {
                    Value = s,
                    Label = ApplicationStatus.Label(s),
                    Count = counts.Where(c => c.Status == s).Select(c => c.Count).FirstOrDefault(),
                    IsActive = status == s
                });
            }
            return View(vm);
        }

        // GET: Application/Review/5   (id = application id)
        [AuthorizeRole(UserRole.Employer)]
        public ActionResult Review(long id)
        {
            long? companyId = GetEmployerCompanyId();
            if (companyId == null) return RedirectNoCompany();

            var app = FindEmployerApplication(id, companyId.Value, withDetails: true);
            if (app == null) return HttpNotFound();

            var c = app.candidates;

            var skills = _db.candidate_skills
                .Where(cs => cs.candidate_id == c.id)
                .OrderByDescending(cs => cs.proficiency)
                .Select(cs => new { cs.skills.name, cs.proficiency, cs.years_experience })
                .ToList()
                .Select(x => new ApplicantSkillViewModel
                {
                    Name = x.name,
                    Proficiency = x.proficiency,
                    YearsExperience = x.years_experience
                }).ToList();

            var experiences = _db.candidate_experiences
                .Where(e => e.candidate_id == c.id)
                .OrderByDescending(e => e.start_date)
                .ToList()
                .Select(e => new ExperienceItemViewModel
                {
                    Id = e.id,
                    CompanyName = e.company_name,
                    JobTitle = e.job_title,
                    StartDate = e.start_date,
                    EndDate = e.end_date,
                    Description = e.description
                }).ToList();

            var educations = _db.candidate_educations
                .Where(e => e.candidate_id == c.id)
                .OrderByDescending(e => e.end_year)
                .ToList()
                .Select(e => new EducationItemViewModel
                {
                    Id = e.id,
                    SchoolName = e.school_name,
                    Major = e.major,
                    DegreeLabel = EducationLevel.Label(e.degree),
                    StartYear = e.start_year,
                    EndYear = e.end_year
                }).ToList();

            var history = _db.application_status_history
                .Where(h => h.application_id == app.id)
                .OrderByDescending(h => h.changed_at)
                .ToList()
                .Select(h => new ApplicantHistoryRowViewModel
                {
                    FromLabel = h.from_status == null ? "" : ApplicationStatus.Label(h.from_status),
                    ToLabel = ApplicationStatus.Label(h.to_status),
                    ChangedAt = h.changed_at
                }).ToList();

            var vm = new ApplicationReviewViewModel
            {
                Id = app.id,
                JobId = app.job_id,
                JobTitle = app.jobs.title,
                AppliedAt = app.applied_at,
                Status = app.status,
                StatusLabel = ApplicationStatus.Label(app.status),
                CoverLetter = app.cover_letter,
                CvTitle = app.cvs != null ? app.cvs.title : null,
                CandidateName = c.full_name,
                Email = c.users != null ? c.users.email : null,
                Phone = c.phone,
                City = c.city,
                Headline = c.headline,
                Summary = c.summary,
                TotalExperienceYears = c.total_experience_years,
                HighestEducationLabel = EducationLevel.Label(c.highest_education),
                ExpectedSalary = c.expected_salary,
                Skills = skills,
                Experiences = experiences,
                Educations = educations,
                NextStatuses = ApplicationStatus.NextOptions(app.status)
                    .Select(s => new ApplicantNextStatusViewModel
                    {
                        Value = s,
                        Label = ApplicationStatus.Label(s),
                        IsDanger = s == ApplicationStatus.Rejected
                    }).ToList(),
                RecruiterNote = app.recruiter_note,
                History = history
            };
            return View(vm);
        }

        // GET: Application/DownloadCv/5   (id = application id, KHÔNG phải cv id)
        [AuthorizeRole(UserRole.Employer)]
        public ActionResult DownloadCv(long id)
        {
            long? companyId = GetEmployerCompanyId();
            if (companyId == null) return RedirectNoCompany();

            var app = FindEmployerApplication(id, companyId.Value, withDetails: true);
            if (app == null || app.cvs == null) return HttpNotFound();

            string physicalPath = Server.MapPath(app.cvs.file_url);
            if (!System.IO.File.Exists(physicalPath)) return HttpNotFound();

            string ext = Path.GetExtension(physicalPath).ToLowerInvariant();
            string contentType = ext == ".pdf" ? "application/pdf"
                               : ext == ".docx" ? "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                               : "application/msword";
            string safeTitle = string.Concat(app.cvs.title.Split(Path.GetInvalidFileNameChars()));
            return File(physicalPath, contentType, safeTitle + ext);
        }

        // POST: Application/ChangeStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(UserRole.Employer)]
        public ActionResult ChangeStatus(long id, string newStatus)
        {
            long? companyId = GetEmployerCompanyId();
            if (companyId == null) return RedirectNoCompany();

            var app = FindEmployerApplication(id, companyId.Value);
            if (app == null) return HttpNotFound();

            // Luật chuyển trạng thái kiểm tra trên status HIỆN TẠI trong DB, không tin dữ liệu form
            if (!ApplicationStatus.CanEmployerChange(app.status, newStatus))
            {
                TempData["Error"] = "Không thể chuyển từ \"" + ApplicationStatus.Label(app.status)
                                  + "\" sang trạng thái này.";
                return RedirectToAction("Review", new { id });
            }

            // note = null: ghi chú nội bộ chỉ đi qua SaveNote, tránh rò sang phía ứng viên qua lịch sử
            ApplyStatus(app, newStatus, null);
            _db.SaveChanges();

            TempData["Success"] = "Đã chuyển hồ sơ sang \"" + ApplicationStatus.Label(newStatus) + "\".";
            return RedirectToAction("Review", new { id });
        }

        // POST: Application/SaveNote
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(UserRole.Employer)]
        public ActionResult SaveNote(long id, string note)
        {
            long? companyId = GetEmployerCompanyId();
            if (companyId == null) return RedirectNoCompany();

            var app = FindEmployerApplication(id, companyId.Value);
            if (app == null) return HttpNotFound();

            note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
            if (note != null && note.Length > 2000)
            {
                TempData["Error"] = "Ghi chú tối đa 2000 ký tự.";
                return RedirectToAction("Review", new { id });
            }

            app.recruiter_note = note;
            app.updated_at = DateTime.Now;
            _db.SaveChanges();

            TempData["Success"] = "Đã lưu ghi chú nội bộ.";
            return RedirectToAction("Review", new { id });
        }

        // =====================================================================
        // HELPER DÙNG CHUNG
        // =====================================================================

        /// <summary>
        /// Đổi trạng thái + chuẩn bị dòng lịch sử. CHƯA gọi SaveChanges: nơi gọi tự SaveChanges
        /// để đơn và lịch sử lưu cùng 1 lần.
        /// </summary>
        private void ApplyStatus(applications app, string newStatus, string note)
        {
            var from = app.status; // đọc status cũ TRƯỚC khi gán
            if (from == newStatus) return;

            var now = DateTime.Now;
            app.status = newStatus;
            app.updated_at = now;

            _db.application_status_history.Add(new application_status_history
            {
                application_id = app.id,
                from_status = from,
                to_status = newStatus,
                changed_by = CurrentUserId,
                note = note,
                changed_at = now
            });
        }

        // ---- helper phía ứng viên ----

        private candidates GetCurrentCandidate()
        {
            long userId = CurrentUserId.Value; // BaseController đã chặn người chưa đăng nhập
            return _db.candidates.FirstOrDefault(c => c.user_id == userId);
        }

        private static bool IsJobOpen(jobs job)
        {
            return job != null
                && job.status == JobStatus.Published
                && (!job.expires_at.HasValue || job.expires_at.Value > DateTime.Now);
        }

        private static List<string> MissingProfileFields(candidates c)
        {
            var missing = new List<string>();
            if (string.IsNullOrWhiteSpace(c.phone)) missing.Add("số điện thoại");
            if (string.IsNullOrWhiteSpace(c.city)) missing.Add("thành phố");
            return missing;
        }

        /// <summary>Trả về ActionResult chuyển hướng nếu KHÔNG đủ điều kiện ứng tuyển, null nếu hợp lệ.</summary>
        private ActionResult CheckCanApply(candidates candidate, jobs job)
        {
            if (!IsJobOpen(job))
            {
                TempData["Error"] = "Tin tuyển dụng không tồn tại hoặc đã ngừng nhận hồ sơ.";
                return RedirectToAction("Index", "SavedJob");
            }

            long candidateId = candidate.id;
            long jobId = job.id;
            if (_db.applications.Any(a => a.job_id == jobId && a.candidate_id == candidateId))
            {
                TempData["Error"] = "Bạn đã ứng tuyển tin này rồi.";
                return RedirectToAction("Index");
            }

            var missing = MissingProfileFields(candidate);
            if (missing.Count > 0)
            {
                TempData["Error"] = "Vui lòng bổ sung " + string.Join(", ", missing) + " trong hồ sơ trước khi ứng tuyển.";
                return RedirectToAction("Edit", "Candidate");
            }

            if (!_db.cvs.Any(c => c.candidate_id == candidateId))
            {
                TempData["Error"] = "Bạn cần tải lên ít nhất một CV trước khi ứng tuyển.";
                return RedirectToAction("Cvs", "Candidate");
            }

            return null;
        }

        private void FillApplyViewModel(ApplicationApplyViewModel model, candidates candidate, jobs job)
        {
            long candidateId = candidate.id;
            model.JobId = job.id;
            model.JobTitle = job.title;
            model.CompanyName = job.companies.name;
            model.Cvs = _db.cvs
                .Where(c => c.candidate_id == candidateId)
                .OrderByDescending(c => c.is_default)
                .ThenByDescending(c => c.uploaded_at)
                .Select(c => new CvOptionViewModel
                {
                    Id = c.id,
                    Title = c.title,
                    UploadedAt = c.uploaded_at,
                    IsDefault = c.is_default
                }).ToList();

            if (!model.CvId.HasValue)
            {
                var def = model.Cvs.FirstOrDefault(c => c.IsDefault) ?? model.Cvs.FirstOrDefault();
                if (def != null) model.CvId = def.Id;
            }
        }

        private static List<SelectListItem> BuildStatusOptions(string selected)
        {
            return AllStatuses.Select(s => new SelectListItem
            {
                Value = s,
                Text = ApplicationStatus.Label(s),
                Selected = s == selected
            }).ToList();
        }

        // ---- helper phía nhà tuyển dụng ----

        private long? GetEmployerCompanyId()
        {
            long userId = CurrentUserId.GetValueOrDefault();
            return _db.company_members
                .Where(m => m.user_id == userId)
                .Select(m => (long?)m.company_id)
                .FirstOrDefault();
        }

        // Điều kiện a.jobs.company_id == companyId chính là kiểm tra "đơn thuộc công ty mình"
        private applications FindEmployerApplication(long id, long companyId, bool withDetails = false)
        {
            IQueryable<applications> q = _db.applications;
            if (withDetails)
            {
                q = q.Include(a => a.jobs)
                     .Include(a => a.cvs)
                     .Include(a => a.candidates.users);
            }
            return q.FirstOrDefault(a => a.id == id && a.jobs.company_id == companyId);
        }

        private ActionResult RedirectNoCompany()
        {
            TempData["Error"] = "Tài khoản của bạn chưa thuộc công ty nào.";
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}