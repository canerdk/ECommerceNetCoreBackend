using Business.Abstract;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Business.Concrete
{
    public class ProductManager : IProductService
    {
        IProductDal _productDal;
        public ProductManager(IProductDal productDal)
        {
            _productDal = productDal;
        }

        public List<Product> GetAll()
        {
            return _productDal.GetAll();
        }


        public PagedResponse<Product> GetAllWithFilter(FilterQuery filter)
        {
            return _productDal.GetAllWithFilter(filter);
        }

        public PagedResponse<Product> GetAllWithPaged(int parentId, int pageNumber, int pageSize)
        {
            return _productDal.PaginationQuery(pageNumber, pageSize, parentId);
        }


        public PagedResponse<Product> GetAllWithSearch(int pageNumber, string filter)
        {
            return _productDal.SearchQuery(pageNumber, filter);
        }

        public Product GetById(int productId)
        {
            throw new NotImplementedException();
        }

    }
}
