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
    [Route("Home")]
    public class HomeController : Controller
    {
        private readonly IStudentRepository _studentRepository;

        public HomeController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        [Route("Details")]
        /// <summary>
        /// 从控制器中返回协商内容
        /// </summary>
        public ObjectResult Details()
        {
            Student model = _studentRepository.GetStudent(1);
            return new ObjectResult(model);
        }

        /// <summary>
        /// 使用 ViewData或ViewBag 传递数据到View
        /// </summary>
        /// <returns></returns>
        [Route("ViewDetails/{id?}")]
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
        
        [Route("VMDetails/{id?}")]
        public ViewResult VMDetails(int? id)
        {
            HomeVMDetailsViewModel model = new HomeVMDetailsViewModel()
            {
                student = _studentRepository.GetStudent(id??1),
                PageTitle = "学生详情"
            };
            return View(model);
        }

        [Route("/")]
        [Route("")]
        [Route("Index")]
        public ViewResult Index()
        {
            var model = _studentRepository.GetAllStudents();
            return View(model);
        }
    }
}
