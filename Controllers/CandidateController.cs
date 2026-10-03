using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using recruitment_website.Constants;
using recruitment_website.DAL;
using recruitment_website.Filters;
using recruitment_website.Models.Candidate;

namespace recruitment_website.Controllers
{
    [AuthorizeRole(UserRole.Candidate)]
    public class CandidateController : BaseController
    {
        private readonly recruitment_dbEntities _db = new recruitment_dbEntities();

        private static readonly string[] AllowedCvExtensions = { ".pdf", ".doc", ".docx" };
        private const int MaxCvSizeBytes = 3 * 1024 * 1024; // 3 MB (Web.config mặc định chặn request > 4 MB)

        // ===================== HỒ SƠ CƠ BẢN =====================

        // GET: Candidate/Profile
        public ActionResult Profile()
        {
            var candidate = GetCurrentCandidate(withDetails: true);
            if (candidate == null) return RedirectToAction("Edit"); // chưa có hồ sơ -> tạo mới

            var vm = new CandidateProfileViewModel
            {
                Id = candidate.id,
                Email = candidate.users != null ? candidate.users.email : null,
                FullName = candidate.full_name,
                Phone = candidate.phone,
                DateOfBirth = candidate.date_of_birth,
                GenderLabel = Gender.Label(candidate.gender),
                City = candidate.city,
                Headline = candidate.headline,
                Summary = candidate.summary,
                TotalExperienceYears = candidate.total_experience_years,
                HighestEducationLabel = EducationLevel.Label(candidate.highest_education),
                ExpectedSalary = candidate.expected_salary,
                OpenToWork = candidate.open_to_work,
                Experiences = candidate.candidate_experiences
                    .OrderByDescending(e => e.start_date)
                    .Select(e => new ExperienceItemViewModel
                    {
                        Id = e.id,
                        CompanyName = e.company_name,
                        JobTitle = e.job_title,
                        StartDate = e.start_date,
                        EndDate = e.end_date,
                        Description = e.description
                    }).ToList(),
                Educations = candidate.candidate_educations
                    .OrderByDescending(e => e.end_year)
                    .Select(e => new EducationItemViewModel
                    {
                        Id = e.id,
                        SchoolName = e.school_name,
                        Major = e.major,
                        DegreeLabel = EducationLevel.Label(e.degree),
                        StartYear = e.start_year,
                        EndYear = e.end_year
                    }).ToList()
            };
            return View(vm);
        }

        // GET: Candidate/Edit
        public ActionResult Edit()
        {
            var c = GetCurrentCandidate();
            var vm = new CandidateEditViewModel { OpenToWork = true };
            if (c != null)
            {
                vm.FullName = c.full_name;
                vm.Phone = c.phone;
                vm.DateOfBirth = c.date_of_birth;
                vm.Gender = c.gender;
                vm.City = c.city;
                vm.Headline = c.headline;
                vm.Summary = c.summary;
                vm.HighestEducation = c.highest_education;
                vm.ExpectedSalary = c.expected_salary;
                vm.OpenToWork = c.open_to_work;
            }
            return View(vm);
        }

        // POST: Candidate/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CandidateEditViewModel model)
        {
            if (!string.IsNullOrEmpty(model.Gender) && !Gender.IsValid(model.Gender))
                ModelState.AddModelError("Gender", "Giới tính không hợp lệ");
            if (!string.IsNullOrEmpty(model.HighestEducation) && !EducationLevel.IsValid(model.HighestEducation))
                ModelState.AddModelError("HighestEducation", "Trình độ học vấn không hợp lệ");
            if (model.DateOfBirth.HasValue &&
                (model.DateOfBirth.Value > DateTime.Today || model.DateOfBirth.Value.Year < 1900))
                ModelState.AddModelError("DateOfBirth", "Ngày sinh không hợp lệ");

            if (!ModelState.IsValid) return View(model);

            var c = GetCurrentCandidate();
            if (c == null)
            {
                c = new candidates
                {
                    user_id = GetCurrentUserId(),
                    total_experience_years = 0,
                    created_at = DateTime.Now
                };
                _db.candidates.Add(c);
            }

            c.full_name = model.FullName.Trim();
            c.phone = NullIfEmpty(model.Phone);
            c.date_of_birth = model.DateOfBirth;
            c.gender = NullIfEmpty(model.Gender);
            c.city = NullIfEmpty(model.City);
            c.headline = NullIfEmpty(model.Headline);
            c.summary = NullIfEmpty(model.Summary);
            c.highest_education = NullIfEmpty(model.HighestEducation);
            c.expected_salary = model.ExpectedSalary;
            c.open_to_work = model.OpenToWork;
            c.updated_at = DateTime.Now;

            _db.SaveChanges();
            TempData["Success"] = "Đã lưu hồ sơ cá nhân.";
            return RedirectToAction("Profile");
        }

