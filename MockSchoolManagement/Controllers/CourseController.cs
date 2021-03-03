using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Application.Courses;
using MockSchoolManagement.Application.Courses.Dtos;
using MockSchoolManagement.DataRepositories;
using MockSchoolManagement.Infrastructure;
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
        private readonly IRepository<CourseAssignment, int> _courseAssignmentRepository;
        private readonly AppDbContext _dbContext;

        public CourseController(ICourseService courseService, IRepository<Course, int> courseRepository,IRepository<Department, int> departmentRepository,
            IRepository<CourseAssignment, int> courseAssignmentRepository,AppDbContext dbContext)
        {
            _courseService = courseService;
            _courseRepository = courseRepository;
            _departmentRepository = departmentRepository;
            _courseAssignmentRepository = courseAssignmentRepository;
            _dbContext = dbContext;
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

        public async Task<ViewResult> Details(int courseId)
        {
            var course = await _courseRepository.GetAll().Include(a=>a.Department).FirstOrDefaultAsync(a => a.CourseID == courseId);
            if (course == null)
            {
                ViewBag.ErrorMessage = $"课程编号{courseId}的信息不存在，请重试。";
                return View("NotFound");
            }
            return View(course);
        }
        private SelectList DepartmentsDropDownList(object selectedDepartment = null)
        {
            var models = _departmentRepository.GetAll().OrderBy(a => a.Name).AsNoTracking().ToList();
            var dtos = new SelectList(models, "DepartmentID", "Name", selectedDepartment);
            return dtos;
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int courseId)
        {
            var model = await _courseRepository.FirstOrDefaultAsync(a => a.CourseID == courseId);
            if (model == null)
            {
                return CourseNotFoundError(courseId);
            }
            await _courseAssignmentRepository.DeleteAsync(a => a.CourseID == model.CourseID);
            await _courseRepository.DeleteAsync(a => a.CourseID == courseId);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult UpdateCourseCredits()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCourseCredits(int? multiplier)
        {
            if (multiplier != null)
            {
                ViewBag.RowsAffected = await _dbContext.Database.ExecuteSqlRawAsync("UPDATE School.Course SET Credits=Credits*{0}", parameters: multiplier);
            }
            return View();
        }
    }
}
