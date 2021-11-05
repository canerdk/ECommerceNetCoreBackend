
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IProductService
    {
        List<Product> GetAll();
        //PagedResponse<Product> GetAllProducts(int pageNumber, int pageSize);
        Product GetById(int productId);
        //PagedResponse<Product> GetAllWithPaged(int parentId, int pageNumber, int pageSize);
        //PagedResponse<Product> GetAllWithSearch(int pageNumber, string filter);
        //PagedResponse<Product> GetAllWithFilter(FilterQuery filter);
        List<OrderCheck> StockAndPriceControl(List<OrderCheck> orderChecks);
        Task<Filter> GetProductsFilter();
        Task<ProductResponse> GetProductsWithPagination(int pageNumber, int pageSize);
    }
}
