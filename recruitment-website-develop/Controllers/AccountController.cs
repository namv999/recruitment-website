using recruitment_website.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using recruitment_website.Models;

namespace recruitment_website.Controllers
{
    public class AccountController : Controller
    {
        private readonly recruitment_dbEntities db = new recruitment_dbEntities();

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = db.users.FirstOrDefault(u => u.email == model.Email && u.status == "active");

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.password_hash))
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                return View(model);
            }

            Session["UserId"] = user.id;
            Session["UserRole"] = user.role;
            Session["FullName"] = GetFullName(user);

            user.last_login_at = DateTime.Now;
            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }

        private string GetFullName(users user)
        {
            if (user.role == "candidate")
                return user.candidates.FirstOrDefault()?.full_name ?? user.email;
            return user.company_members.FirstOrDefault()?.full_name ?? user.email;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (db.users.Any(u => u.email == model.Email))
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng.");
                return View(model);
            }

            if (model.Role == "candidate" && string.IsNullOrWhiteSpace(model.FullName))
            {
                ModelState.AddModelError("FullName", "Vui lòng nhập họ tên.");
                return View(model);
            }
            if (model.Role == "employer")
            {
                if (string.IsNullOrWhiteSpace(model.CompanyName))
                {
                    ModelState.AddModelError("CompanyName", "Vui lòng nhập tên công ty.");
                    return View(model);
                }
                if (!string.IsNullOrWhiteSpace(model.TaxCode) && db.companies.Any(c => c.tax_code == model.TaxCode))
                {
                    ModelState.AddModelError("TaxCode", "Mã số thuế đã tồn tại trong hệ thống.");
                    return View(model);
                }
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var newUser = new users
                    {
                        email = model.Email,
                        password_hash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                        role = model.Role,
                        status = "active",
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now
                    };
                    db.users.Add(newUser);
                    db.SaveChanges();

                    if (model.Role == "candidate")
                    {
                        db.candidates.Add(new candidates
                        {
                            user_id = newUser.id,
                            full_name = model.FullName,
                            open_to_work = true,
                            total_experience_years = 0,
                            created_at = DateTime.Now,
                            updated_at = DateTime.Now
                        });
                    }
                    else // employer
                    {
                        var newCompany = new companies
                        {
                            name = model.CompanyName,
                            tax_code = string.IsNullOrWhiteSpace(model.TaxCode) ? null : model.TaxCode,
                            is_verified = false,
                            created_at = DateTime.Now,
                            updated_at = DateTime.Now
                        };
                        db.companies.Add(newCompany);
                        db.SaveChanges();

                        db.company_members.Add(new company_members
                        {
                            company_id = newCompany.id,
                            user_id = newUser.id,
                            full_name = model.FullName ?? model.Email,
                            member_role = "owner"
                        });
                    }

                    db.SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "Có lỗi xảy ra, vui lòng thử lại.");
                    return View(model);
                }
            }

            TempData["Success"] = "Vui lòng đăng nhập lại để tiếp tục.";
            return RedirectToAction("Login");
        }
    }
}