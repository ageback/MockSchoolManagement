using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Application.Dtos
{
    public class PagedSortedAndFilterInput
    {
        /// <summary>
        /// 每页分页条数
        /// </summary>
        [Range(0,1000)]
        public int MaxResultCount { get; set; }
        [Range(0,1000)]
        public int CurrentPage { get; set; }
        /// <summary>
        /// 排序字段 ID
        /// </summary>
        public string Sorting { get; set; }
        /// <summary>
        /// 查询名称
        /// </summary>
        public string FilterText { get; set; }
        public PagedSortedAndFilterInput()
        {
            CurrentPage = 1;
            MaxResultCount = 10;
        }
    }
}
