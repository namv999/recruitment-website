using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace recruitment_website.Constants
{
    // Giá trị khớp CHECK constraint trong DB (users.role)
    /*
        Hằng số dùng chung, khớp 1-1 với CHECK constraint trong database.
        KHÔNG gõ tay chuỗi như "published", "full_time"... ở bất kỳ chỗ nào khác — dùng class ở đây.
        Mỗi class có:
            - các const string (giá trị lưu trong DB)
            - All: danh sách giá trị hợp lệ (dùng để validate dữ liệu gửi lên từ form/query string)
            - Label(value): tên hiển thị tiếng Việt cho View
    */

    /// <summary>users.role</summary>
    public static class UserRole
    {
        public const string Admin = "admin";
        public const string Employer = "employer";
        public const string Candidate = "candidate";
    }

    // Khớp CHECK constraint jobs.status
    public static class JobStatus
    {
        public const string Draft = "draft";
        public const string Pending = "pending";
        public const string Published = "published";
        public const string Paused = "paused";
        public const string Closed = "closed";
        public const string Expired = "expired";
    }

    // Khớp CHECK constraint applications.status
    public static class ApplicationStatus
    {
        public const string Applied = "applied";
        public const string Screening = "screening";
        public const string Shortlisted = "shortlisted";
        public const string Interview = "interview";
        public const string Offer = "offer";
        public const string Hired = "hired";
        public const string Rejected = "rejected";
        public const string Withdrawn = "withdrawn";

        private static readonly Dictionary<string, string[]> _employerTransitions =
            new Dictionary<string, string[]>
            {
                { Applied,     new[] { Screening,   Rejected } },
                { Screening,   new[] { Shortlisted, Rejected } },
                { Shortlisted, new[] { Interview,   Rejected } },
                { Interview,   new[] { Offer,       Rejected } },
                { Offer,       new[] { Hired,       Rejected } }
            };

        public static bool IsFinal(string status)
        {
            return status == Hired || status == Rejected || status == Withdrawn;
        }

        public static bool CanEmployerChange(string from, string to)
        {
            string[] allowed;
            return _employerTransitions.TryGetValue(from ?? "", out allowed)
                   && allowed.Contains(to);
        }

        public static string[] NextOptions(string from)
        {
            string[] allowed;
            return _employerTransitions.TryGetValue(from ?? "", out allowed)
                ? allowed : new string[0];
        }

        public static bool CanWithdraw(string current)
        {
            return !IsFinal(current);
        }

        public static string Label(string status)
        {
            switch (status)
            {
                case Applied: return "Đã nộp hồ sơ";
                case Screening: return "Đang xem xét";
                case Shortlisted: return "Vào danh sách rút gọn";
                case Interview: return "Mời phỏng vấn";
                case Offer: return "Đã gửi offer";
                case Hired: return "Trúng tuyển";
                case Rejected: return "Không phù hợp";
                case Withdrawn: return "Đã rút đơn";
                default: return status;
    /// <summary>jobs.status — draft -> pending -> published -> paused/closed/expired</summary>
    public static class JobStatus
        {
            public const string Draft = "draft";
            public const string Pending = "pending";       // chờ Admin duyệt
            public const string Published = "published";   // hiển thị công khai
            public const string Paused = "paused";
            public const string Closed = "closed";
            public const string Expired = "expired";

            public static readonly string[] All = { Draft, Pending, Published, Paused, Closed, Expired };

            /// <summary>Tin đã đóng hoặc hết hạn thì không cho sửa nữa.</summary>
            public static bool IsEditable(string status)
            {
                return status != Closed && status != Expired;
            }

            public static string Label(string value)
            {
                switch (value)
                {
                    case Draft: return "Bản nháp";
                    case Pending: return "Chờ duyệt";
                    case Published: return "Đang tuyển";
                    case Paused: return "Tạm dừng";
                    case Closed: return "Đã đóng";
                    case Expired: return "Hết hạn";
                    default: return value;
                }
            }
        }

        // Đã chuyển ra ngoài class ApplicationStatus
        public static class Gender
        {
            public const string Male = "male";
            public const string Female = "female";
            public const string Other = "other";

            public static bool IsValid(string v) { return v == Male || v == Female || v == Other; }

            public static string Label(string v)
            {
                switch (v)
                {
                    case Male: return "Nam";
                    case Female: return "Nữ";
                    case Other: return "Khác";
                    default: return "";
    /// <summary>jobs.employment_type</summary>
    public static class EmploymentType
            {
                public const string FullTime = "full_time";
                public const string PartTime = "part_time";
                public const string Internship = "internship";
                public const string Contract = "contract";
                public const string Freelance = "freelance";

                public static readonly string[] All = { FullTime, PartTime, Internship, Contract, Freelance };

                public static string Label(string value)
                {
                    switch (value)
                    {
                        case FullTime: return "Toàn thời gian";
                        case PartTime: return "Bán thời gian";
                        case Internship: return "Thực tập";
                        case Contract: return "Hợp đồng";
                        case Freelance: return "Freelance";
                        default: return value;
                    }
                }
            }

            // Đã chuyển ra ngoài class ApplicationStatus
            /// <summary>jobs.level</summary>
            public static class JobLevel
            {
                public const string Intern = "intern";
                public const string Fresher = "fresher";
                public const string Junior = "junior";
                public const string Middle = "middle";
                public const string Senior = "senior";
                public const string Lead = "lead";
                public const string Manager = "manager";

                public static readonly string[] All = { Intern, Fresher, Junior, Middle, Senior, Lead, Manager };

                public static string Label(string value)
                {
                    switch (value)
                    {
                        case Intern: return "Thực tập sinh";
                        case Fresher: return "Fresher";
                        case Junior: return "Junior";
                        case Middle: return "Middle";
                        case Senior: return "Senior";
                        case Lead: return "Trưởng nhóm";
                        case Manager: return "Quản lý";
                        default: return value;
                    }
                }
            }

            /// <summary>jobs.work_mode</summary>
            public static class WorkMode
            {
                public const string Onsite = "onsite";
                public const string Remote = "remote";
                public const string Hybrid = "hybrid";

                public static readonly string[] All = { Onsite, Remote, Hybrid };

                public static string Label(string value)
                {
                    switch (value)
                    {
                        case Onsite: return "Tại văn phòng";
                        case Remote: return "Làm từ xa";
                        case Hybrid: return "Linh hoạt";
                        default: return value;
                    }
                }
            }

            /// <summary>jobs.min_education (cũng dùng cho candidates.highest_education)</summary>
            public static class EducationLevel
            {
                public const string HighSchool = "high_school";
                public const string College = "college";
                public const string Bachelor = "bachelor";
                public const string Master = "master";
                public const string Phd = "phd";

                public static readonly string[] All = { HighSchool, College, Bachelor, Master, Phd };

                public static bool IsValid(string v) { return All.Contains(v); }

                public static string Label(string v)
                {
                    switch (v)
                public static string Label(string value)
                {
                    switch (value)
                    {
                        case HighSchool: return "Trung học phổ thông";
                        case College: return "Cao đẳng";
                        case Bachelor: return "Đại học";
                        case Master: return "Thạc sĩ";
                        case Phd: return "Tiến sĩ";
                        default: return "";
                    }
                }
            }
        }
                default: return value;
            }
}
    }

    /// <summary>company_members.member_role</summary>
    public static class CompanyMemberRole
{
    public const string Owner = "owner";
    public const string Recruiter = "recruiter";
    public const string Viewer = "viewer";   // chỉ xem, không đăng/sửa tin
}

/// <summary>job_skills.importance</summary>
public static class SkillImportance
{
    public const string Required = "required";
    public const string Preferred = "preferred";
    public const string NiceToHave = "nice_to_have";

    public static readonly string[] All = { Required, Preferred, NiceToHave };

    public static string Label(string value)
    {
        switch (value)
        {
            case Required: return "Bắt buộc";
            case Preferred: return "Ưu tiên";
            case NiceToHave: return "Có thì tốt";
            default: return value;
        }
    }
}
}
