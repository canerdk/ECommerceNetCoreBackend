using Business.ElasticSearchOptions.Abstract;
using Entities.Concrete;
using Microsoft.Extensions.Configuration;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ElasticSearchOptions.Concrete
{
    public class ElasticSearchManager : IElasticSearchService
    {
        private readonly IConfiguration _configuration;
        private readonly IElasticClient _client;

        public ElasticSearchManager(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = CreateInstance();
        }

        private ElasticClient CreateInstance()
        {
            string host = _configuration.GetSection("ElasticSearchOptions:ConnectionString:HostUrl").Value;
            string username = _configuration.GetSection("ElasticSearchOptions:ConnectionString:Username").Value;
            string password = _configuration.GetSection("ElasticSearchOptions:ConnectionString:Password").Value;
            var settings = new ConnectionSettings(new Uri(host));
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                settings.BasicAuthentication(username, password);
            return new ElasticClient(settings);
        }


        public async Task CheckIndex(string indexName)
        {
            var any = await _client.Indices.ExistsAsync(indexName);
            if (any.Exists)
                return;
            var response = await _client.Indices.CreateAsync(indexName, c => c.Index(indexName).ProductMapping().Settings(s => s.NumberOfShards(3).NumberOfReplicas(1)));
            return;
        }

        public async Task InsertDocument(string indexName, Product product)
        {
            var response = await _client.CreateAsync(product, p => p.Index(indexName));
            if (response.ApiCall?.HttpStatusCode == 409)
            {
                await _client.UpdateAsync<Product>(response.Id, p => p.Index(indexName).Doc(product));
            }
        }

        public async Task InsertDocuments(string indexName, List<Product> products)
        {
            await _client.IndexManyAsync(products, index: indexName);
        }

        public async Task<Product> GetDocument(string indexName, int id)
        {
            var response = await _client.GetAsync<Product>(id, p => p.Index(indexName));
            return response.Source;
        }

        public async Task<List<Product>> GetDocuments(string indexName)
        {
            var response = await _client.SearchAsync<Product>(p => p.Index(indexName).Scroll("5m"));
            return response.Documents.ToList();
        }
    }
}
