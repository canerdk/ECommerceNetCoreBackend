using Business.Abstract;
using Business.ElasticSearchOptions.Abstract;
using Core.Aspects.Autofac.Caching;
using DataAccess.Abstract;
using Entities.Concrete;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class ProductManager : IProductService
    {
        IProductDal _productDal;
        private readonly IElasticSearchService _elasticSearchService;
        public ProductManager(IProductDal productDal, IElasticSearchService elasticSearchService)
        {
            _productDal = productDal;
            _elasticSearchService = elasticSearchService;
        }

        public List<Product> GetAll()
        {
            return _productDal.GetAll();
        }

        //public PagedResponse<Product> GetAllProducts(int pageNumber, int pageSize)
        //{
        //    return _productDal.GetAllProducts(pageNumber, pageSize);
        //}

        //public PagedResponse<Product> GetAllWithFilter(FilterQuery filter)
        //{
        //    return _productDal.GetAllWithFilter(filter);
        //}

        //public PagedResponse<Product> GetAllWithPaged(int parentId, int pageNumber, int pageSize)
        //{
        //    return _productDal.PaginationQuery(pageNumber, pageSize, parentId);
        //}


        //public PagedResponse<Product> GetAllWithSearch(int pageNumber, string filter)
        //{
        //    return _productDal.SearchQuery(pageNumber, filter);
        //}

        public Product GetById(int productId)
        {
            throw new NotImplementedException();
        }

        [CacheAspect]
        public async Task<Entities.Concrete.Filter> GetProductsFilter()
        {
            return await _productDal.GetProductsFilter();
        }

        public async Task<ProductResponse> GetProductsWithPagination(int pageNumber, int pageSize)
        {
            return await _productDal.GetProductsWithPagination(pageNumber, pageSize);
        }

        public List<OrderCheck> StockAndPriceControl(List<OrderCheck> orderChecks)
        {
            List<OrderCheck> OrderChecks = new List<OrderCheck>();            
            foreach (var item in orderChecks)
            {
                var result = _productDal.Get(p => p.Id == item.Product.Id);
                if (result.UnitsInStock != item.Product.UnitsInStock)
                {
                    item.Product.UnitsInStock = result.UnitsInStock;
                }
                OrderCheck order = new OrderCheck
                {
                    Product = item.Product,
                    Quantity = item.Quantity
                };
                OrderChecks.Add(order);
            }
            return OrderChecks;
        }

    }
}
