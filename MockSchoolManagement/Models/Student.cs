using MockSchoolManagement.Models.EnumTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MockSchoolManagement.Models
{
    public class Student : Person
    {
        /// <summary>
        /// 使用可空属性，当前台提交的值为空“请选择”时就不会出错。
        /// </summary>
        public MajorEnum? Major { get; set; }

        public string PhotoPath { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }

        /// <summary>
        /// 入学时间
        /// </summary>
        public DateTime EnrollmentDate { get; set; }

        public ICollection<StudentCourse> StudentCourses { get; set; }
    }
}
