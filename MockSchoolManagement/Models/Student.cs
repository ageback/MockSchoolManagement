using MockSchoolManagement.Models.EnumTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public MajorEnum Major { get; set; }
        
    }
}
