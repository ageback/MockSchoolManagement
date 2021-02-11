using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "请输入邮箱地址，它不能为空！")]
        [EmailAddress]
        [Display(Name ="邮箱地址")]
        public string Email { get; set; }

        [Required(ErrorMessage ="请输入密码")]
        [DataType(DataType.Password)]
        [Display(Name ="密码")]
        public string Password { get; set; }

        [Display(Name ="记住我")]
        public bool RememberMe { get; set; }
    }
}
