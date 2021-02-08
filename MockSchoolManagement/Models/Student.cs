using MockSchoolManagement.Models.EnumTypes;

namespace MockSchoolManagement.Models
{
    public class Student
    {
        public int Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 使用可空属性，当前台提交的值为空“请选择”时就不会出错。
        /// </summary>
        public MajorEnum? Major { get; set; }

        public string Email { get; set; }

        public string PhotoPath { get; set; }
    }
}
