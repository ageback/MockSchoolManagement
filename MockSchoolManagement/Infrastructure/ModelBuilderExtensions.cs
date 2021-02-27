using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Infrastructure
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Student>().HasData(
            //    new Student { Id = 1, Name = "花老虎", Major = Models.EnumTypes.MajorEnum.ComputerScience, Email = "lmh@xxx.ccc" }
            //    );

            //modelBuilder.Entity<Student>().HasData(
            //    new Student { Id = 2, Name = "张三", Major = Models.EnumTypes.MajorEnum.ElectronicCommerce, Email = "zs@xxx.ccc" }
            //    );

            modelBuilder.Entity<Course>().ToTable("Course", "School");
            modelBuilder.Entity<StudentCourse>().ToTable("StudentCourse", "School");
            modelBuilder.Entity<Student>().ToTable("Student", "School");
            modelBuilder.Entity<CourseAssignment>().HasKey(c => new { c.CourseID, c.TeacherID });
        }
    }
}
