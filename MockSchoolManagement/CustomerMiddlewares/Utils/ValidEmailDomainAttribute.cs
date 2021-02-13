﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.CustomerMiddlewares.Utils
{
    public class ValidEmailDomainAttribute : ValidationAttribute
    {
        /// <summary>
        /// 注册时允许使用的邮箱域名
        /// </summary>
        private readonly string _allowedDomail;
        public ValidEmailDomainAttribute(string allowedDomail) => _allowedDomail = allowedDomail;

        public override bool IsValid(object value)
        {
            string[] strings = value.ToString().Split('@');
            return strings[1].ToUpper() == _allowedDomail;
        }
    }
}