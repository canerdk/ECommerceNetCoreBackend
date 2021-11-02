using Business.Abstract;
using Core.Entities.Concrete;
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
        IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("getproductsbypagination")]
        public IActionResult GetProductsWithPagination(int parentId, int pageNumber, int pageSize)
        {
            var result = _productService.GetAllWithPaged(parentId, pageNumber, pageSize);
            return Ok(result);
        }


        [HttpGet("getproductsbysearch")]
        public IActionResult GetProductsWithSearch(int pageNumber, string filter)
        {
            var result = _productService.GetAllWithSearch(pageNumber, filter);
            return Ok(result);
        }


        [HttpPost("getproductswithfilter")]
        public IActionResult GetProductsWithFilter([FromBody]FilterQuery filter)
        {
            var result = _productService.GetAllWithFilter(filter);
            return Ok(result);
        }

        [HttpPost("productscheck")]
        public IActionResult ProductsCheck([FromBody]List<OrderCheck> orderChecks)
        {
            var result = _productService.StockAndPriceControl(orderChecks);
            return Ok(result);
        }
    }
}
