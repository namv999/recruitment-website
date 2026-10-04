using System;
using System.Data.Entity.Infrastructure;
using System.Web.Mvc;
using recruitment_website.Constants;
using recruitment_website.DAL;
using recruitment_website.Filters;
using recruitment_website.Models.Job;
using recruitment_website.Services;

namespace recruitment_website.Controllers
{
    public class JobController : BaseController
    {
        private readonly recruitment_dbEntities _db = new recruitment_dbEntities();
        private readonly IJobService _jobService;

        public JobController()
        {
            _jobService = new JobService(_db);
        }

        // GET: /Job?Keyword=...&CategoryId=...&City=...&Level=...&EmploymentType=...&SalaryMin=...&SalaryMax=...&Page=...
        // Công khai: khách chưa đăng nhập vẫn xem được danh sách.
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index(JobSearchViewModel model)
        {
            if (model == null) model = new JobSearchViewModel();

            _jobService.Search(model);
            return View(model);
        }

        // GET: /Job/Details/5
        // Công khai với tin đang tuyển. Tin chưa công khai chỉ chủ tin / Admin xem được (còn lại trả 404).
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Details(long id)
        {
            var model = _jobService.GetDetail(id, CurrentUserId, CurrentUserRole);
            if (model == null) return HttpNotFound();

            return View(model);
        }

        // GET: /Job/Create
        [AuthorizeRole(UserRole.Employer)]
        [HttpGet]
        public ActionResult Create()
        {
            if (!_jobService.GetCompanyId(CurrentUserId.Value).HasValue)
            {
                TempData["Error"] = "Tài khoản của bạn chưa thuộc công ty nào nên chưa thể đăng tin.";
                return RedirectToAction("Index");
            }

            var model = new JobCreateViewModel();
            _jobService.LoadFormOptions(model);
            return View(model);
        }

        // POST: /Job/Create
        [AuthorizeRole(UserRole.Employer)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(JobCreateViewModel model)
        {
            long userId = CurrentUserId.Value;

            if (!_jobService.GetCompanyId(userId).HasValue)
            {
                TempData["Error"] = "Tài khoản của bạn chưa thuộc công ty nào nên chưa thể đăng tin.";
                return RedirectToAction("Index");
            }

            if (model.CategoryId.HasValue && !_jobService.CategoryExists(model.CategoryId.Value))
                ModelState.AddModelError("CategoryId", "Ngành nghề không tồn tại.");

            if (!ModelState.IsValid)
            {
                _jobService.LoadFormOptions(model); // dropdown không được POST về nên phải nạp lại
                return View(model);
            }

            try
            {
                _jobService.Create(model, userId);
                TempData["Success"] = "Đã gửi tin tuyển dụng. Tin sẽ hiển thị công khai sau khi Admin duyệt.";
                return RedirectToAction("MyJobs");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi lưu tin, vui lòng thử lại.");
                _jobService.LoadFormOptions(model);
                return View(model);
            }
        }

        // GET: /Job/Edit/5
        [AuthorizeRole(UserRole.Employer)]
        [HttpGet]
        public ActionResult Edit(long id)
        {
            var model = _jobService.GetForEdit(id, CurrentUserId.Value);
            if (model == null) return HttpNotFound(); // không tồn tại hoặc không thuộc công ty của bạn

            if (!JobStatus.IsEditable(model.CurrentStatus))
            {
                TempData["Error"] = "Tin đã đóng hoặc hết hạn nên không thể chỉnh sửa.";
                return RedirectToAction("Details", new { id });
            }

            return View(model);
        }

        // POST: /Job/Edit
        [AuthorizeRole(UserRole.Employer)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(JobEditViewModel model)
        {
            long userId = CurrentUserId.Value;

            // Đọc lại từ DB để kiểm tra quyền + trạng thái thật (không tin giá trị hidden field gửi lên)
            var current = _jobService.GetForEdit(model.Id, userId);
            if (current == null) return HttpNotFound();

            if (!JobStatus.IsEditable(current.CurrentStatus))
            {
                TempData["Error"] = "Tin đã đóng hoặc hết hạn nên không thể chỉnh sửa.";
                return RedirectToAction("Details", new { id = model.Id });
            }

            model.CurrentStatus = current.CurrentStatus;
            model.WillBeReviewedAgain = current.WillBeReviewedAgain;

            if (model.CategoryId.HasValue && !_jobService.CategoryExists(model.CategoryId.Value))
                ModelState.AddModelError("CategoryId", "Ngành nghề không tồn tại.");

            if (!ModelState.IsValid)
            {
                _jobService.LoadFormOptions(model);
                return View(model);
            }

            try
            {
                string newStatus = _jobService.Update(model, userId);
                if (newStatus == null) return HttpNotFound();

                TempData["Success"] = newStatus == JobStatus.Pending
                    ? "Đã cập nhật tin. Tin đang chờ Admin duyệt."
                    : "Đã cập nhật tin.";
                return RedirectToAction("Details", new { id = model.Id });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi lưu tin, vui lòng thử lại.");
                _jobService.LoadFormOptions(model);
                return View(model);
            }
        }

        // GET: /Job/MyJobs — quản lý tin của công ty mình
        [AuthorizeRole(UserRole.Employer)]
        [HttpGet]
        public ActionResult MyJobs()
        {
            var model = _jobService.GetMyJobs(CurrentUserId.Value);
            return View(model);
        }

        // POST: /Job/Pause/5
        [AuthorizeRole(UserRole.Employer)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Pause(long id)
        {
            return HandleStatusChange(_jobService.Pause(id, CurrentUserId.Value), "Đã tạm dừng tin tuyển dụng.");
        }

        // POST: /Job/Resume/5
        [AuthorizeRole(UserRole.Employer)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Resume(long id)
        {
            return HandleStatusChange(_jobService.Resume(id, CurrentUserId.Value), "Tin tuyển dụng đã hiển thị công khai trở lại.");
        }

        // POST: /Job/Close/5
        [AuthorizeRole(UserRole.Employer)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Close(long id)
        {
            return HandleStatusChange(_jobService.Close(id, CurrentUserId.Value), "Đã đóng tin tuyển dụng.");
        }

        private ActionResult HandleStatusChange(StatusChangeResult result, string successMessage)
        {
            switch (result)
            {
                case StatusChangeResult.NotFound:
                    return HttpNotFound();

                case StatusChangeResult.InvalidState:
                    TempData["Error"] = "Không thể thực hiện thao tác này với trạng thái hiện tại của tin.";
                    break;

                default:
                    TempData["Success"] = successMessage;
                    break;
            }
            return RedirectToAction("MyJobs");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
