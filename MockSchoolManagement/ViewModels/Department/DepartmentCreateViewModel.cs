using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.ViewModels.Department
{
    public class DepartmentCreateViewModel
    {
        public int DepartmentID { get; set; }
        [StringLength(50,MinimumLength =3)]
        [Display(Name ="学院名称")]
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name ="预算")]
        public decimal Budget { get; set; }


        /// <summary>
        /// 成立时间
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}",ApplyFormatInEditMode =true)]
        [Display(Name="成立时间")]
        public DateTime StartDate { get; set; }

        public int? TeacherID { get; set; }

        /// <summary>
        /// 学院主任
        /// </summary>
        public MockSchoolManagement.Models.Teacher Administrator { get; set; }

        [Display(Name="负责人")]
        public SelectList TeacherList { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
