using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Application.Teachers;
using MockSchoolManagement.Application.Teachers.Dtos;
using MockSchoolManagement.Infrastructure.Repositories;
using MockSchoolManagement.Models;
using MockSchoolManagement.ViewModels;
using MockSchoolManagement.ViewModels.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly IRepository<Teacher,int> _teacherRepository;
        private readonly IRepository<Course,int> _courseRepository;

        public TeacherController(ITeacherService teacherService, IRepository<Course,int> courseRepository,IRepository<Teacher,int> teacherRepository)
        {
            _teacherService = teacherService;
            _courseRepository = courseRepository;
            _teacherRepository = teacherRepository; 
        }
        public async Task<IActionResult> Index(GetTeacherInput input)
        {
            var models = await _teacherService.GetPagedTeacherList(input);
            var dto = new TeacherListViewModel();
            if (input.Id != null)
            {
                var teacher = models.Data.FirstOrDefault(a => a.Id == input.Id.Value);
                if (teacher != null)
                {
                    dto.Courses = teacher.CourseAssignments.Select(a => a.Course).ToList();
                }
                dto.SelectedId = input.Id.Value;
            }
            if (input.CourseId.HasValue)
            {
                // 查询该课程下有多少学生报名
                var course = dto.Courses.FirstOrDefault(a => a.CourseID == input.CourseId.Value);
                if (course != null)
                {
                    dto.StudentCourses = course.StudentCourses.ToList();
                }
                dto.SelectedCourseId = input.CourseId.Value;
            }
            dto.Teachers = models;
            return View(dto);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            var model = await _teacherRepository.GetAll().Include(a => a.OfficeLocation).Include(a => a.CourseAssignments)
                .ThenInclude(a => a.Course).AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            if (model == null)
            {
                ViewBag.ErrorMessage = $"教师信息ID为{id}的信息不存在，请重试。";
                return View("NotFound");
            }
            var dto = new TeacherCreateViewModel
            {
                Name = model.Name,
                Id = model.Id,
                HireDate = model.HireDate,
                OfficeLocation = model.OfficeLocation
            };
            var assignedCourses = AssignedCourseDropDownList(model);
            dto.AssignedCourses = assignedCourses;
            return View(dto);
        }

        private List<AssignedCourseViewModel> AssignedCourseDropDownList(Teacher teacher)
        {
            var allCourses = _courseRepository.GetAllList();
            var teacherCourses = new HashSet<int>(teacher.CourseAssignments.Select(c => c.CourseID));
            var viewModel = new List<AssignedCourseViewModel>();
            foreach(var course in allCourses)
            {
                viewModel.Add(new AssignedCourseViewModel
                {
                    CourseID = course.CourseID,
                    Title=course.Title,
                    IsSelected=teacherCourses.Contains(course.CourseID)
                });
            }
            return viewModel;
        }
    }
}
