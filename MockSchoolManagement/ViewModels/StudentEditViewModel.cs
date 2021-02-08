using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.ViewModels
{
    public class StudentEditViewModel : StudentCreateViewModel
    {
        public int Id { get; set; }

        /// <summary>
        /// 已经存在的头像图片文件路径
        /// </summary>
        public string ExistingPhotoPath { get; set; }
    }
}
