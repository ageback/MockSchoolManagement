using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MockSchoolManagement.DataRepositories;
using MockSchoolManagement.Infrastructure.Repositories;
using MockSchoolManagement.Models;
using MockSchoolManagement.Security;
using MockSchoolManagement.ViewModels;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace MockSchoolManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Student,int> _studentRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDataProtector _protector;

        public HomeController(IRepository<Student,int> studentRepository, IWebHostEnvironment webHostEnvironment, IDataProtectionProvider dataProtectionProvider,
            DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            _studentRepository = studentRepository;
            _webHostEnvironment = webHostEnvironment;
            _protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.StudentIdRouteValue);

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
        //public ViewResult ViewDetails(int? id)
        //{
        //    Student model = _studentRepository.GetStudentById(id??1);
        //    ViewBag.PageTitle = "学生详情";
        //    return View(model);
        //}

        /// <summary>
        /// 在视图中使用 ViewModel
        /// </summary>
        /// <returns></returns>
        public ViewResult Details(string id)
        {
            var stu = DecryptedStudent(id);
            if (stu == null)
            {
                ViewBag.ErrorMessage = $"学生Id={id}的信息不存在，请重试。";
                return View("NotFound", id);
            }
            HomeDetailsViewModel model = new HomeDetailsViewModel()
            {
                Student = stu,
                PageTitle = "学生详情"
            };
            model.Student.EncryptedId = _protector.Protect(stu.Id.ToString());
            return View(model);
        }

        private Student DecryptedStudent(string id)
        {
            return _studentRepository.FirstOrDefault(s => s.Id == Convert.ToInt32(_protector.Unprotect(id)));
        }

        public IActionResult Index(int? pageNumber, int pageSize = 10, string sortBy = "Id")
        {
            IQueryable<Student> query = _studentRepository.GetAll().OrderBy(sortBy).AsNoTracking();
            List<Student> model = query.ToList().Select(s =>
            {
                s.EncryptedId = _protector.Protect(s.Id.ToString());
                return s;
            }).ToList();

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
                string uniqueFileName = ProcessUploadedFile(model);
                Student newStudent = new Student
                {
                    Name = model.Name,
                    Email = model.Email,
                    Major = model.Major,
                    // 将文件名保存在Student对象的PhotoPath属性中，它将被保存到数据库的Students的表中
                    PhotoPath = uniqueFileName,
                    EnrollmentDate=model.EnrollmentDate
                };
                    
                _studentRepository.Insert(newStudent);
                
                return RedirectToAction("Details", new { id = _protector.Protect(newStudent.Id.ToString()) });
            }
            return View();
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }
        [HttpGet]
        public ViewResult Edit(string id)
        {
            Student student = DecryptedStudent(id);
            if (student == null)
            {
                ViewBag.ErrorMessage = $"学生Id={id}的信息不存在，请重试。";
                return View("NotFound", id);
            }
            StudentEditViewModel studentEditViewModel = new StudentEditViewModel
            {
                Id = id,
                Name = student.Name,
                Email = student.Email,
                Major = student.Major,
                ExistingPhotoPath = student.PhotoPath,
                EnrollmentDate=student.EnrollmentDate
            };
            return View(studentEditViewModel);
        }
        [HttpPost]
        public IActionResult Edit(StudentEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Student student = DecryptedStudent(model.Id);
                student.Name = model.Name;
                student.Email = model.Email;
                student.Major = model.Major;
                student.EnrollmentDate = model.EnrollmentDate;
                if (model.Photos != null && model.Photos.Count > 0)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "avatars", model.ExistingPhotoPath);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                    student.PhotoPath = ProcessUploadedFile(model);
                }
                Student updatedStudent = _studentRepository.Update(student);
                return RedirectToAction("index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _studentRepository.FirstOrDefaultAsync(a => a.Id == id);
            if (student == null)
            {
                ViewBag.ErrorMessage = $"无法找到ID为{id}的学生信息。";
                return View("NotFound");
            }
            await _studentRepository.DeleteAsync(a => a.Id == id);
            return RedirectToAction("index");
        }


        /// <summary>
        /// 将图片保存到指定的路径中，并返回唯一的文件名
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string ProcessUploadedFile(StudentCreateViewModel model)
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
