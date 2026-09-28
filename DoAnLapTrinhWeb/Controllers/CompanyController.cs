using DoAnLapTrinhWeb.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DoAnLapTrinhWeb.Controllers
{
    public class CompanyController : Controller
    {
        // Khai báo Database dùng chung cho toàn bộ Controller (giống mẫu TuyenDungController)
        Database db = new Database();

        // =====================================================
        // 1. DANH SÁCH CÔNG TY
        // =====================================================
        public ActionResult Index()
        {
            List<Company> dsCongTy = db.dsCompany.OrderBy(c => c.Name).ToList();

            // Truyền nguyên danh sách Job để View tự đếm số tin đang tuyển của
            // từng công ty bằng LINQ — cùng cách TrangThaiUngTuyen.cshtml đang
            // tra cứu Job/Company trực tiếp trong View.
            ViewBag.DsJob = db.dsJob;

            return View(dsCongTy);
        }

        // =====================================================
        // 2. CHI TIẾT CÔNG TY
        // =====================================================
        public ActionResult Details(long id)
        {
            Company congTy = db.dsCompany.FirstOrDefault(c => c.Id == id);

            if (congTy == null)
            {
                return HttpNotFound();
            }

            ViewBag.DsJobCuaCongTy = db.dsJob
                .Where(j => j.CompanyId == id)
                .OrderByDescending(j => j.PublishedAt)
                .ToList();

            return View(congTy);
        }

        // =====================================================
        // 3. ĐĂNG KÝ CÔNG TY MỚI — GET: hiển thị form / POST: lưu
        // =====================================================
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Company company)
        {
            if (string.IsNullOrWhiteSpace(company.Name))
            {
                TempData["Loi"] = "Vui lòng nhập tên công ty.";
                return View(company);
            }

            bool trungMaSoThue = !string.IsNullOrEmpty(company.TaxCode)
                && db.dsCompany.Any(c => c.TaxCode == company.TaxCode);

            if (trungMaSoThue)
            {
                TempData["Loi"] = "Mã số thuế này đã được đăng ký bởi công ty khác.";
                return View(company);
            }

            company.Id = db.dsCompany.Count > 0 ? db.dsCompany.Max(c => c.Id) + 1 : 1;
            company.IsVerified = false; // công ty mới đăng ký cần admin xác thực thủ công

            db.dsCompany.Add(company);

            // Lưu Company vào SQL Server: bổ sung phương thức db.InsertCompany(company) vào class Database.

            return RedirectToAction("Details", new { id = company.Id });
        }

        // =====================================================
        // 4. CẬP NHẬT THÔNG TIN CÔNG TY
        // =====================================================
        public ActionResult Edit(long id)
        {
            Company congTy = db.dsCompany.FirstOrDefault(c => c.Id == id);

            if (congTy == null)
            {
                return HttpNotFound();
            }

            return View(congTy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(long id, Company model)
        {
            Company congTy = db.dsCompany.FirstOrDefault(c => c.Id == id);

            if (congTy == null)
            {
                return HttpNotFound();
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                TempData["Loi"] = "Vui lòng nhập tên công ty.";
                model.Id = id;
                return View(model);
            }

            congTy.Name = model.Name;
            congTy.TaxCode = model.TaxCode;
            congTy.LogoUrl = model.LogoUrl;
            congTy.Website = model.Website;
            congTy.Industry = model.Industry;
            congTy.CompanySize = model.CompanySize;
            congTy.Address = model.Address;
            congTy.City = model.City;
            congTy.Description = model.Description;
            congTy.IsVerified = model.IsVerified;

            // Vì Job đang lưu kèm CompanyName/CompanyLogo (denormalize) nên khi
            // đổi tên/logo công ty, đồng bộ luôn cho các tin đang có của công ty này.
            foreach (Job j in db.dsJob.Where(j => j.CompanyId == id))
            {
                j.CompanyName = congTy.Name;
                j.CompanyLogo = congTy.LogoUrl;
            }

            // Lưu thay đổi vào SQL Server: bổ sung phương thức db.UpdateCompany(congTy) vào class Database.

            return RedirectToAction("Details", new { id = congTy.Id });
        }
    }
}
