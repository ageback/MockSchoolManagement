using Microsoft.AspNetCore.Mvc;
using MockSchoolManagement.DataRepositories;
using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Controllers
{
    public class StudentController:Controller
    {
        private IStudentRepository studentRepository;
        public StudentController(IStudentRepository studentRepository)
        {
            this.studentRepository = studentRepository;
        }

        public IActionResult Details(int id)
        {
            Student model = studentRepository.GetStudentById(id);
            return View(model);
        }

        public JsonResult DetailsJson()
        {
            Student model = studentRepository.GetStudentById(2);
            return Json(model);
        }
    }
}
