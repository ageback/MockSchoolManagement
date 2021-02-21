using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Application.Students
{
    public interface IStudentService
    {
        Task<List<Student>> GetPaginatedResult(int currentPage, string searchString, string sortBy, int pageSize = 2);
    }
}
