using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.ElasticSearchOptions.Concrete
{
    public class ElasticEntity
    {
        public int Id { get; set; }
        public CompletionField Suggest { get; set; }
        public string SearchingArea { get; set; }
        public double? Score { get; set; }
    }
}
