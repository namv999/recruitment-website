using DoAnLapTrinhWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DoAnLapTrinhWeb.Controllers
{
    // =====================================================================
    // Ghi chú: Controller này giả định class Job/Company/JobCategory/Skill
    // (trong Models) đã có đủ các thuộc tính tương ứng với lược đồ trong
    // db_recruitment_1.md (đặt tên theo PascalCase, ví dụ salary_min -> SalaryMin).
    // Các thuộc tính "đếm ngược" (created_at/updated_at/slug/posted_by) chưa
    // được dùng vì chưa xuất hiện trong TuyenDungController/View gốc, nên
    // controller này KHÔNG đụng tới để tránh lệch với Model thật của nhóm.
    // =====================================================================
    public class JobController : Controller
    {
        // Khai báo Database dùng chung cho toàn bộ Controller (giống mẫu TuyenDungController)
        Database db = new Database();

        // =====================================================
        // 1. DANH SÁCH — quản lý toàn bộ tin tuyển dụng (mọi trạng thái, không chỉ "published")
        // =====================================================
        public ActionResult Index()
        {
            List<Job> dsJob = db.dsJob.OrderByDescending(j => j.Id).ToList();

            return View(dsJob);
        }

        // =====================================================
        // 2. CHI TIẾT
        // =====================================================
        public ActionResult Details(long id)
        {
            Job job = db.dsJob.FirstOrDefault(j => j.Id == id);

            if (job == null)
            {
                return HttpNotFound();
            }

            List<Skill> dsKyNangYeuCau =
                (from js in db.dsJobSkill
                 join s in db.dsSkill on js.SkillId equals s.Id
                 where js.JobId == id
                 select s).ToList();

            ViewBag.DsKyNangYeuCau = dsKyNangYeuCau;

            return View(job);
        }

        // =====================================================
        // 3. ĐĂNG TIN MỚI — GET: hiển thị form / POST: lưu tin tuyển dụng
        // =====================================================
        public ActionResult Create()
        {
            ViewBag.DsCongTy = db.dsCompany;
            ViewBag.DsDanhMuc = db.dsJobCategory;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Job job)
        {
            if (string.IsNullOrWhiteSpace(job.Title) || job.CompanyId == 0)
            {
                TempData["Loi"] = "Vui lòng nhập tên vị trí và chọn công ty đăng tin.";
                ViewBag.DsCongTy = db.dsCompany;
                ViewBag.DsDanhMuc = db.dsJobCategory;
                return View(job);
            }

            // Lấy tên công ty / danh mục để lưu kèm (denormalize) — giống cách
            // Job.CompanyName / Job.CategoryName đang được hiển thị trực tiếp
            // trong DanhSachJob.cshtml / ChiTietJob.cshtml mà không cần join.
            Company congTy = db.dsCompany.FirstOrDefault(c => c.Id == job.CompanyId);
            JobCategory danhMuc = db.dsJobCategory.FirstOrDefault(c => c.Id == job.CategoryId);

            job.Id = db.dsJob.Count > 0 ? db.dsJob.Max(j => j.Id) + 1 : 1;
            job.CompanyName = congTy?.Name;
            job.CompanyLogo = congTy?.LogoUrl;
            job.CategoryName = danhMuc?.Name;
            job.ViewCount = 0;

            if (job.Status == "published")
            {
                job.PublishedAt = DateTime.Now;
            }

            db.dsJob.Add(job);

            // Lưu Job vào SQL Server: bổ sung phương thức db.InsertJob(job) vào class Database.

            return RedirectToAction("Details", new { id = job.Id });
        }

        // =====================================================
        // 4. CẬP NHẬT — GET: hiển thị form / POST: lưu thay đổi
        // =====================================================
        public ActionResult Edit(long id)
        {
            Job job = db.dsJob.FirstOrDefault(j => j.Id == id);

            if (job == null)
            {
                return HttpNotFound();
            }

            ViewBag.DsCongTy = db.dsCompany;
            ViewBag.DsDanhMuc = db.dsJobCategory;

            return View(job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(long id, Job model)
        {
            Job job = db.dsJob.FirstOrDefault(j => j.Id == id);

            if (job == null)
            {
                return HttpNotFound();
            }

            if (string.IsNullOrWhiteSpace(model.Title) || model.CompanyId == 0)
            {
                TempData["Loi"] = "Vui lòng nhập tên vị trí và chọn công ty đăng tin.";
                ViewBag.DsCongTy = db.dsCompany;
                ViewBag.DsDanhMuc = db.dsJobCategory;
                model.Id = id;
                return View(model);
            }

            Company congTy = db.dsCompany.FirstOrDefault(c => c.Id == model.CompanyId);
            JobCategory danhMuc = db.dsJobCategory.FirstOrDefault(c => c.Id == model.CategoryId);
            bool moiChuyenSangPublished = job.Status != "published" && model.Status == "published";

            job.Title = model.Title;
            job.CompanyId = model.CompanyId;
            job.CompanyName = congTy?.Name;
            job.CompanyLogo = congTy?.LogoUrl;
            job.CategoryId = model.CategoryId;
            job.CategoryName = danhMuc?.Name;
            job.Description = model.Description;
            job.Requirements = model.Requirements;
            job.Benefits = model.Benefits;
            job.EmploymentType = model.EmploymentType;
            job.Level = model.Level;
            job.WorkMode = model.WorkMode;
            job.City = model.City;
            job.Address = model.Address;
            job.SalaryMin = model.SalaryMin;
            job.SalaryMax = model.SalaryMax;
            job.SalaryNegotiable = model.SalaryNegotiable;
            job.Currency = model.Currency;
            job.MinExperienceYears = model.MinExperienceYears;
            job.MinEducation = model.MinEducation;
            job.Headcount = model.Headcount;
            job.Status = model.Status;

            // Chỉ set lại PublishedAt khi tin CHUYỂN sang published lần đầu,
            // tránh ghi đè ngày đăng gốc mỗi lần sửa nội dung.
            if (moiChuyenSangPublished)
            {
                job.PublishedAt = DateTime.Now;
            }

            // Lưu thay đổi vào SQL Server: bổ sung phương thức db.UpdateJob(job) vào class Database.

            return RedirectToAction("Details", new { id = job.Id });
        }

        // =====================================================
        // 5. XOÁ TIN
        // =====================================================
        public ActionResult Delete(long id)
        {
            Job job = db.dsJob.FirstOrDefault(j => j.Id == id);

            if (job == null)
            {
                return HttpNotFound();
            }

            return View(job);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Job job = db.dsJob.FirstOrDefault(j => j.Id == id);

            if (job == null)
            {
                return HttpNotFound();
            }

            // Ghi chú: theo tài liệu thiết kế DB (mục 7, quyết định #4), khi lên
            // SQL Server thật nên cân nhắc "xoá mềm" (job.Status = "closed")
            // thay vì DELETE hẳn, để không vướng các Application/JobSkill đang
            // tham chiếu tới job này. Ở bản demo (List trong bộ nhớ) thì xoá
            // thẳng khỏi danh sách cho đơn giản.
            db.dsJob.Remove(job);

            return RedirectToAction("Index");
        }
    }
}
