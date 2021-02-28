using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.ViewModels.Teacher
{
    public class AssignedCourseViewModel
    {
        public int CourseID { get; set; }
        public string Title { get; set; }
        public bool IsSelected { get; set; }
    }
}
