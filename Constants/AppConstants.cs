using System.Collections.Generic;
using System.Linq;

namespace recruitment_website.Constants
{
    // Giá trị khớp CHECK constraint trong DB (users.role)
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
            }
        }
    }

    // Đã chuyển ra ngoài class ApplicationStatus
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