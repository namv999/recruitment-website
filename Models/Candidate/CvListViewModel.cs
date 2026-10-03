using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace recruitment_website.Models.Candidate
{
    public class CvListViewModel
    {
        public List<CvItemViewModel> Cvs { get; set; } = new List<CvItemViewModel>();
    }

    public class CvItemViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string FileType { get; set; }   // PDF / DOC / DOCX
        public bool IsDefault { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    public class CvUploadViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên CV")]
        [StringLength(150, ErrorMessage = "Tên CV tối đa 150 ký tự")]
        public string Title { get; set; }
    }
}