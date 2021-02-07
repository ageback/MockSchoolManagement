using MockSchoolManagement.Models.EnumTypes;
using System.ComponentModel.DataAnnotations;

namespace MockSchoolManagement.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Display(Name = "姓名")]
        [Required(ErrorMessage = "请输入姓名，它不能为空！")]
        public string Name { get; set; }

        /// <summary>
        /// 使用可空属性，当前台提交的值为空“请选择”时就不会出错。
        /// </summary>
        [Display(Name = "主修科目")]
        [Required(ErrorMessage ="请选择一门科目")]
        public MajorEnum? Major { get; set; }

        [Display(Name = "电子邮箱")]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "邮箱的格式不正确")]
        [Required(ErrorMessage = "请输入邮箱地址，它不能为空！")]
        public string Email { get; set; }

        public string PhotoPath { get; set; }
    }
}
