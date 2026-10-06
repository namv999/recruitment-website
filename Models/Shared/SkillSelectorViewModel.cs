using System;
using System.Collections.Generic;
using System.Linq;

namespace recruitment_website.Models.Shared
{
    /* Một kỹ năng trong danh sách chọn (nguồn: bảng skills + skill_categories). */
    public class SkillOptionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
    }

    /*
        Model của partial view Views/Shared/_SkillSelector.cshtml.
        Dùng chung cho Job (B) và hồ sơ ứng viên (C).
        FieldName: giá trị thuộc tính name của input khi POST (vd "SelectedSkillIds").
    */
    public class SkillSelectorViewModel
    {
        public SkillSelectorViewModel()
        {
            FieldName = "SelectedSkillIds";
            Options = new List<SkillOptionViewModel>();
            SelectedIds = new List<int>();
        }

        public string FieldName { get; set; }
        public List<SkillOptionViewModel> Options { get; set; }
        public List<int> SelectedIds { get; set; }

        /// <summary>Nhóm kỹ năng theo danh mục (Backend, Frontend...) để View hiển thị; kỹ năng không có danh mục xếp vào "Khác".</summary>
        public IEnumerable<IGrouping<string, SkillOptionViewModel>> GroupedOptions
        {
            get
            {
                return (Options ?? new List<SkillOptionViewModel>())
                    .GroupBy(o => string.IsNullOrWhiteSpace(o.CategoryName) ? "Khác" : o.CategoryName)
                    .OrderBy(g => g.Key == "Khác" ? 1 : 0)
                    .ThenBy(g => g.Key);
            }
        }

        public bool IsSelected(int skillId)
        {
            return SelectedIds != null && SelectedIds.Contains(skillId);
        }
    }
}
