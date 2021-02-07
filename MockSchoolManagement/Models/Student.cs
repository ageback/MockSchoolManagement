using MockSchoolManagement.Models.EnumTypes;
using System.ComponentModel.DataAnnotations;

namespace MockSchoolManagement.Models
{
    public class Student
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="请输入姓名，它不能为空！")]
        public string Name { get; set; }
        public MajorEnum Major { get; set; }        
        public string Email { get; set; }
    }
}
