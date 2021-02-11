using Microsoft.AspNetCore.Mvc;
using MockSchoolManagement.CustomerMiddlewares.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.ViewModels
{
    public class RegisterViewModel
    {
        [EmailAddress]
        [Display(Name ="邮箱地址")]
        [Required(ErrorMessage = "请输入邮箱地址，它不能为空！")]
        [Remote(action:"IsEmailInUse",controller:"Account")]
        [ValidEmailDomain(allowedDomail:"52abp.com",ErrorMessage ="邮箱的域名必须是52abp.com")]
        public string Email { get; set; }

        [Required(ErrorMessage ="请输入密码")]
        [DataType(DataType.Password)]
        [Display(Name ="密码")]
        public string Password { get; set; }

        [Required(ErrorMessage ="请输入确认密码")]
        [DataType(DataType.Password)]
        [Display(Name ="确认密码")]
        [Compare("Password",ErrorMessage ="密码与确认密码不一致，请重新输入。")]
        public string ConfirmPassword { get; set; }
    }
}
