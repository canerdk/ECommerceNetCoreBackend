using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Concrete
{
    public class OrderCheck : IEntity
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
