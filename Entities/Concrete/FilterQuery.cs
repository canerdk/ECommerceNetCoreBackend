using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Concrete
{
    public class FilterQuery : IEntity
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int ParentCategoryId { get; set; }
        public string SearchString { get; set; }
        public List<int> Prices { get; set; }
        public List<string> Colors { get; set; }
    }
}
