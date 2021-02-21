using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockSchoolManagement.Application.Dtos;
namespace MockSchoolManagement.Application.Students.Dtos
{
    public class GetStudentInput : PagedSortedAndFilterInput
    {
        public GetStudentInput()
        {
            Sorting = "Id";
        }
    }
}
