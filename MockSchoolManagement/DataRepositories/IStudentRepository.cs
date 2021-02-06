using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.DataRepositories
{
    public interface IStudentRepository
    {
        Student GetStudent(int id);
        IEnumerable<Student> GetAllStudents();
        void Save(Student student);
        Student Add(Student student);
    }
}