        // ===================== KINH NGHIỆM =====================

        // POST: Candidate/AddExperience
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddExperience(ExperienceFormViewModel model)
        {
            var c = GetCurrentCandidate();
            if (c == null) return RedirectToEditFirst();

            if (model.StartDate.HasValue && model.EndDate.HasValue && model.EndDate.Value < model.StartDate.Value)
                ModelState.AddModelError("EndDate", "Ngày kết thúc phải sau ngày bắt đầu");
            if (!ModelState.IsValid) return BackWithError("Profile");

            _db.candidate_experiences.Add(new candidate_experiences
            {
                candidate_id = c.id,
                company_name = model.CompanyName.Trim(),
                job_title = model.JobTitle.Trim(),
                start_date = model.StartDate.Value,
                end_date = model.EndDate,
                description = NullIfEmpty(model.Description)
            });
            _db.SaveChanges();
            RecalcExperienceYears(c);

            TempData["Success"] = "Đã thêm kinh nghiệm làm việc.";
            return RedirectToAction("Profile");
        }

        // POST: Candidate/DeleteExperience/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteExperience(long id)
        {
            var c = GetCurrentCandidate();
            if (c == null) return RedirectToEditFirst();

            // Kiểm tra quyền sở hữu: chỉ xóa bản ghi của chính mình
            var entity = _db.candidate_experiences.FirstOrDefault(e => e.id == id && e.candidate_id == c.id);
            if (entity == null) return HttpNotFound();

            _db.candidate_experiences.Remove(entity);
            _db.SaveChanges();
            RecalcExperienceYears(c);

            TempData["Success"] = "Đã xóa kinh nghiệm làm việc.";
            return RedirectToAction("Profile");
        }

        // ===================== HỌC VẤN =====================

        // POST: Candidate/AddEducation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEducation(EducationFormViewModel model)
        {
            var c = GetCurrentCandidate();
            if (c == null) return RedirectToEditFirst();

            if (!string.IsNullOrEmpty(model.Degree) && !EducationLevel.IsValid(model.Degree))
                ModelState.AddModelError("Degree", "Bằng cấp không hợp lệ");
            if (model.StartYear.HasValue && model.EndYear.HasValue && model.EndYear.Value < model.StartYear.Value)
                ModelState.AddModelError("EndYear", "Năm kết thúc phải sau năm bắt đầu");
            if (!ModelState.IsValid) return BackWithError("Profile");

            _db.candidate_educations.Add(new candidate_educations
            {
                candidate_id = c.id,
                school_name = model.SchoolName.Trim(),
                major = NullIfEmpty(model.Major),
                degree = NullIfEmpty(model.Degree),
                start_year = model.StartYear,
                end_year = model.EndYear
            });
            _db.SaveChanges();

            TempData["Success"] = "Đã thêm học vấn.";
            return RedirectToAction("Profile");
        }

        // POST: Candidate/DeleteEducation/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteEducation(long id)
        {
            var c = GetCurrentCandidate();
            if (c == null) return RedirectToEditFirst();

            var entity = _db.candidate_educations.FirstOrDefault(e => e.id == id && e.candidate_id == c.id);
            if (entity == null) return HttpNotFound();

            _db.candidate_educations.Remove(entity);
            _db.SaveChanges();

            TempData["Success"] = "Đã xóa học vấn.";
            return RedirectToAction("Profile");
        }

        // ===================== CV =====================

