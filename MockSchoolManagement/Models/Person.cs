using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Models
{
    public abstract class Person
    {
        public int Id { get; set; }

        [Required]
        [Display(Name ="姓名")]
        [StringLength(50)]
        public string Name { get; set; }

        [Display(Name ="电子邮箱")]
        public string Email { get; set; }
    }
}
