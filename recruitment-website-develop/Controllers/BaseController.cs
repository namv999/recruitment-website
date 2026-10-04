using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace recruitment_website.Controllers
{
    /* 
        Controller nền tảng dùng chung: quản lý trạng thái đăng nhập, 
            lấy UserId/Role từ Session, 
            chặn user chưa đăng nhập và cung cấp thông tin đăng nhập cho View.
    */
    public class BaseController : Controller
    {
        protected long? CurrentUserId
        {
            get { return Session["UserId"] as long?; }
        }

        protected string CurrentUserRole
        {
            get { return Session["UserRole"] as string; } // "admin" | "employer" | "candidate"
        }

        protected bool IsLoggedIn
        {
            get { return CurrentUserId.HasValue; }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool allowAnonymous =
                filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) ||
                filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);

            if (!allowAnonymous && !IsLoggedIn)
            {
                filterContext.Result = RedirectToAction("Login", "Account");
                return;
            }

            // Đẩy 2 biến này lên mọi View kế thừa BaseController — dùng cho _Layout
            ViewBag.IsLoggedIn = IsLoggedIn;
            ViewBag.CurrentUserRole = CurrentUserRole;

            base.OnActionExecuting(filterContext);
        }
    }
}