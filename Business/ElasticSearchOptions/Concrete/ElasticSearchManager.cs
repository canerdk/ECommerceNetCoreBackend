using Business.Abstract;
using Business.ElasticSearchOptions.Abstract;
using Business.ElasticSearchOptions.Dtos;
using Elasticsearch.Net;
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
        private readonly ILanguageService _languageService;

        public ElasticSearchManager(IConfiguration configuration, IColorService color, IParentCategoryService category, ILanguageService languageService)
        {
            _configuration = configuration;
            _client = CreateInstance();
            _color = color;
            _category = category;
            _languageService = languageService;
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

        public async Task InsertDocuments(List<Product> products)
        {
            List<ProductElasticIndexDto> productElasticIndexDtos = new List<ProductElasticIndexDto>();
            ProductElasticIndexDto productElasticIndexDto = new ProductElasticIndexDto();
            var color = _color.GetAll();
            string indexName = "";
            var language = _languageService.GetAll();

            foreach (var product in products.GroupBy(g => g.LanguageId))
            {

                var p = products.Where(p => p.LanguageId == product.Key).ToList();
                indexName = language.FirstOrDefault(x => x.Id == product.Key).Code.ToLower().Replace(" ", "");
                await CreateIndexAsync(indexName + "index", indexName);
                foreach (var item in p)
                {
                    Random gen = new Random();
                    DateTime startDate = new DateTime(2019, 1, 1);
                    int range = (DateTime.Today - startDate).Days;

                    productElasticIndexDto = new ProductElasticIndexDto()
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                        Color = color.FirstOrDefault(c => c.Id == item.ColorId).Name,
                        UnitPrice = item.UnitPrice,
                        UnitsInStock = item.UnitsInStock,
                        Language = language.FirstOrDefault(c => c.Id == item.LanguageId).Name,
                        AddedDate = startDate.AddDays(gen.Next(range))
                    };
                    productElasticIndexDtos.Add(productElasticIndexDto);

                }
                await _client.IndexManyAsync(productElasticIndexDtos, index: indexName);
                productElasticIndexDtos.Clear();
            }
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
            //await _client.DeleteAsync(new DeleteRequest(indexName, productId));
            await _client.Indices.DeleteAsync(indexName);
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

        public async Task<List<ProductElasticIndexDto>> SearchAsync(string searchText, Entities.Concrete.Filter filter, string indexName, int skipItemCount, int maxItemCount)
        {
            searchText = searchText ?? string.Empty;
            string[] splittedText = searchText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var searchQuery = new SearchDescriptor<ProductElasticIndexDto>();
            if (filter.MinPrice > 0 || filter.MaxPrice > 0)
            {
                searchQuery = new SearchDescriptor<ProductElasticIndexDto>().Query(q => q
                .MultiMatch(m => m.Fields(f => f.Field(ff => ff.Name, 8.0)
                                            .Field(ff => ff.Code, 4.0)
                                            .Field(ff => ff.Color, 2.0))
                .Query(searchText).Type(TextQueryType.CrossFields).Operator(Operator.Or).MinimumShouldMatch(splittedText.Length)) && q.Range(r => r.Field(rf => rf.UnitPrice).GreaterThanOrEquals((double?)filter.MinPrice)) && q.Range(r => r.Field(rf => rf.UnitPrice).LessThanOrEquals((double?)filter.MaxPrice)));
            }
            else if (filter.MinDate != null || filter.MaxDate != null)
            {
                searchQuery = new SearchDescriptor<ProductElasticIndexDto>().Query(q => q
                .MultiMatch(m => m.Fields(f => f.Field(ff => ff.Name, 8.0)
                                            .Field(ff => ff.Code, 4.0)
                                            .Field(ff => ff.Color, 2.0))
                .Query(searchText).Type(TextQueryType.CrossFields).Operator(Operator.Or).MinimumShouldMatch(splittedText.Length)) && q.DateRange(r => r.Field(rf => rf.AddedDate).GreaterThanOrEquals(filter.MinDate)) && q.DateRange(r => r.Field(rf => rf.AddedDate).LessThanOrEquals(filter.MaxDate))).Sort(s => s.Descending(a => a.AddedDate));
            }
            else
            {
                searchQuery = new SearchDescriptor<ProductElasticIndexDto>().Query(q => q
                .MultiMatch(m => m.Fields(f => f.Field(ff => ff.Name, 8.0)
                                            .Field(ff => ff.Code, 4.0)
                                            .Field(ff => ff.Color, 2.0))
                .Query(searchText).Type(TextQueryType.CrossFields).Operator(Operator.Or).MinimumShouldMatch(splittedText.Length)));
            }
            MultiMatchQueryDescriptor<ProductElasticIndexDto> multiMatchQueryDescriptor = new MultiMatchQueryDescriptor<ProductElasticIndexDto>();

            searchQuery.Index(indexName);
            searchQuery.Skip(skipItemCount).Take(maxItemCount);

            var searchResponse = await _client.SearchAsync<ProductElasticIndexDto>(searchQuery);

            return searchResponse.Documents.ToList();
        }


        private async Task CreateIndexAsync(string indexName, string aliasName)
        {
            var exist = await _client.Indices.ExistsAsync(indexName);
            if (exist.Exists)
                return;

            var result = await _client.Indices.CreateAsync(indexName, ss => ss.Index(indexName).Settings(o => o.NumberOfShards(1).NumberOfReplicas(1).Setting("max_result_window", int.MaxValue).Analysis(a => a.TokenFilters(t => t.AsciiFolding("my_ascii_folding", af => af.PreserveOriginal(true)))
            .Analyzers(aa => aa.Custom("turkish_analyzer", ca => ca.Filters("lowercase", "my_ascii_folding").Tokenizer("standard")))))
            .ProductMapping());


            if (result.Acknowledged)
            {
                await _client.Indices.PutAliasAsync(new PutAliasRequest(indexName, aliasName));
                return;
            }
            var alias = await _client.Indices.GetAliasAsync(indexName);
            throw new ElasticsearchClientException($"Insert Document failed at index {indexName} :" + result.ServerError.Error.Reason);
        }
    }
}
