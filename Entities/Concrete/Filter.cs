using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Concrete
{
    public class Filter : IEntity
    {
        public List<ParentCategory> ParentCategories { get; set; }
        public List<Color> Colors { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string MinDate { get; set; }
        public string MaxDate { get; set; }
    }
}
