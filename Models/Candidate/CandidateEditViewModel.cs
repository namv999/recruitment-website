using recruitment_website.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using static recruitment_website.Constants.ApplicationStatus;
using recruitment_website.Constants;

namespace recruitment_website.Models.Candidate
{
    public class CandidateEditViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(150, ErrorMessage = "Họ tên tối đa 150 ký tự")]
        public string FullName { get; set; }

        [StringLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự")]
        [RegularExpression(@"^(0|\+84)\d{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }

        public string Gender { get; set; }

        [StringLength(100, ErrorMessage = "Thành phố tối đa 100 ký tự")]
        public string City { get; set; }

        [StringLength(200, ErrorMessage = "Tiêu đề hồ sơ tối đa 200 ký tự")]
        public string Headline { get; set; }

        public string Summary { get; set; }

        public string HighestEducation { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Mức lương mong muốn không hợp lệ")]
        public int? ExpectedSalary { get; set; }

        public bool OpenToWork { get; set; }

        // Dropdown do ViewModel cấp sẵn, View chỉ việc render
        public IEnumerable<SelectListItem> GenderOptions
        {
            get
            {
                return new[] { Constants.Gender.Male, Constants.Gender.Female, Constants.Gender.Other }
                    .Select(g => new SelectListItem
                    {
                        Value = g,
                        Text = Constants.Gender.Label(g),
                        Selected = (g == Gender)
                    });
            }
        }

        public IEnumerable<SelectListItem> EducationOptions
        {
            get
            {
                return EducationLevel.All
                    .Select(e => new SelectListItem { Value = e, Text = EducationLevel.Label(e), Selected = e == HighestEducation });
            }
        }
    }
}   