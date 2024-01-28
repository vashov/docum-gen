using System;
using System.Collections.Generic;

namespace DocumGen.Application.Contracts.Pagination
{
    public class PageResult<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public List<T> Data { get; set; }
    }
}
