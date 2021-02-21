using MockSchoolManagement.Application.Courses.Dtos;
using MockSchoolManagement.Application.Dtos;
using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Application.Courses
{
    public interface ICourseService
    {
        Task<PagedResultDto<Course>> GetPaginatedResult(GetCourseInput input);
    }
}
