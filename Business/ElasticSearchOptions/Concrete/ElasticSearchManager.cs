using Business.Abstract;
using Business.ElasticSearchOptions.Abstract;
using Business.ElasticSearchOptions.Dtos;
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
        private readonly IColorService _color;
        private readonly IParentCategoryService _category;

        public ElasticSearchManager(IConfiguration configuration, IColorService color, IParentCategoryService category)
        {
            _configuration = configuration;
            _client = CreateInstance();
            _color = color;
            _category = category;
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
            var response = await _client.Indices.CreateAsync(indexName, c => c.Index(indexName).ProductMapping());
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
            List<ProductElasticIndexDto> productElasticIndexDtos = new List<ProductElasticIndexDto>();
            ProductElasticIndexDto productElasticIndexDto = new ProductElasticIndexDto();
            foreach (var item in products)
            {
                productElasticIndexDto = new ProductElasticIndexDto()
                {
                    Id = item.Id,
                    Code = item.Code,
                    Name = item.Name,
                    Color = _color.GetById(Convert.ToInt32(item.ColorId)).Name,
                    UnitPrice = item.UnitPrice,
                    UnitsInStock = item.UnitsInStock,
                    Category = _category.GetById(item.Id).Name
                };
                productElasticIndexDtos.Add(productElasticIndexDto);
            }
            await _client.IndexManyAsync(productElasticIndexDtos, index: indexName);
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

        public async Task DeleteIndex(string indexName, int productId)
        {
            await _client.DeleteAsync(new DeleteRequest(indexName, productId));
            //await _client.Indices.DeleteAsync(indexName);
        }

        public async Task AddOrUpdate(string indexName, Product product)
        {
            //gönderilen ürün mevcut indexlerde var mı ona bakılacak
            var exist = _client.DocumentExists(DocumentPath<Product>.Id(product), p => p.Index(indexName));
            //varsa olanı güncelle
            if (exist.Exists)
            {
                var result = await _client.UpdateAsync(DocumentPath<Product>.Id(product), p => p.Index(indexName).Doc(product).RetryOnConflict(3));
            }
        }

        public Task<List<Product>> SearchAsync(string searchText, int skipItemCount, int maxItemCount)
        {
            List<Product> products = new List<Product>();
            var searchQuery = new SearchDescriptor<Product>();
            //searchQuery = new SearchDescriptor<Product>().Query(q => q
            //.MultiMatch(m => m.Fields(f => f.Field(ff => ff.Name, 3.0)
            //                                .Field(ff => ff.Code, 2.0)
            //                                .Field(ff => ff.ColorId, 1.5))
            //.Query(searchText).Type(TextQueryType.BestFields).Operator(Operator.Or).MinimumShouldMatch(3))).Sort(s => s.Descending(f => f.Id));
            searchQuery = new SearchDescriptor<Product>().Query(q => q
            .MatchPhrasePrefix(m => m.Field(f => f.Name).Query(searchText)) || q.MatchPhrasePrefix(m => m.Field(f => f.ColorId).Query(searchText)));

            searchQuery.Index("product");
            searchQuery.Skip(skipItemCount).Take(maxItemCount);

             var result = _client.Search<Product>(searchQuery);

            return Task.FromResult(result.Documents.ToList());
        }
    }
}
