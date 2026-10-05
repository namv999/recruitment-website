using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using recruitment_website.Constants;

namespace recruitment_website.Models.Job
{
    /* Dựng SelectListItem cho dropdown từ Constants (form đăng tin + bộ lọc dùng chung). */
    public static class JobSelectLists
    {
        public static List<SelectListItem> EmploymentTypes(string selected = null)
        {
            return Build(EmploymentType.All, EmploymentType.Label, selected);
        }

        public static List<SelectListItem> Levels(string selected = null)
        {
            return Build(JobLevel.All, JobLevel.Label, selected);
        }

        public static List<SelectListItem> WorkModes(string selected = null)
        {
            return Build(WorkMode.All, WorkMode.Label, selected);
        }

        public static List<SelectListItem> Educations(string selected = null)
        {
            return Build(EducationLevel.All, EducationLevel.Label, selected);
        }

        private static List<SelectListItem> Build(string[] values, Func<string, string> label, string selected)
        {
            return values.Select(v => new SelectListItem
            {
                Value = v,
                Text = label(v),
                Selected = v == selected
            }).ToList();
        }
    }
}
