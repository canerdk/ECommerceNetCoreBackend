using Core.DataAccess;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstract
{
    public interface IProductDal : IEntityRepository<Product>
    {
        Task<Filter> GetProductsFilter();
        Task<ProductResponse> GetProductsWithPagination(int pageNumber, int pageSize);
        //PagedResponse<Product> GetAllProducts(int pageNumber, int pageSize);
        //PagedResponse<Product> PaginationQuery(int pageNumber, int pageSize, int parentCategoryId);
        //PagedResponse<Product> SearchQuery(int pageNumber, string filter);
        //PagedResponse<Product> GetAllWithFilter(FilterQuery filter);
    }
}
