using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MockSchoolManagement.DataRepositories;
using MockSchoolManagement.Models;

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
        public ObjectResult Details()
        {
            Student model = _studentRepository.GetStudent(1);
            return new ObjectResult(model);
        }

        /// <summary>
        /// 使用 ViewData或ViewBag 传递数据到View
        /// </summary>
        /// <returns></returns>
        public ViewResult ViewDetails()
        {
            Student model = _studentRepository.GetStudent(1);
            ViewBag.PageTitle = "学生详情";
            return View(model);
        }

        public string Index()
        {
            return _studentRepository.GetStudent(1).Name;
        }
    }
}
