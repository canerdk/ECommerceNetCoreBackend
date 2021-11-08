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
        public async Task<IActionResult> AddProducts(List<Product> products)
        {
            await _elasticSearchService.InsertDocuments("product", products);
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
        public IActionResult ProductsCheck([FromBody]List<OrderCheck> orderChecks)
        {
            var result = _productService.StockAndPriceControl(orderChecks);
            return Ok(result);
        }
    }
}
