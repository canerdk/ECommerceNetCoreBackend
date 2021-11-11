using Business.ElasticSearchOptions.Dtos;
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
            return descriptor.Map<ProductElasticIndexDto>(p => p.Properties(p => p.Keyword(k => k.Name(n => n.Id)).Text(t => t.Name(n => n.Name).Analyzer("turkish_analyzer")).Text(t => t.Name(n => n.Code).Analyzer("turkish_analyzer")).Text(t => t.Name(n => n.Color).Analyzer("turkish_analyzer"))));
        }
    }
}
