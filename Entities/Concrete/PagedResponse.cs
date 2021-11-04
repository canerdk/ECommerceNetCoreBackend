using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Concrete
{
    public class PagedResponse<T> : IEntity
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public List<Color> Colors { get; set; }
        public List<ParentCategory> ParentCategories { get; set; }
        public List<T> Data { get; set; }
    }
}
