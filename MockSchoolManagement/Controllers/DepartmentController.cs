using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Application.Departments;
using MockSchoolManagement.Application.Departments.Dtos;
using MockSchoolManagement.Infrastructure;
using MockSchoolManagement.Infrastructure.Repositories;
using MockSchoolManagement.Models;
using MockSchoolManagement.ViewModels.Department;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IRepository<Department, int> _departmentRepository;
        private readonly IRepository<Teacher, int> _teacherRepository;
        private readonly IDepartmentsService _departmentsService;
        private readonly AppDbContext _dbContext;

        public DepartmentController(IRepository<Department, int> departmentRepository,
                                    IRepository<Teacher, int> teacherRepository,
                                    IDepartmentsService departmentsService,
                                    AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _departmentRepository = departmentRepository;
            _departmentsService = departmentsService;
            _teacherRepository = teacherRepository;
        }
        public async Task<IActionResult> Index(GetDepartmentInput input)
        {
            var models = await _departmentsService.GetPagedDepartmentsList(input);
            return View(models);
        }


        private SelectList TeachersDropDownList(object selectedTeacher = null)
        {
            var models = _teacherRepository.GetAll().OrderBy(a => a.Name).AsNoTracking().ToList();
            return new SelectList(models, "Id", "Name", selectedTeacher); 
        }

        public IActionResult Create()
        {
            var dto = new DepartmentCreateViewModel { TeacherList = TeachersDropDownList() };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DepartmentCreateViewModel input)
        {
            if (ModelState.IsValid)
            {
                Department model = new Department
                {
                    StartDate = input.StartDate,
                    DepartmentID = input.DepartmentID,
                    TeacherID = input.TeacherID,
                    Budget = input.Budget,
                    Name = input.Name
                };
                await _departmentRepository.InsertAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            var model = await _departmentRepository.GetAll().Include(a => a.Administrator).FirstOrDefaultAsync(a => a.DepartmentID == id);
            if (model == null)
            {
                return DepartmentNotFoundError(id);
            }
            return View(model);
        }
        private IActionResult DepartmentNotFoundError(int? id)
        {
            ViewBag.ErrorMessage = $"学院ID为{id}的信息不存在，请重试。";
            return View("NotFound");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _departmentRepository.FirstOrDefaultAsync(a => a.DepartmentID == id);
            if (model == null)
            {
                return DepartmentNotFoundError(id);
            }
            await _departmentRepository.DeleteAsync(a => a.DepartmentID == id);
            return RedirectToAction(nameof(Index));
        }
    }
}
