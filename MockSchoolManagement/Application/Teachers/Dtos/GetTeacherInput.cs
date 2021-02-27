using MockSchoolManagement.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Application.Teachers.Dtos
{
    public class GetTeacherInput: PagedSortedAndFilterInput
    {
        public int? Id { get; set; }
        public int? CourseId { get; set; }
        public GetTeacherInput()
        {
            Sorting = "Id";
            MaxResultCount = 5;
        }
    }
}