        // GET: Candidate/Cvs
        public ActionResult Cvs()
        {
            var c = GetCurrentCandidate();
            if (c == null) return RedirectToEditFirst();

            var vm = new CvListViewModel
            {
                Cvs = _db.cvs
                    .Where(x => x.candidate_id == c.id)
                    .OrderByDescending(x => x.is_default)
                    .ThenByDescending(x => x.uploaded_at)
                    .ToList()
                    .Select(x => new CvItemViewModel
                    {
                        Id = x.id,
                        Title = x.title,
                        FileType = Path.GetExtension(x.file_url).TrimStart('.').ToUpperInvariant(),
                        IsDefault = x.is_default,
                        UploadedAt = x.uploaded_at
                    }).ToList()
            };
            return View(vm);
        }

        // POST: Candidate/UploadCv
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadCv(CvUploadViewModel model, HttpPostedFileBase file)
        {
            var c = GetCurrentCandidate();
            if (c == null) return RedirectToEditFirst();

            string ext = null;
            if (file == null || file.ContentLength == 0)
            {
                ModelState.AddModelError("file", "Vui lòng chọn file CV.");
            }
            else
            {
                ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!AllowedCvExtensions.Contains(ext))
                    ModelState.AddModelError("file", "Chỉ chấp nhận file .pdf, .doc, .docx.");
                else if (file.ContentLength > MaxCvSizeBytes)
                    ModelState.AddModelError("file", "File CV tối đa 3 MB.");
                else if (!HasValidSignature(file, ext))
                    ModelState.AddModelError("file", "Nội dung file không đúng định dạng.");
            }
            if (!ModelState.IsValid) return BackWithError("Cvs");

            // Lưu trong App_Data: IIS không cho truy cập trực tiếp, chỉ tải qua DownloadCv (có kiểm tra quyền)
            string relativeFolder = "~/App_Data/cvs/" + c.id;
            string fileName = Guid.NewGuid().ToString("N") + ext;   // tên ngẫu nhiên, không dùng tên người dùng gửi
            string physicalFolder = Server.MapPath(relativeFolder);
            Directory.CreateDirectory(physicalFolder);
            string physicalPath = Path.Combine(physicalFolder, fileName);
            file.SaveAs(physicalPath);

            try
            {
                bool isFirst = !_db.cvs.Any(x => x.candidate_id == c.id);
                _db.cvs.Add(new cvs
                {
                    candidate_id = c.id,
                    title = model.Title.Trim(),
                    file_url = relativeFolder + "/" + fileName,
                    is_default = isFirst,          // CV đầu tiên tự thành mặc định
                    uploaded_at = DateTime.Now
                });
                _db.SaveChanges();
            }
            catch
            {
                System.IO.File.Delete(physicalPath);   // tránh file mồ côi nếu lưu DB lỗi
                throw;
            }

            TempData["Success"] = "Đã tải lên CV.";
            return RedirectToAction("Cvs");
        }

        // POST: Candidate/SetDefaultCv/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetDefaultCv(long id)
        {
            var c = GetCurrentCandidate();
            if (c == null) return RedirectToEditFirst();

            var cv = _db.cvs.FirstOrDefault(x => x.id == id && x.candidate_id == c.id);
            if (cv == null) return HttpNotFound();

            foreach (var other in _db.cvs.Where(x => x.candidate_id == c.id && x.is_default))
                other.is_default = false;
            cv.is_default = true;
            _db.SaveChanges();

            TempData["Success"] = "Đã đặt CV mặc định.";
            return RedirectToAction("Cvs");
        }

        // POST: Candidate/DeleteCv/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCv(long id)
        {
            var c = GetCurrentCandidate();
            if (c == null) return RedirectToEditFirst();

            var cv = _db.cvs.FirstOrDefault(x => x.id == id && x.candidate_id == c.id);
            if (cv == null) return HttpNotFound();

            // FK applications.cv_id là NO ACTION: xóa CV đang được dùng sẽ làm SaveChanges ném lỗi 500
            if (_db.applications.Any(a => a.cv_id == id))
            {
                TempData["Error"] = "CV này đã được dùng để ứng tuyển nên không thể xóa.";
                return RedirectToAction("Cvs");
            }

            bool wasDefault = cv.is_default;
            string physicalPath = Server.MapPath(cv.file_url);

            _db.cvs.Remove(cv);
            _db.SaveChanges();
            if (System.IO.File.Exists(physicalPath)) System.IO.File.Delete(physicalPath);

            if (wasDefault)
            {
                var next = _db.cvs.Where(x => x.candidate_id == c.id)
                                  .OrderByDescending(x => x.uploaded_at)
                                  .FirstOrDefault();
                if (next != null) { next.is_default = true; _db.SaveChanges(); }
            }

            TempData["Success"] = "Đã xóa CV.";
            return RedirectToAction("Cvs");
        }

