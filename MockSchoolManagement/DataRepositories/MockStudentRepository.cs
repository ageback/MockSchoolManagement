using MockSchoolManagement.Models;
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
                new Student() { Id = 1, Name = "张三", Major = "计算机科学" },
                new Student() { Id = 2, Name = "李四", Major = "物流" },
                new Student() { Id = 3, Name = "王五", Major = "电子商务" }
            };

        }


        public Student GetStudent(int id)
        {
            return _studentList.FirstOrDefault(a => a.Id == id);
        }

        public void Save(Student student)
        {
            throw new NotImplementedException();
        }
    }
}
