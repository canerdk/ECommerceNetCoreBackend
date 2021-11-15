using AutoMapper;
using Business.Abstract;
using Business.ElasticSearchOptions.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IElasticSearchService _elasticSearchService;

        public ProductController(IProductService productService, IElasticSearchService elasticSearchService)
        {
            _productService = productService;
            _elasticSearchService = elasticSearchService;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize)
        {
            return Ok(await _productService.GetProductsWithPagination(pageNumber, pageSize));
        }

        [HttpGet("getproductsfilter")]
        public async Task<IActionResult> GetProductsFilter()
        {
            return Ok(await _productService.GetProductsFilter());
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddProducts(string indexName)
        {
            var products = _productService.GetAll().OrderBy(o => o.Id).ToList();
            await _elasticSearchService.InsertDocuments(indexName, products.ToList());

            return Ok("asd");
        }

        [HttpPost("addorupdate")]
        public async Task<IActionResult> AddProducts(string indexName, int productId)
        {
            Product product = _productService.GetById(productId);
            product.UnitPrice = 450;
            await _elasticSearchService.AddOrUpdate(indexName, product);

            return Ok("asd");
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteIndex(string indexName, int productId)
        {
            await _elasticSearchService.DeleteIndex(indexName, productId);
            return Ok();
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search(string searchText, [FromBody]Filter filter, string indexName, int skip, int count)
        {
            var result = await _elasticSearchService.SearchAsync(searchText, filter, indexName, skip, count);
            return Ok(result);
        }

        [HttpPost("createindex")]
        public async Task<IActionResult> CreateIndexAsyncs(string index, string alias)
        {
            await _elasticSearchService.CreateIndexAsync(index, alias);
            return Ok();
        }


        //[HttpGet("getproductsbypagination")]
        //public IActionResult GetProductsWithPagination(int parentId, int pageNumber, int pageSize)
        //{
        //    var result = _productService.GetAllWithPaged(parentId, pageNumber, pageSize);
        //    return Ok(result);
        //}


        //[HttpGet("getproductsbysearch")]
        //public IActionResult GetProductsWithSearch(int pageNumber, string filter)
        //{
        //    var result = _productService.GetAllWithSearch(pageNumber, filter);
        //    return Ok(result);
        //}


        //[HttpPost("getproductswithfilter")]
        //public IActionResult GetProductsWithFilter([FromBody]FilterQuery filter)
        //{
        //    var result = _productService.GetAllWithFilter(filter);
        //    return Ok(result);
        //}

        [HttpPost("productscheck")]
        public IActionResult ProductsCheck([FromBody] List<OrderCheck> orderChecks)
        {
            var result = _productService.StockAndPriceControl(orderChecks);
            return Ok(result);
        }
    }
}
