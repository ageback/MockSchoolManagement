using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MockSchoolManagement.DataRepositories;
using MockSchoolManagement.Models;
using MockSchoolManagement.ViewModels;

namespace MockSchoolManagement.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(IStudentRepository studentRepository, IWebHostEnvironment webHostEnvironment)
        {
            _studentRepository = studentRepository;
            _webHostEnvironment = webHostEnvironment;
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
            Student model = _studentRepository.GetStudentById(id??1);
            ViewBag.PageTitle = "学生详情";
            return View(model);
        }

        /// <summary>
        /// 在视图中使用 ViewModel
        /// </summary>
        /// <returns></returns>
        public ViewResult Details(int? id)
        {
            throw new Exception("故意抛出异常");
            var stu = _studentRepository.GetStudentById(id);
            if (stu == null)
            {
                Response.StatusCode = 404;
                return View("StudentNotFound", id);
            }
            HomeDetailsViewModel model = new HomeDetailsViewModel()
            {
                Student = stu,
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
        public IActionResult Create(StudentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (model.Photos != null && model.Photos.Count > 0)
                {
                    foreach (IFormFile photo in model.Photos)
                    {
                        // 必须将图片文件上传到wwwroot/images中
                        // 而要获取wwwroot文件夹的路径，需要注入WebHostEnvironment服务
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "avatars");

                        // 确保文件名是唯一
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // 使用 IFormFile 接口提供的CopyTo()方法将文件复制到wwwroot/images文件夹
                        photo.CopyTo(new FileStream(filePath, FileMode.Create));
                    }
                }
                Student newStudent = new Student
                {
                    Name = model.Name,
                    Email = model.Email,
                    Major = model.Major,
                    // 将文件名保存在Student对象的PhotoPath属性中，它将被保存到数据库的Students的表中
                    PhotoPath = uniqueFileName
                };
                    
                _studentRepository.Insert(newStudent);
                return RedirectToAction("Details", new { id = newStudent.Id });
            }
            return View();
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }
        [HttpGet]
        public ViewResult Edit(int id)
        {
            Student student = _studentRepository.GetStudentById(id);
            StudentEditViewModel studentEditViewModel = new StudentEditViewModel
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Major = student.Major,
                ExistingPhotoPath = student.PhotoPath
            };
            return View(studentEditViewModel);
        }
        [HttpPost]
        public IActionResult Edit(StudentEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Student student = _studentRepository.GetStudentById(model.Id);
                student.Name = model.Name;
                student.Email = model.Email;
                student.Major = model.Major;
                if (model.Photos.Count > 0)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "avatars", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    student.PhotoPath = ProcessUploadedFile(model);
                }
                Student updatedStudent = _studentRepository.Update(student);
                return RedirectToAction("index");
            }
            return View(model);
        }


        /// <summary>
        /// 将图片保存到指定的路径中，并返回唯一的文件名
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string ProcessUploadedFile(StudentEditViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photos.Count > 0)
            {
                foreach (var photo in model.Photos)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "avatars");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    // 非托管方法，需要手工释放资源
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        photo.CopyTo(fileStream);
                    }
                }
            }
            return uniqueFileName;
        }
    }
}
