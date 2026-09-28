using DoAnLapTrinhWeb.Models;
using DoAnLapTrinhWeb.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnLapTrinhWeb.Controllers
{
    public class TuyenDungController : Controller
    {
        // Khai báo Database dùng chung cho toàn bộ Controller (giống mẫu Bai1Controller)
        Database csdl = new Database();

        // =====================================================
        // 1. TRANG CHỦ — hiển thị job nổi bật + thống kê nhanh
        // =====================================================
        public ActionResult TrangChu()
        {
            List<Job> dsJobNoiBat = csdl.dsJob
                .Where(j => j.Status == "published")
                .OrderByDescending(j => j.PublishedAt)
                .Take(8)
                .ToList();

            ViewBag.DsDanhMuc = csdl.dsJobCategory;
            ViewBag.TongSoJob = csdl.dsJob.Count(j => j.Status == "published");
            ViewBag.TongSoCongTy = csdl.dsCompany.Count(c => c.IsVerified);
            ViewBag.TongSoUngVien = csdl.dsCandidate.Count();

            return View(dsJobNoiBat);
        }

        // =====================================================
        // 2. DANH SÁCH JOB — tìm kiếm + lọc theo danh mục / thành phố
        // =====================================================
        public ActionResult DanhSachJob(string key, int? madm, string city)
        {
            IEnumerable<Job> q = csdl.dsJob.Where(j => j.Status == "published");

            if (!string.IsNullOrEmpty(key))
            {
                q = q.Where(j => j.Title.Contains(key) || j.CompanyName.Contains(key));
            }

            if (madm.HasValue)
            {
                q = q.Where(j => j.CategoryId == madm);
            }

            if (!string.IsNullOrEmpty(city))
            {
                q = q.Where(j => j.City == city);
            }

            List<Job> dsJob = q.OrderByDescending(j => j.PublishedAt).ToList();

            ViewBag.DsDanhMuc = csdl.dsJobCategory;
            ViewBag.DsThanhPho = csdl.dsJob.Select(j => j.City).Distinct().OrderBy(c => c).ToList();
            ViewBag.Key = key;
            ViewBag.Madm = madm;
            ViewBag.City = city;

            return View(dsJob);
        }

        // =====================================================
        // 3. CHI TIẾT JOB
        // =====================================================
        public ActionResult ChiTietJob(long id)
        {
            Job job = csdl.dsJob.FirstOrDefault(j => j.Id == id);

            if (job == null)
            {
                return HttpNotFound();
            }

            // Danh sách kỹ năng yêu cầu của job (join JobSkill - Skill)
            List<Skill> dsKyNangYeuCau =
                (from js in csdl.dsJobSkill
                 join s in csdl.dsSkill on js.SkillId equals s.Id
                 where js.JobId == id
                 select s).ToList();

            // Các job khác cùng công ty
            List<Job> dsJobLienQuan = csdl.dsJob
                .Where(j => j.CompanyId == job.CompanyId && j.Id != job.Id && j.Status == "published")
                .Take(4)
                .ToList();

            ViewBag.DsKyNangYeuCau = dsKyNangYeuCau;
            ViewBag.DsJobLienQuan = dsJobLienQuan;

            return View(job);
        }

        // =====================================================
        // 4. TRANG CÔNG TY
        // =====================================================
        public ActionResult Company(long id)
        {
            Company congTy = csdl.dsCompany.FirstOrDefault(c => c.Id == id);

            if (congTy == null)
            {
                return HttpNotFound();
            }

            List<Job> dsJobCuaCongTy = csdl.dsJob
                .Where(j => j.CompanyId == id && j.Status == "published")
                .OrderByDescending(j => j.PublishedAt)
                .ToList();

            ViewBag.DsJobCuaCongTy = dsJobCuaCongTy;

            return View(congTy);
        }

        // =====================================================
        // 5. HỒ SƠ ỨNG VIÊN
        // =====================================================
        // Ghi chú: dự án demo chưa có đăng nhập, nên mã ứng viên (id) được
        // truyền qua query string, giống cách "matour" được dùng trong Bai1Controller.
        public ActionResult UngVien(long id = 1)
        {
            Candidate ungVien = csdl.dsCandidate.FirstOrDefault(c => c.Id == id);

            if (ungVien == null)
            {
                return HttpNotFound();
            }

            List<int> maKyNang = csdl.dsCandidateSkill
                .Where(s => s.CandidateId == id)
                .Select(s => s.SkillId)
                .ToList();

            ViewBag.DsCV = csdl.dsCV.Where(c => c.CandidateId == id).ToList();
            ViewBag.DsKinhNghiem = csdl.dsCandidateExperience
                .Where(e => e.CandidateId == id)
                .OrderByDescending(e => e.StartDate)
                .ToList();
            ViewBag.DsHocVan = csdl.dsCandidateEducation.Where(e => e.CandidateId == id).ToList();
            ViewBag.DsKyNang = csdl.dsSkill.Where(s => maKyNang.Contains(s.Id)).ToList();

            return View(ungVien);
        }

        // =====================================================
        // 6. CHI TIẾT CV
        // =====================================================
        public ActionResult CV(long id)
        {
            CV cv = csdl.dsCV.FirstOrDefault(c => c.Id == id);

            if (cv == null)
            {
                return HttpNotFound();
            }

            ViewBag.UngVien = csdl.dsCandidate.FirstOrDefault(c => c.Id == cv.CandidateId);

            return View(cv);
        }

        // =====================================================
        // 7. ỨNG TUYỂN — GET: hiển thị form / POST: lưu đơn ứng tuyển
        // =====================================================
        public ActionResult UngTuyen(long jobId, long ungvien = 1)
        {
            Job job = csdl.dsJob.FirstOrDefault(j => j.Id == jobId);

            if (job == null)
            {
                return HttpNotFound();
            }

            ViewBag.DsCV = csdl.dsCV.Where(c => c.CandidateId == ungvien).ToList();
            ViewBag.MaUngVien = ungvien;
            ViewBag.DaUngTuyen = csdl.dsApplication.Any(a => a.JobId == jobId && a.CandidateId == ungvien);

            return View(job);
        }

        // cvIdCoSan: ứng viên chọn 1 CV đã có sẵn trong hồ sơ
        // cvFile: ứng viên tải lên file CV mới (PDF / Word) ngay tại form ứng tuyển
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UngTuyen(long jobId, long ungvien, string coverLetter, long? cvIdCoSan, HttpPostedFileBase cvFile)
        {
            bool daUngTuyenRoi = csdl.dsApplication.Any(a => a.JobId == jobId && a.CandidateId == ungvien);

            if (daUngTuyenRoi)
            {
                return RedirectToAction("TrangThaiUngTuyen", new { ungvien = ungvien });
            }

            long cvIdSuDung;

            if (cvFile != null && cvFile.ContentLength > 0)
            {
                // ============ NỘP CV BẰNG FILE MỚI (PDF / DOCX) ============
                string phanMoRong = Path.GetExtension(cvFile.FileName).ToLower();
                string[] dinhDangChoPhep = { ".pdf", ".docx" };

                if (!dinhDangChoPhep.Contains(phanMoRong))
                {
                    TempData["LoiUngTuyen"] = "Chỉ hỗ trợ file PDF hoặc Word (.docx). Vui lòng chọn lại file.";
                    return RedirectToAction("UngTuyen", new { jobId, ungvien });
                }

                string thuMucLuu = Server.MapPath("~/Uploads/CV/");
                if (!Directory.Exists(thuMucLuu))
                {
                    Directory.CreateDirectory(thuMucLuu);
                }

                string tenFileVatLy = Guid.NewGuid().ToString("N") + phanMoRong;
                string duongDanVatLy = Path.Combine(thuMucLuu, tenFileVatLy);
                cvFile.SaveAs(duongDanVatLy);

                // Đọc nội dung file + tự động lọc skill khớp với bảng Skill / SkillAlias
                string noiDungText = CvParserHelper.TrichXuatText(duongDanVatLy, phanMoRong);
                List<Skill> dsSkillTimThay = CvParserHelper.LocSkillTrongNoiDung(noiDungText, csdl.dsSkill, csdl.dsSkillAlias);

                CV cvMoi = new CV
                {
                    Id = csdl.dsCV.Count > 0 ? csdl.dsCV.Max(c => c.Id) + 1 : 1,
                    CandidateId = ungvien,
                    Title = Path.GetFileNameWithoutExtension(cvFile.FileName),
                    FileUrl = "/Uploads/CV/" + tenFileVatLy, // link tải file GỐC cho nhà tuyển dụng
                    ParsedText = noiDungText,
                    IsDefault = !csdl.dsCV.Any(c => c.CandidateId == ungvien),
                    UploadedAt = DateTime.Now
                };

                csdl.dsCV.Add(cvMoi);
                cvIdSuDung = cvMoi.Id;

                // Ghi nhận các kỹ năng vừa lọc được vào hồ sơ ứng viên (nếu chưa có)
                foreach (Skill sk in dsSkillTimThay)
                {
                    bool daCoKyNang = csdl.dsCandidateSkill.Any(cs => cs.CandidateId == ungvien && cs.SkillId == sk.Id);

                    if (!daCoKyNang)
                    {
                        csdl.dsCandidateSkill.Add(new CandidateSkill
                        {
                            CandidateId = ungvien,
                            SkillId = sk.Id,
                            Proficiency = 1,
                            YearsExperience = 0
                        });
                    }
                }

                TempData["SkillTimThayTuCV"] = dsSkillTimThay.Select(s => s.Name).ToList();

                // Lưu CV vào SQL Server: bổ sung phương thức csdl.InsertCV(cvMoi) vào class Database.
            }
            else if (cvIdCoSan.HasValue)
            {
                // ============ DÙNG LẠI CV ĐÃ CÓ SẴN TRONG HỒ SƠ ============
                cvIdSuDung = cvIdCoSan.Value;
            }
            else
            {
                TempData["LoiUngTuyen"] = "Vui lòng chọn 1 CV có sẵn hoặc tải lên file CV mới.";
                return RedirectToAction("UngTuyen", new { jobId, ungvien });
            }

            Application donUngTuyen = new Application
            {
                Id = csdl.dsApplication.Count > 0 ? csdl.dsApplication.Max(a => a.Id) + 1 : 1,
                JobId = jobId,
                CandidateId = ungvien,
                CvId = cvIdSuDung,
                CoverLetter = coverLetter,
                Status = "submitted",
                AppliedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            csdl.dsApplication.Add(donUngTuyen);

            // Lưu Application vào SQL Server: bổ sung phương thức
            // csdl.InsertApplication(donUngTuyen) vào class Database (xem ghi chú ở tin nhắn trước).

            return RedirectToAction("TrangThaiUngTuyen", new { ungvien = ungvien });
        }

        // =====================================================
        // 8. TRẠNG THÁI ỨNG TUYỂN
        // =====================================================
        public ActionResult TrangThaiUngTuyen(long ungvien = 1)
        {
            List<Application> dsDonUngTuyen = csdl.dsApplication
                .Where(a => a.CandidateId == ungvien)
                .OrderByDescending(a => a.AppliedAt)
                .ToList();

            ViewBag.DsJob = csdl.dsJob;
            ViewBag.DsCongTy = csdl.dsCompany;
            ViewBag.DsLichSu = csdl.dsApplicationStatusHistory;
            ViewBag.MaUngVien = ungvien;

            return View(dsDonUngTuyen);
        }

        // =====================================================
        // MENU DANH MỤC NGHỀ (partial view dùng trong layout)
        // =====================================================
        public ActionResult Menu()
        {
            List<JobCategory> dsDanhMuc = csdl.dsJobCategory;
            return PartialView("_MenuDanhMuc", dsDanhMuc);
        }
    }
}
