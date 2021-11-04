using Business.Abstract;
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

        public Filter GetProductsFilter()
        {
            return _productDal.GetProductsFilter();
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
