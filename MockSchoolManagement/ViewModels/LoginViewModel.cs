using Microsoft.AspNetCore.Authentication;
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
        [EmailAddress(ErrorMessage ="请输入正确的邮箱地址。")]
        [Display(Name ="邮箱地址")]
        public string Email { get; set; }

        [Required(ErrorMessage ="请输入密码")]
        [DataType(DataType.Password)]
        [Display(Name ="密码")]
        public string Password { get; set; }

        [Display(Name ="记住我")]
        public bool RememberMe { get; set; }

        /// <summary>
        /// 第三方登录后重定向到该URL
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// 启用第三方登录列表
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
    }
}
