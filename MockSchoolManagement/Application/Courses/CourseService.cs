using MockSchoolManagement.Application.Courses.Dtos;
using MockSchoolManagement.Application.Dtos;
using MockSchoolManagement.Infrastructure.Repositories;
using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace MockSchoolManagement.Application.Courses
{
    public class CourseService : ICourseService
    {
        private readonly IRepository<Course, int> _courseRepository;
        public CourseService(IRepository<Course, int> courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<PagedResultDto<Course>> GetPaginatedResult(GetCourseInput input)
        {
            var query = _courseRepository.GetAll();
            var count = query.Count();
            query = query.OrderBy(input.Sorting).Skip((input.CurrentPage - 1) * input.MaxResultCount).Take(input.MaxResultCount);
            var models = await query.AsNoTracking().ToListAsync();
            var dtos = new PagedResultDto<Course>
            {
                TotalCount = count,
                CurrentPage = input.CurrentPage,
                MaxResultCount = input.MaxResultCount,
                Data = models,
                FilterText = input.FilterText,
                Sorting = input.Sorting
            };
            return dtos;
        }
    }
}