        // GET: Candidate/DownloadCv/5  (chỉ chủ CV; nhà tuyển dụng sẽ tải qua ApplicationController)
        public ActionResult DownloadCv(long id)
        {
            var c = GetCurrentCandidate();
            if (c == null) return RedirectToEditFirst();

            var cv = _db.cvs.FirstOrDefault(x => x.id == id && x.candidate_id == c.id);
            if (cv == null) return HttpNotFound();

            string physicalPath = Server.MapPath(cv.file_url);
            if (!System.IO.File.Exists(physicalPath)) return HttpNotFound();

            string ext = Path.GetExtension(physicalPath).ToLowerInvariant();
            string contentType = ext == ".pdf" ? "application/pdf"
                               : ext == ".docx" ? "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                               : "application/msword";
            string safeTitle = string.Concat(cv.title.Split(Path.GetInvalidFileNameChars()));
            return File(physicalPath, contentType, safeTitle + ext);
        }

        // ===================== HELPER =====================

        private long GetCurrentUserId()
        {
            // TODO: sửa cho khớp cách AccountController lưu đăng nhập
            // (xem hàm Login: nó lưu Session["UserId"] hay FormsAuthentication?)
            return Convert.ToInt64(Session["UserId"]);
        }

        // withDetails = true chỉ dùng ở Profile (cần nạp kinh nghiệm, học vấn, email);
        // các action khác không cần nên bỏ Include để tránh truy vấn thừa
        private candidates GetCurrentCandidate(bool withDetails = false)
        {
            long userId = GetCurrentUserId();
            IQueryable<candidates> query = _db.candidates;
            if (withDetails)
            {
                query = _db.candidates
                    .Include(x => x.users)
                    .Include(x => x.candidate_experiences)
                    .Include(x => x.candidate_educations);
            }
            return query.FirstOrDefault(x => x.user_id == userId);
        }

        // Tự tính tổng năm kinh nghiệm từ danh sách kinh nghiệm (khoảng thời gian chồng nhau sẽ bị cộng dồn)
        private void RecalcExperienceYears(candidates c)
        {
            var today = DateTime.Today;
            var days = _db.candidate_experiences
                .Where(e => e.candidate_id == c.id)
                .ToList()
                .Sum(e => ((e.end_date ?? today) - e.start_date).TotalDays);

            c.total_experience_years = Math.Round((decimal)(days / 365.25), 1);
            c.updated_at = DateTime.Now;
            _db.SaveChanges();
        }

        private ActionResult RedirectToEditFirst()
        {
            TempData["Error"] = "Vui lòng cập nhật thông tin cơ bản trước.";
            return RedirectToAction("Edit");
        }

        // Gom lỗi ModelState vào TempData["Error"], rồi quay về action chỉ định
        private ActionResult BackWithError(string actionName)
        {
            var msg = string.Join(" ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .Where(m => !string.IsNullOrEmpty(m)));
            TempData["Error"] = string.IsNullOrEmpty(msg) ? "Dữ liệu không hợp lệ." : msg;
            return RedirectToAction(actionName);
        }

        // Kiểm tra vài byte đầu file để chặn trường hợp đổi đuôi .exe thành .pdf
        private static bool HasValidSignature(HttpPostedFileBase file, string ext)
        {
            var head = new byte[4];
            file.InputStream.Position = 0;
            int read = file.InputStream.Read(head, 0, 4);
            file.InputStream.Position = 0;
            if (read < 4) return false;

            if (ext == ".pdf") return head[0] == 0x25 && head[1] == 0x50 && head[2] == 0x44 && head[3] == 0x46; // %PDF
            if (ext == ".docx") return head[0] == 0x50 && head[1] == 0x4B;                                       // PK (zip)
            return head[0] == 0xD0 && head[1] == 0xCF && head[2] == 0x11 && head[3] == 0xE0;                     // .doc
        }

        private static string NullIfEmpty(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}