using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace recruitment_website.Filters
{
    /* Bộ lọc phân quyền: kiểm tra Role của user trước khi cho phép Action chạy. */
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;
        public AuthorizeRoleAttribute(params string[] roles) { _roles = roles; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var role = filterContext.HttpContext.Session["UserRole"] as string;
            if (role == null || !_roles.Contains(role))
            {
                filterContext.Result = new RedirectResult("~/Home/AccessDenied");
            }
        }
    }
}