using System.Collections.Generic;
using recruitment_website.Models.Shared;

namespace recruitment_website.Models.Candidate
{
    // Một dòng kỹ năng của ứng viên (dùng ở trang Skills và khối Kỹ năng trong Profile)
    public class CandidateSkillRowViewModel
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public int Proficiency { get; set; }          // 1..5
        public string YearsText { get; set; }         // đã format bằng InvariantCulture ("1.5"), dùng cho value của input number

        public string ProficiencyLabel { get { return LevelLabel(Proficiency); } }

        public static string LevelLabel(int level)
        {
            switch (level)
            {
                case 1: return "Mới học";
                case 2: return "Cơ bản";
                case 3: return "Khá";
                case 4: return "Tốt";
                case 5: return "Chuyên gia";
                default: return "";
            }
        }
    }

    // Model của View Candidate/Skills
    public class CandidateSkillsViewModel
    {
        public SkillSelectorViewModel SkillSelector { get; set; } = new SkillSelectorViewModel();
        public List<CandidateSkillRowViewModel> Items { get; set; } = new List<CandidateSkillRowViewModel>();
    }

    // POST SaveSkills: checkbox name = "SelectedSkillIds"
    public class CandidateSkillSelectionForm
    {
        public List<int> SelectedSkillIds { get; set; }
    }

    // POST UpdateSkillLevels: Items[i].SkillId / Items[i].Proficiency / Items[i].YearsExperience
    public class CandidateSkillLevelItem
    {
        public int SkillId { get; set; }
        public int Proficiency { get; set; }
        public string YearsExperience { get; set; }   // nhận chuỗi, tự parse (tránh lỗi dấu "." và "," theo culture vi-VN)
    }

    public class CandidateSkillLevelsForm
    {
        public List<CandidateSkillLevelItem> Items { get; set; }
    }
}