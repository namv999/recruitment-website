using DoAnLapTrinhWeb.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DoAnLapTrinhWeb.Controllers
{
    public class CandidateController : Controller
    {
        // Khai báo Database dùng chung cho toàn bộ Controller (giống mẫu TuyenDungController)
        Database db = new Database();

        // =====================================================
        // 1. DANH SÁCH ỨNG VIÊN (dành cho NTD/admin duyệt hồ sơ) + tìm theo tên/headline
        // =====================================================
        public ActionResult Index(string key)
        {
            IEnumerable<Candidate> q = db.dsCandidate;

            if (!string.IsNullOrEmpty(key))
            {
                q = q.Where(c => c.FullName.Contains(key)
                    || (c.Headline != null && c.Headline.Contains(key)));
            }

            ViewBag.Key = key;

            return View(q.ToList());
        }

        // =====================================================
        // 2. CHI TIẾT HỒ SƠ ỨNG VIÊN — xem đầy đủ kỹ năng/kinh nghiệm/học vấn/CV
        // (dùng khi NTD/admin xem hồ sơ của MỘT ứng viên cụ thể theo id)
        // =====================================================
        public ActionResult Details(long id)
        {
            Candidate ungVien = db.dsCandidate.FirstOrDefault(c => c.Id == id);

            if (ungVien == null)
            {
                return HttpNotFound();
            }

            NapDuLieuHoSo(id);

            return View(ungVien);
        }

        // =====================================================
        // 3. HỒ SƠ CỦA TÔI — candidate tự xem trang hồ sơ của mình, có link chỉnh sửa
        // =====================================================
        // Ghi chú: dự án demo chưa có đăng nhập nên id được truyền qua query string,
        // mặc định = 1, giống cách UngVien đang làm bên TuyenDungController.
        public ActionResult Profile(long id = 1)
        {
            Candidate ungVien = db.dsCandidate.FirstOrDefault(c => c.Id == id);

            if (ungVien == null)
            {
                return HttpNotFound();
            }

            NapDuLieuHoSo(id);
            ViewBag.SoDonUngTuyen = db.dsApplication.Count(a => a.CandidateId == id);

            return View(ungVien);
        }

        // =====================================================
        // 4. CẬP NHẬT THÔNG TIN CÁ NHÂN
        // =====================================================
        public ActionResult Edit(long id)
        {
            Candidate ungVien = db.dsCandidate.FirstOrDefault(c => c.Id == id);

            if (ungVien == null)
            {
                return HttpNotFound();
            }

            return View(ungVien);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(long id, Candidate model)
        {
            Candidate ungVien = db.dsCandidate.FirstOrDefault(c => c.Id == id);

            if (ungVien == null)
            {
                return HttpNotFound();
            }

            if (string.IsNullOrWhiteSpace(model.FullName))
            {
                TempData["Loi"] = "Vui lòng nhập họ tên.";
                model.Id = id;
                return View(model);
            }

            ungVien.FullName = model.FullName;
            ungVien.Phone = model.Phone;
            ungVien.DateOfBirth = model.DateOfBirth;
            ungVien.Gender = model.Gender;
            ungVien.City = model.City;
            ungVien.Headline = model.Headline;
            ungVien.Summary = model.Summary;
            ungVien.TotalExperienceYears = model.TotalExperienceYears;
            ungVien.HighestEducation = model.HighestEducation;
            ungVien.ExpectedSalary = model.ExpectedSalary;
            ungVien.OpenToWork = model.OpenToWork;

            // Lưu thay đổi vào SQL Server: bổ sung phương thức db.UpdateCandidate(ungVien) vào class Database.

            return RedirectToAction("Profile", new { id = ungVien.Id });
        }

        // =====================================================
        // Hàm dùng chung: nạp kỹ năng / kinh nghiệm / học vấn / CV của 1 ứng viên
        // vào ViewBag — giống hệt logic trong TuyenDungController.UngVien(id)
        // =====================================================
        private void NapDuLieuHoSo(long candidateId)
        {
            List<int> maKyNang = db.dsCandidateSkill
                .Where(s => s.CandidateId == candidateId)
                .Select(s => s.SkillId)
                .ToList();

            ViewBag.DsCV = db.dsCV.Where(c => c.CandidateId == candidateId).ToList();
            ViewBag.DsKinhNghiem = db.dsCandidateExperience
                .Where(e => e.CandidateId == candidateId)
                .OrderByDescending(e => e.StartDate)
                .ToList();
            ViewBag.DsHocVan = db.dsCandidateEducation.Where(e => e.CandidateId == candidateId).ToList();
            ViewBag.DsKyNang = db.dsSkill.Where(s => maKyNang.Contains(s.Id)).ToList();
        }
    }
}
