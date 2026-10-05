using System;
using System.Globalization;
using System.Text;

namespace recruitment_website.Services
{
    /* Chuyển tiêu đề tiếng Việt thành slug không dấu, vd "Lập trình viên ASP.NET" -> "lap-trinh-vien-asp-net". */
    public static class SlugHelper
    {
        private const string Fallback = "tin-tuyen-dung";

        public static string ToSlug(string text, int maxLength = 240)
        {
            if (string.IsNullOrWhiteSpace(text)) return Fallback;

            string normalized = text.Trim().ToLowerInvariant().Replace('đ', 'd').Normalize(NormalizationForm.FormD);

            var sb = new StringBuilder();
            foreach (char c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark) continue; // bỏ dấu

                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                    sb.Append(c);
                else if (sb.Length > 0 && sb[sb.Length - 1] != '-')
                    sb.Append('-');
            }

            string slug = sb.ToString().Trim('-');
            if (slug.Length > maxLength) slug = slug.Substring(0, maxLength).Trim('-');

            return slug.Length == 0 ? Fallback : slug;
        }
    }
}
