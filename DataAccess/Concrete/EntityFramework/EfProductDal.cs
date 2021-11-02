using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfProductDal : EfEntityRepositoryBase<Product, ECommerceContext>, IProductDal
    {
        public PagedResponse<Product> GetAllWithFilter(FilterQuery filter)
        {
            using (ECommerceContext context = new ECommerceContext())
            {
                PagedResponse<Product> response = new PagedResponse<Product>();
                int count = context.Products.AsQueryable().Where(p => p.ParentCategoryId == filter.ParentCategoryId && (filter.Prices.Count() >= 1 ? p.UnitPrice >= Convert.ToDecimal(filter.Prices[0]) && p.UnitPrice <= Convert.ToDecimal(filter.Prices[1]) : true) && (filter.Colors.Count() >= 1 ? filter.Colors.Any(c => p.ColorId == c) : true)).OrderBy(o => o.Id).Count();
                response.Data = context.Products.AsQueryable().Where(p => p.ParentCategoryId == filter.ParentCategoryId && (filter.Prices.Count() >= 1 ? p.UnitPrice >= Convert.ToDecimal(filter.Prices[0]) && p.UnitPrice <= Convert.ToDecimal(filter.Prices[1]) : true) && (filter.Colors.Count() >= 1 ? filter.Colors.Any(c => p.ColorId == c) : true)).OrderBy(o => o.Id).Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();                
                response.PageNumber = filter.PageNumber;
                response.PageSize = filter.PageSize;
                response.TotalPages = (int)Math.Ceiling(count / (double)filter.PageSize);
                response.TotalRecords = count;
                return response;
            }
        }

        public PagedResponse<Product> PaginationQuery(int pageNumber, int pageSize, int parentCategoryId)
        {
            using (ECommerceContext context = new ECommerceContext())
            {
                PagedResponse<Product> response = new PagedResponse<Product>();
                int count = context.Products.AsQueryable().Where(p => p.ParentCategoryId == parentCategoryId).Count();
                response.Data = context.Products.AsQueryable().Where(p => p.ParentCategoryId == parentCategoryId).OrderBy(o => o.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                response.PageNumber = pageNumber;
                response.PageSize = pageSize;
                response.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                response.TotalRecords = count;
                return response;
            }
        }


        public PagedResponse<Product> SearchQuery(int pageNumber, string filter)
        {
            using (ECommerceContext context = new ECommerceContext())
            {
                PagedResponse<Product> response = new PagedResponse<Product>();
                int count = 5;
                var result = context.Products.AsQueryable().Where(p => p.Name.ToLower().Contains(filter)).OrderBy(o => o.Id).Skip((pageNumber - 1) * 50).Take(50).ToList();
                response.Data = result;
                response.PageNumber = pageNumber;
                response.PageSize = 50;
                response.TotalPages = (int)Math.Ceiling(count / (double)50);
                response.TotalRecords = count;
                return response;
            }
        }
    }
}
