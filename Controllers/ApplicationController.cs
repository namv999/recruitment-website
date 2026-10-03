using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using recruitment_website.Constants;
using recruitment_website.DAL;
using recruitment_website.Filters;
using recruitment_website.Models.Application;

namespace recruitment_website.Controllers
{
    public class ApplicationController : BaseController
    {
        private readonly recruitment_dbEntities _db = new recruitment_dbEntities();

        // ===== PHÍA ỨNG VIÊN =====

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

            ChangeStatus(app, ApplicationStatus.Withdrawn, null);
            _db.SaveChanges();

            TempData["Success"] = "Đã rút đơn ứng tuyển.";
            return RedirectToAction("Index");
        }

        // ===== HELPER DÙNG CHUNG (phía nhà tuyển dụng sẽ dùng lại ChangeStatus) =====

        /// <summary>
        /// Đổi trạng thái + chuẩn bị dòng lịch sử. CHƯA gọi SaveChanges: nơi gọi tự SaveChanges
        /// để đơn và lịch sử lưu cùng 1 lần.
        /// </summary>
        private void ChangeStatus(applications app, string newStatus, string note)
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
            var all = new[]
            {
                ApplicationStatus.Applied, ApplicationStatus.Screening, ApplicationStatus.Shortlisted,
                ApplicationStatus.Interview, ApplicationStatus.Offer, ApplicationStatus.Hired,
                ApplicationStatus.Rejected, ApplicationStatus.Withdrawn
            };
            return all.Select(s => new SelectListItem
            {
                Value = s,
                Text = ApplicationStatus.Label(s),
                Selected = s == selected
            }).ToList();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}