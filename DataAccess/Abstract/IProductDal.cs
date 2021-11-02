using Core.DataAccess;
using Core.Entities.Concrete;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Abstract
{
    public interface IProductDal : IEntityRepository<Product>
    {
        PagedResponse<Product> PaginationQuery(int pageNumber, int pageSize, int parentCategoryId);
        PagedResponse<Product> SearchQuery(int pageNumber, string filter);
        PagedResponse<Product> GetAllWithFilter(FilterQuery filter);
    }
}
