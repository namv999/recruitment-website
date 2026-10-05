using System.ComponentModel.DataAnnotations;

namespace recruitment_website.Models.Candidate
{
    public class EducationFormViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên trường")]
        [StringLength(255)]
        public string SchoolName { get; set; }

        [StringLength(150)]
        public string Major { get; set; }

        public string Degree { get; set; }

        [Range(1950, 2100, ErrorMessage = "Năm bắt đầu không hợp lệ")]
        public short? StartYear { get; set; }

        [Range(1950, 2100, ErrorMessage = "Năm kết thúc không hợp lệ")]
        public short? EndYear { get; set; }
    }
}