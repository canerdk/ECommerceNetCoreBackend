using Entities.Concrete;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.ElasticSearchOptions.Concrete
{
    public static class Mapping
    {
        public static CreateIndexDescriptor ProductMapping(this CreateIndexDescriptor descriptor)
        {
            return descriptor.Map<Product>(p => p.Properties(p => p.Keyword(k => k.Name(n => n.Id)).Text(t => t.Name(n => n.Name)).Text(t => t.Name(n => n.Code)).Number(t => t.Name(n => n.UnitPrice))));
        }
    }
}
