using DoAnLapTrinhWeb.Models;
using DocumentFormat.OpenXml.Packaging;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DoAnLapTrinhWeb.Utils
{
    // Helper đọc nội dung CV (PDF / Word) và tự động lọc ra các Skill xuất hiện trong CV
    public static class CvParserHelper
    {
        // ---- Đọc toàn bộ text từ file PDF ----
        public static string DocPdf(string duongDanVatLy)
        {
            StringBuilder sb = new StringBuilder();

            using (PdfDocument pdf = new PdfDocument(new PdfReader(duongDanVatLy)))
            {
                for (int trang = 1; trang <= pdf.GetNumberOfPages(); trang++)
                {
                    sb.AppendLine(PdfTextExtractor.GetTextFromPage(pdf.GetPage(trang)));
                }
            }

            return sb.ToString();
        }

        // ---- Đọc toàn bộ text từ file Word (.docx) ----
        public static string DocWord(string duongDanVatLy)
        {
            using (WordprocessingDocument doc = WordprocessingDocument.Open(duongDanVatLy, false))
            {
                return doc.MainDocumentPart.Document.Body.InnerText;
            }
        }

        // ---- Tự nhận diện định dạng theo phần mở rộng và trích xuất text ----
        public static string TrichXuatText(string duongDanVatLy, string phanMoRong)
        {
            phanMoRong = phanMoRong.ToLower();

            try
            {
                if (phanMoRong == ".pdf")
                {
                    return DocPdf(duongDanVatLy);
                }

                if (phanMoRong == ".docx")
                {
                    return DocWord(duongDanVatLy);
                }
            }
            catch
            {
                // File lỗi / không đọc được nội dung -> trả về rỗng, không chặn việc nộp đơn
                return "";
            }

            return "";
        }

        // ---- Lọc các Skill (theo tên chính hoặc alias) có xuất hiện trong nội dung CV ----
        public static List<Skill> LocSkillTrongNoiDung(string noiDung, List<Skill> dsSkill, List<SkillAlias> dsAlias)
        {
            List<Skill> ketQua = new List<Skill>();

            if (string.IsNullOrWhiteSpace(noiDung))
            {
                return ketQua;
            }

            string noiDungChuan = ChuanHoa(noiDung);

            foreach (Skill sk in dsSkill)
            {
                bool trungTen = KiemTraXuatHien(noiDungChuan, sk.Name);

                bool trungAlias = dsAlias
                    .Where(a => a.SkillId == sk.Id)
                    .Any(a => KiemTraXuatHien(noiDungChuan, a.Alias));

                if (trungTen || trungAlias)
                {
                    ketQua.Add(sk);
                }
            }

            return ketQua;
        }

        private static bool KiemTraXuatHien(string noiDungChuan, string tuKhoa)
        {
            if (string.IsNullOrWhiteSpace(tuKhoa))
            {
                return false;
            }

            string mau = @"(?<![a-z0-9\+#\.])" + Regex.Escape(ChuanHoa(tuKhoa)) + @"(?![a-z0-9\+#])";
            return Regex.IsMatch(noiDungChuan, mau);
        }

        private static string ChuanHoa(string s)
        {
            return s.ToLower().Trim();
        }
    }
}
