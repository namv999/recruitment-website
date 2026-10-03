using System;
using System.ComponentModel.DataAnnotations;

namespace recruitment_website.Models.Candidate
{
    public class ExperienceFormViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên công ty")]
        [StringLength(255)]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập chức danh")]
        [StringLength(150)]
        public string JobTitle { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }   // để trống = đang làm

        public string Description { get; set; }
    }
}   