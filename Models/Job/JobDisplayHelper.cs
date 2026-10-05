using System;
using System.Globalization;

namespace recruitment_website.Models.Job
{
    /* Hàm định dạng hiển thị dùng chung cho các ViewModel Job (không truy cập DB). */
    public static class JobDisplayHelper
    {
        private static readonly CultureInfo Vi = new CultureInfo("vi-VN");

        /// <summary>"8 - 12 triệu", "Từ 10 triệu", "Đến 15 triệu" hoặc "Thỏa thuận".</summary>
        public static string Salary(int? min, int? max, string currency)
        {
            if (!min.HasValue && !max.HasValue) return "Thỏa thuận";

            string cur = string.IsNullOrWhiteSpace(currency) ? "VND" : currency.Trim().ToUpperInvariant();
            bool isVnd = cur == "VND";
            string unit = isVnd ? " triệu" : " " + cur;

            if (min.HasValue && max.HasValue) return Format(min.Value, isVnd) + " - " + Format(max.Value, isVnd) + unit;
            if (min.HasValue) return "Từ " + Format(min.Value, isVnd) + unit;
            return "Đến " + Format(max.Value, isVnd) + unit;
        }

        private static string Format(int value, bool isVnd)
        {
            return isVnd ? (value / 1000000m).ToString("0.##", Vi) : value.ToString("N0", Vi);
        }

        /// <summary>"Hôm nay", "Hôm qua", "3 ngày trước", "2 tháng trước"...</summary>
        public static string PostedAgo(DateTime? date)
        {
            if (!date.HasValue) return "";
            int days = (DateTime.Now.Date - date.Value.Date).Days;
            if (days <= 0) return "Hôm nay";
            if (days == 1) return "Hôm qua";
            if (days < 30) return days + " ngày trước";
            if (days < 365) return (days / 30) + " tháng trước";
            return (days / 365) + " năm trước";
        }

        /// <summary>Chữ cái đầu tên công ty — dùng thay logo khi logo_url trống.</summary>
        public static string Initial(string name)
        {
            return string.IsNullOrWhiteSpace(name) ? "?" : name.Trim().Substring(0, 1).ToUpperInvariant();
        }
    }
}
