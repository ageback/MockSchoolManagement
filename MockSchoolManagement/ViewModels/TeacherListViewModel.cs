using MockSchoolManagement.Application.Dtos;
using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.ViewModels
{
    public class TeacherListViewModel
    {
        public PagedResultDto<MockSchoolManagement.Models.Teacher> Teachers { get; set; }
        public List<MockSchoolManagement.Models.Course> Courses { get; set; }
        public List<StudentCourse> StudentCourses { get; set; }

        /// <summary>
        /// 选中的教师ID
        /// </summary>
        public int SelectedId { get; set; }
        /// <summary>
        /// 选中的课程ID
        /// </summary>
        public int SelectedCourseId { get; set; }
    }
}
