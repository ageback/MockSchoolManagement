using MockSchoolManagement.Application.Departments.Dtos;
using MockSchoolManagement.Application.Dtos;
using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Application.Departments
{
    public interface IDepartmentsService
    {
        Task<PagedResultDto<Department>> GetPagedDepartmentsList(GetDepartmentInput input);
    }
}
