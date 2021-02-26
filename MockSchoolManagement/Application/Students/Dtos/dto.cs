﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Application.Students.Dtos
{
    /// <summary>
    /// 入学时间分组
    /// </summary>
    public class EnrollmentDateGroupDto
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}", ApplyFormatInEditMode =true)]
        public DateTime? EnrollmentDate { get; set; }
        public int StudentCount { get; set; }
    }
}
