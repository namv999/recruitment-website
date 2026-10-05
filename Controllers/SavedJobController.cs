using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using recruitment_website.Constants;
using recruitment_website.DAL;
using recruitment_website.Filters;
using recruitment_website.Models.SavedJob;

namespace recruitment_website.Controllers
{
    [AuthorizeRole(UserRole.Candidate)]
    public class SavedJobController : BaseController
    {
        private readonly recruitment_dbEntities _db = new recruitment_dbEntities();

        public ActionResult Index()
        {
            var candidate = GetCurrentCandidate();
            if (candidate == null) return RedirectToAction("Edit", "Candidate");

            long candidateId = candidate.id;
            var rows = _db.saved_jobs
                .Where(s => s.candidate_id == candidateId)
                .OrderByDescending(s => s.saved_at)
                .Select(s => new
                {
                    s.job_id,
                    s.saved_at,
                    Title = s.jobs.title,
                    CompanyName = s.jobs.companies.name,
                    City = s.jobs.city,
                    SalaryMin = s.jobs.salary_min,
                    SalaryMax = s.jobs.salary_max,
                    Negotiable = s.jobs.salary_negotiable,
                    JobStatus = s.jobs.status,
                    ExpiresAt = s.jobs.expires_at
                })
                .ToList();

            var appliedJobIds = _db.applications
                .Where(a => a.candidate_id == candidateId)
                .Select(a => a.job_id)
                .ToList();

            var now = DateTime.Now;
            var model = rows.Select(r => new SavedJobItemViewModel
            {
                JobId = r.job_id,
                Title = r.Title,
                CompanyName = r.CompanyName,
                City = r.City,
                SalaryText = FormatSalary(r.SalaryMin, r.SalaryMax, r.Negotiable),
                SavedAt = r.saved_at,
                IsOpen = r.JobStatus == JobStatus.Published && (!r.ExpiresAt.HasValue || r.ExpiresAt.Value > now),
                HasApplied = appliedJobIds.Contains(r.job_id)
            }).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Toggle(long jobId, string returnUrl)
        {
            var candidate = GetCurrentCandidate();
            if (candidate == null)
                return Respond(false, false, "Bạn cần hoàn thiện hồ sơ trước khi lưu tin.", returnUrl);

            long candidateId = candidate.id;
            var existing = _db.saved_jobs.FirstOrDefault(s => s.candidate_id == candidateId && s.job_id == jobId);
            bool saved;

            if (existing != null)
            {
                _db.saved_jobs.Remove(existing); // bỏ lưu: luôn cho phép, kể cả tin đã đóng
                saved = false;
            }
            else
            {
                var job = _db.jobs.FirstOrDefault(j => j.id == jobId);
                if (job == null || job.status != JobStatus.Published)
                    return Respond(false, false, "Tin tuyển dụng không tồn tại hoặc đã đóng.", returnUrl);

                _db.saved_jobs.Add(new saved_jobs
                {
                    candidate_id = candidateId,
                    job_id = jobId,
                    saved_at = DateTime.Now
                });
                saved = true;
            }

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                // Bấm đúp: trùng khóa chính (candidate_id, job_id)
                return Respond(false, !saved, "Không thể cập nhật, vui lòng thử lại.", returnUrl);
            }

            return Respond(true, saved, saved ? "Đã lưu tin." : "Đã bỏ lưu tin.", returnUrl);
        }

        private ActionResult Respond(bool success, bool saved, string message, string returnUrl)
        {
            if (Request.IsAjaxRequest())
                return Json(new { success = success, saved = saved, message = message });

            if (success) TempData["Success"] = message; else TempData["Error"] = message;

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl); // chỉ nhận URL nội bộ, chống open redirect
            return RedirectToAction("Index");
        }

        private candidates GetCurrentCandidate()
        {
            long userId = CurrentUserId.Value;
            return _db.candidates.FirstOrDefault(c => c.user_id == userId);
        }

        private static string FormatSalary(int? min, int? max, bool negotiable)
        {
            if (negotiable) return "Thỏa thuận";
            if (min.HasValue && max.HasValue)
                return (min.Value / 1000000m).ToString("0.##") + " - " + (max.Value / 1000000m).ToString("0.##") + " triệu";
            if (min.HasValue) return "Từ " + (min.Value / 1000000m).ToString("0.##") + " triệu";
            if (max.HasValue) return "Đến " + (max.Value / 1000000m).ToString("0.##") + " triệu";
            return "";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}