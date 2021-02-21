using MockSchoolManagement.Application.Dtos;
using MockSchoolManagement.Application.Students.Dtos;
using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Application.Students
{
    public interface IStudentService
    {
        Task<PagedResultDto<Student>> GetPaginatedResult(GetStudentInput input);
    }
}
