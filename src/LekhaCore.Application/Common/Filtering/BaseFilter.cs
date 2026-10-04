using LekhaCore.Application.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace LekhaCore.Application.Common.Filtering
{
    public abstract class BaseFilter : PaginationRequest
    {
        public string? Search { get; set; }
        public string? SortBy { get; set; }
        public bool Descending { get; set; }
    }
}
