using System;
using System.Collections.Generic;
using System.Text;

namespace Business.ElasticSearchOptions.Dtos
{
    public class ProductElasticIndexDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public int UnitsInStock { get; set; }
        public string Code { get; set; }
        public string Color { get; set; }
    }
}
