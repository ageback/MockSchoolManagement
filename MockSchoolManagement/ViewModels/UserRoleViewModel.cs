using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.ViewModels
{
    public class UserRoleViewModel
    {

        public string UserId { get; set; }
        public string UserName { get; set; }
        /// <summary>
        /// 是否选择用户作为角色的成员
        /// </summary>
        public bool IsSelected { get; set; }
    }
}
