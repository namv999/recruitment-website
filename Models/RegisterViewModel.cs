using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace recruitment_website.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // "candidate" | "employer" — dùng RadioButton/Select trong View

        // --- Chỉ cần khi Role = candidate ---
        public string FullName { get; set; }

        // --- Chỉ cần khi Role = employer ---
        public string CompanyName { get; set; }
        public string TaxCode { get; set; }
    }
}