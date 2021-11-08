﻿using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Business.ElasticSearchOptions.Abstract
{
    public interface IElasticSearchService
    {
        Task CheckIndex(string indexName);
        Task InsertDocument(string indexName, Product product);
        Task InsertDocuments(string indexName, List<Product> products);
        Task<Product> GetDocument(string indexName, int id);
        Task<List<Product>> GetDocuments(string indexName);
    }
}