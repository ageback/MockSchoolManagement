using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MockSchoolManagement.DataRepositories;
using MockSchoolManagement.Models;
using MockSchoolManagement.ViewModels;

namespace MockSchoolManagement.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly IStudentRepository _studentRepository;

        public HomeController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

       
        /// <summary>
        /// 从控制器中返回协商内容
        /// </summary>
        //public ObjectResult Details()
        //{
        //    Student model = _studentRepository.GetStudent(1);
        //    return new ObjectResult(model);
        //}

        /// <summary>
        /// 使用 ViewData或ViewBag 传递数据到View
        /// </summary>
        /// <returns></returns>
        public ViewResult ViewDetails(int? id)
        {
            Student model = _studentRepository.GetStudent(id??1);
            ViewBag.PageTitle = "学生详情";
            return View(model);
        }

        /// <summary>
        /// 在视图中使用 ViewModel
        /// </summary>
        /// <returns></returns>
        public ViewResult Details(int? id)
        {
            HomeDetailsViewModel model = new HomeDetailsViewModel()
            {
                student = _studentRepository.GetStudent(id ?? 1),
                PageTitle = "学生详情"
            };
            return View(model);
        }

        public string MoDetails(int? id, string name)
        {
            return "id = " + id.Value.ToString() + " 并且 名字 = " + name;
        }

        public ViewResult Index()
        {
            var model = _studentRepository.GetAllStudents();
            return View(model);
        }

        /// <summary>
        /// 创建学生信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public RedirectToActionResult Create(Student student)
        {
            Student newStudent = _studentRepository.Add(student);
            return RedirectToAction("Details", new { id = newStudent.Id });
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }


    }
}
