using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Concrete
{
    public class ProductResponse : IEntity
    {
        public List<Product> Products { get; set; }
        public Filter Filter { get; set; }
        public Pagination Pagination { get; set; }
    }
}
