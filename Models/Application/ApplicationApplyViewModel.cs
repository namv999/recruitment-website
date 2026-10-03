using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace recruitment_website.Models.Application
{
    public class CvOptionViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public DateTime UploadedAt { get; set; }
        public bool IsDefault { get; set; }
    }

    public class ApplicationApplyViewModel
    {
        public long JobId { get; set; }

        // Chỉ để hiển thị, được dựng lại ở server mỗi lần render
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public List<CvOptionViewModel> Cvs { get; set; } = new List<CvOptionViewModel>();

        [Required(ErrorMessage = "Vui lòng chọn một CV.")]
        [Display(Name = "CV ứng tuyển")]
        public long? CvId { get; set; }

        [StringLength(2000, ErrorMessage = "Thư giới thiệu tối đa 2000 ký tự.")]
        [Display(Name = "Thư giới thiệu")]
        public string CoverLetter { get; set; }
    }
}