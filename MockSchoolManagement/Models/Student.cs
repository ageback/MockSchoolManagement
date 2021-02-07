using MockSchoolManagement.Models.EnumTypes;
using System.ComponentModel.DataAnnotations;

namespace MockSchoolManagement.Models
{
    public class Student
    {
        public int Id { get; set; }
        [Display(Name ="姓名")]
        [Required(ErrorMessage ="请输入姓名，它不能为空！")]
        public string Name { get; set; }
        [Display(Name = "主修科目")]
        public MajorEnum Major { get; set; }
        [Display(Name = "电子邮箱")]
        [Required(ErrorMessage = "请输入邮箱地址，它不能为空！")]
        public string Email { get; set; }
    }
}
