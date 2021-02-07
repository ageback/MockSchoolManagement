using MockSchoolManagement.Models;
using MockSchoolManagement.Models.EnumTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.DataRepositories
{
    public class MockStudentRepository : IStudentRepository
    {
        private List<Student> _studentList;

        public MockStudentRepository()
        {
            _studentList = new List<Student>()
            {
                new Student() { Id = 1, Name = "张三", Major =MajorEnum.ComputerScience, Email="zs@yahoo.com.cn" },
                new Student() { Id = 2, Name = "李四", Major = MajorEnum.Mathematics, Email="ls@yahoo.com.cn" },
                new Student() { Id = 3, Name = "王五", Major = MajorEnum.ElectronicCommerce, Email="ww@yahoo.com.cn" }
            };

        }

        public Student Insert(Student student)
        {
            student.Id = GetNewStudentId();
            _studentList.Add(student);
            return student;
        }

        private int GetNewStudentId()
        {
            return _studentList.Max(s => s.Id) + 1;
        }

        public Student Delete(int id)
        {
            Student student = _studentList.FirstOrDefault(s => s.Id == id);
            if (student != null)
            {
                _studentList.Remove(student);
            }
            return student;
        }

        public Student GetStudent(int id)
        {
            return _studentList.FirstOrDefault(a => a.Id == id);
        }        

        public Student Update(Student updateStudent)
        {
            Student student = _studentList.FirstOrDefault(s => s.Id == updateStudent.Id);
            if (student != null)
            {
                student.Name = updateStudent.Name;
                student.Email = updateStudent.Email;
                student.Major = updateStudent.Major;
            }
            return student;
        }

        IEnumerable<Student> IStudentRepository.GetAllStudents()
        {
            return _studentList;
        }
    }
}
