using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Application.Courses;
using MockSchoolManagement.Application.Courses.Dtos;
using MockSchoolManagement.DataRepositories;
using MockSchoolManagement.Infrastructure.Repositories;
using MockSchoolManagement.Models;
using MockSchoolManagement.ViewModels.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Controllers
{
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IRepository<Course, int> _courseRepository;
        private readonly IRepository<Department, int> _departmentRepository;

        public CourseController(ICourseService courseService, IRepository<Course, int> courseRepository,IRepository<Department, int> departmentRepository)
        {
            _courseService = courseService;
            _courseRepository = courseRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<ActionResult> Index(GetCourseInput input)
        {
            var models = await _courseService.GetPaginatedResult(input);
            return View(models);
        }

        #region 添加课程
        public IActionResult Create()
        {
            var dtos = DepartmentsDropDownList();
            CourseCreateViewModel courseCreateViewModel = new CourseCreateViewModel
            {
                DepartmentList = dtos
            };
            return View(courseCreateViewModel);
        }



        [HttpPost]
        public async Task<IActionResult> Create(CourseCreateViewModel input)
        {
            if (ModelState.IsValid)
            {
                Course course = new Course
                {
                    CourseID = input.CourseID,
                    Title = input.Title,
                    Credits = input.Credits,
                    DepartmentID = input.DepartmentID
                };
                await _courseRepository.InsertAsync(course);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        #endregion 添加课程

        #region 课程编辑
        public IActionResult Edit(int? courseId)
        {
            if (!courseId.HasValue)
            {
                return CourseNotFoundError(courseId);
            }
            var course = _courseRepository.FirstOrDefault(a => a.CourseID == courseId);
            if (course == null)
            {
                return CourseNotFoundError(courseId);
            }
            var dtos = DepartmentsDropDownList(course.DepartmentID);
            CourseCreateViewModel courseCreateViewModel = new CourseCreateViewModel
            {
                DepartmentList = dtos,
                CourseID = course.CourseID,
                Credits = course.Credits,
                Title = course.Title,
                DepartmentID = course.DepartmentID
            };
            return View(courseCreateViewModel);
        }

        private IActionResult CourseNotFoundError(int? courseId)
        {
            ViewBag.ErrorMessage = $"课程编号{courseId}的信息不存在，请重试。";
            return View("NotFound");
        }

        [HttpPost]
        public IActionResult Edit(CourseCreateViewModel input)
        {
            if (ModelState.IsValid)
            {
                var course = _courseRepository.FirstOrDefault(a => a.CourseID == input.CourseID);
                if (course != null)
                {
                    course.CourseID = input.CourseID;
                    course.Credits = input.Credits;
                    course.DepartmentID = input.DepartmentID;
                    course.Title = input.Title;
                    _courseRepository.Update(course);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return CourseNotFoundError(input.CourseID);
                }
            }
            return View(input);
        }
        #endregion 课程编辑
        private SelectList DepartmentsDropDownList(object selectedDepartment = null)
        {
            var models = _departmentRepository.GetAll().OrderBy(a => a.Name).AsNoTracking().ToList();
            var dtos = new SelectList(models, "DepartmentID", "Name", selectedDepartment);
            return dtos;
        }
    }
}
