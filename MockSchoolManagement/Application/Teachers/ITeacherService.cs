using MockSchoolManagement.Application.Dtos;
using MockSchoolManagement.Application.Teachers.Dtos;
using MockSchoolManagement.Models;
using System.Threading.Tasks;

namespace MockSchoolManagement.Application.Teachers
{
    public interface ITeacherService
    {
        Task<PagedResultDto<Teacher>> GetPagedTeacherList(GetTeacherInput input);
    }
}