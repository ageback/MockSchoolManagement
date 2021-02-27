﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Infrastructure.Data
{
    public static class DataInitializer
    {
        public static IApplicationBuilder UseDataInitializer(this IApplicationBuilder builder)
        {
            using (var scope = builder.ApplicationServices.CreateScope())
            {
                var dbcontext = scope.ServiceProvider.GetService<AppDbContext>();
                var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                #region 学生种子信息
                if (dbcontext.Students.Any())
                {
                    return builder;
                }
                var students = new[]
                {
                    new Student{Name="张三",Major=Models.EnumTypes.MajorEnum.ComputerScience,Email="zhangsan@52apb.com",EnrollmentDate=DateTime.Parse("2016-09-01")},
                    new Student{Name="李四",Major=Models.EnumTypes.MajorEnum.Mathematics,Email="lisi@52apb.com",EnrollmentDate=DateTime.Parse("2017-09-01")},
                    new Student{Name="王五",Major=Models.EnumTypes.MajorEnum.ElectronicCommerce,Email="wangwu@52apb.com",EnrollmentDate=DateTime.Parse("2012-09-01")},
                };
                foreach (Student item in students)
                {
                    dbcontext.Students.Add(item);
                }
                dbcontext.SaveChanges();
                #endregion 学生种子信息
                #region 老师种子数据

                var teachers = new[] {
                    new Teacher { Name = "孔子", HireDate = DateTime.Parse ("1995-03-11") },
                    new Teacher { Name = "墨子", HireDate = DateTime.Parse ("2003-03-11") },
                    new Teacher { Name = "荀子", HireDate = DateTime.Parse ("1990-03-11") },
                    new Teacher { Name = "鬼谷子", HireDate = DateTime.Parse ("1985-03-11") },
                    new Teacher { Name = "孟子", HireDate = DateTime.Parse ("2003-03-11") },
                    new Teacher { Name = "朱熹", HireDate = DateTime.Parse ("2003-03-11") }
                };

                foreach (var i in teachers)
                    dbcontext.Teachers.Add(i);
                dbcontext.SaveChanges();

                #endregion 老师种子数据

                #region 学院种子数据
                var departments = new[] {
                    new Department { Name = "论语", Budget = 350000, StartDate = DateTime.Parse ("2017-09-01"), TeacherID = teachers.Single (i => i.Name == "孟子").Id },
                    new Department { Name = "兵法", Budget = 100000, StartDate = DateTime.Parse ("2017-09-01"), TeacherID = teachers.Single (i => i.Name == "鬼谷子").Id },
                    new Department { Name = "文言文", Budget = 350000, StartDate = DateTime.Parse ("2017-09-01"), TeacherID = teachers.Single (i => i.Name == "朱熹").Id },
                    new Department { Name = "墨学", Budget = 100000, StartDate = DateTime.Parse ("2017-09-01"), TeacherID = teachers.Single (i => i.Name == "墨子").Id }
                };

                foreach (var d in departments)
                    dbcontext.Departments.Add(d);
                dbcontext.SaveChanges();
                #endregion 部门种子数据
                #region 课程种子数据 
                if (dbcontext.Courses.Any())
                {
                    return builder; // 数据已经初始化了。
                }
                var courses = new[] {
                    new Course { CourseID = 1050, Title = "数学", Credits = 3, DepartmentID = departments.Single (s => s.Name == "兵法").DepartmentID },
                    new Course { CourseID = 4022, Title = "政治", Credits = 3, DepartmentID = departments.Single (s => s.Name == "文言文").DepartmentID },
                    new Course { CourseID = 4041, Title = "物理", Credits = 3, DepartmentID = departments.Single (s => s.Name == "兵法").DepartmentID },
                    new Course { CourseID = 1045, Title = "化学", Credits = 4, DepartmentID = departments.Single (s => s.Name == "墨学").DepartmentID },
                    new Course { CourseID = 3141, Title = "生物", Credits = 4, DepartmentID = departments.Single (s => s.Name == "论语").DepartmentID },
                    new Course { CourseID = 2021, Title = "英语", Credits = 3, DepartmentID = departments.Single (s => s.Name == "论语").DepartmentID },
                    new Course { CourseID = 2042, Title = "历史", Credits = 4, DepartmentID = departments.Single (s => s.Name == "文言文").DepartmentID }
                };

                foreach (var c in courses)
                    dbcontext.Courses.Add(c);
                dbcontext.SaveChanges();

                #endregion 课程种子数据
                #region 学生课程关联种子数据
                var studentCourses = new[] {
                    new StudentCourse { StudentID = students.Single (s => s.Name == "张三").Id, CourseID = courses.Single (c => c.Title == "数学").CourseID, Grade = Grade.A },

                };
                foreach (var sc in studentCourses)
                {
                    dbcontext.StudentCourses.Add(sc);
                }
                dbcontext.SaveChanges();
                #endregion 学生课程关联种子数据

               


                #region 办公室分配的种子数据

                var OfficeLocations = new[] {
                    new OfficeLocation { TeacherId = teachers.Single (i => i.Name == "孟子").Id, Location = "逸夫楼 17" },
                    new OfficeLocation { TeacherId = teachers.Single (i => i.Name == "朱熹").Id, Location = "青霞路 27" },
                    new OfficeLocation { TeacherId = teachers.Single (i => i.Name == "墨子").Id, Location = "天府楼 304" }
                };

                foreach (var o in OfficeLocations)
                    dbcontext.OfficeLocations.Add(o);
                dbcontext.SaveChanges();

                #endregion
                #region 为老师分配课程的种子数据

                var coursetTeachers = new[] {
                    new CourseAssignment { CourseID = courses.Single (c => c.Title == "数学").CourseID, TeacherID = teachers.Single (i => i.Name == "鬼谷子").Id },
                    new CourseAssignment { CourseID = courses.Single (c => c.Title == "数学").CourseID, TeacherID = teachers.Single (i => i.Name == "墨子").Id },
                    new CourseAssignment { CourseID = courses.Single (c => c.Title == "政治").CourseID, TeacherID = teachers.Single (i => i.Name == "朱熹").Id },
                    new CourseAssignment { CourseID = courses.Single (c => c.Title == "化学").CourseID, TeacherID = teachers.Single (i => i.Name == "墨子").Id },
                    new CourseAssignment { CourseID = courses.Single (c => c.Title == "生物").CourseID, TeacherID = teachers.Single (i => i.Name == "孟子").Id },
                    new CourseAssignment { CourseID = courses.Single (c => c.Title == "英语").CourseID, TeacherID = teachers.Single (i => i.Name == "孟子").Id },
                    new CourseAssignment { CourseID = courses.Single (c => c.Title == "物理").CourseID, TeacherID = teachers.Single (i => i.Name == "鬼谷子").Id },
                    new CourseAssignment { CourseID = courses.Single (c => c.Title == "历史").CourseID, TeacherID = teachers.Single (i => i.Name == "朱熹").Id }

                };

                foreach (var ci in coursetTeachers)
                    dbcontext.CourseAssignments.Add(ci);
                dbcontext.SaveChanges();

                #endregion

                #region 用户种子数据
                if (dbcontext.Users.Any()) { return builder; }
                var user = new ApplicationUser { Email = "lmh@52abp.com", UserName = "lmh@52abp.com", EmailConfirmed = true, City = "无锡" };
                userManager.CreateAsync(user, "12345").Wait();
                dbcontext.SaveChanges();
                var adminRole = "Admin";
                var role = new IdentityRole { Name = adminRole };

                dbcontext.Roles.Add(role);
                dbcontext.SaveChanges();
                dbcontext.UserRoles.Add(new IdentityUserRole<string> { RoleId = role.Id, UserId = user.Id });
                dbcontext.SaveChanges();
                #endregion 用户种子数据
            }
            return builder;
        }
    }
}
