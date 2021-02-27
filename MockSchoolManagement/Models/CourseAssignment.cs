namespace MockSchoolManagement.Models
{
    public class CourseAssignment
    {
        public int TeacherID { get; set; }
        public int CourseID { get; set; }
        public Teacher Teacher { get; set; }
        public Course Course { get; set; }
    }
}