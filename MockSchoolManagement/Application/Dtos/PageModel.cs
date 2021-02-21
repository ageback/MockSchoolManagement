using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Application.Dtos
{
    public class PagedResultDto<TEntity> : PagedSortedAndFilterInput
    {
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(TotalCount, MaxResultCount));
        public List<TEntity> Data { get; set; }
        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;
        public bool ShowFirst => CurrentPage != 1;
        public bool ShowLast => CurrentPage != TotalPages;
    }
}
