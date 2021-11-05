using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfProductDal : EfEntityRepositoryBase<Product, ECommerceContext>, IProductDal
    {
        public async Task<Filter> GetProductsFilter()
        {
            using (ECommerceContext context = new ECommerceContext())
            {
                Filter filter = new Filter();
                var productsColorAndCategory = await context.Products.GroupBy(g => new { g.ColorId, g.ParentCategoryId }).Select(s => new { colors = s.Key.ColorId, categories = s.Key.ParentCategoryId }).ToListAsync();
                var productsPrices = await context.Products.Select(x => x.UnitPrice).Distinct().ToListAsync();
                var colorId = productsColorAndCategory.Select(c => int.Parse(c.colors)).Distinct().ToList();
                var categoryId = productsColorAndCategory.Select(c => c.categories).Distinct().ToList();
                var productsColor = await context.Colors.Where(c => colorId.Any(x => c.Id == x)).ToListAsync();
                var productsCategory = await context.ParentCategories.Where(c => categoryId.Any(x => c.Id == x)).ToListAsync();
                filter.Colors = productsColor;
                filter.ParentCategories = productsCategory;
                filter.MinPrice = productsPrices.Min();
                filter.MaxPrice = productsPrices.Max();
                return filter;
            }
        }

        public async Task<ProductResponse> GetProductsWithPagination(int pageNumber, int pageSize)
        {
            using (ECommerceContext context = new ECommerceContext())
            {
                ProductResponse productResponse = new ProductResponse();
                Pagination pagination = new Pagination();
                int count = await context.Products.CountAsync();
                var products = await context.Products.OrderByDescending(o => o.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
                pagination.PageNumber = pageNumber;
                pagination.PageSize = pageSize;
                pagination.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                pagination.TotalRecords = count;
                productResponse.Products = products;
                productResponse.Pagination = pagination;

                return productResponse;
            }
        }



        //public PagedResponse<Product> GetAllProducts(int pageNumber, int pageSize)
        //{            
        //    using (ECommerceContext context = new ECommerceContext())
        //    {
        //        var watch = System.Diagnostics.Stopwatch.StartNew();

        //        //var result = context.Products.Select(p => p.ColorId).Distinct().ToList();
        //        //var result = from p in context.Products
        //        //             group p by p.ColorId into colorGroup
        //        //             orderby colorGroup.Key
        //        //             select colorGroup;
        //        var result = (from p in context.Products
        //                                 join c in context.ParentCategories on p.ParentCategoryId equals c.Id
        //                                 select c).Distinct().ToList();
        //        watch.Stop();
        //        var elapsedMs = watch.ElapsedMilliseconds;


        //        PagedResponse<Product> response = new PagedResponse<Product>();
        //        List<Color> colors = new List<Color>();
        //        var color = context.Products.AsQueryable().Select(c => int.Parse(c.ColorId)).Distinct().ToList();
        //        colors = context.Colors.AsQueryable().Where(c => (color.Count() > 0 ? color.Any(x => c.Id == x) : false)).ToList();
        //        var prices = context.Products.AsQueryable().Select(x => x.UnitPrice).Distinct().ToList();
        //        var parentCategories = context.Products.AsQueryable().Select(x => x.ParentCategoryId).Distinct().ToList();
        //        var categories = context.ParentCategories.AsQueryable().Where(c => (parentCategories.Count() > 0 ? parentCategories.Any(x => c.Id == x) : false)).ToList();
        //        response.ParentCategories = parentCategoryDtos;
        //        response.Colors = colorDtos;
        //        int count = context.Products.Count();
        //        response.MinPrice = prices.Min();
        //        response.MaxPrice = prices.Max();
        //        response.Data = context.Products.AsQueryable().OrderBy(o => o.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        //        response.PageNumber = pageNumber;
        //        response.PageSize = pageSize;
        //        response.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        //        response.TotalRecords = count;

        //        return response;                
        //    }

        //}

        //public PagedResponse<Product> GetAllWithFilter(FilterQuery filter)
        //{
        //    using (ECommerceContext context = new ECommerceContext())
        //    {
        //        PagedResponse<Product> response = new PagedResponse<Product>();
        //        int count = context.Products.AsQueryable().Where(p => p.ParentCategoryId == filter.ParentCategoryId && (filter.Prices.Count() >= 1 ? p.UnitPrice >= Convert.ToDecimal(filter.Prices[0]) && p.UnitPrice <= Convert.ToDecimal(filter.Prices[1]) : true) && (filter.Colors.Count() >= 1 ? filter.Colors.Any(c => p.ColorId == c) : true)).OrderBy(o => o.Id).Count();
        //        response.Data = context.Products.AsQueryable().Where(p => p.ParentCategoryId == filter.ParentCategoryId && (filter.Prices.Count() >= 1 ? p.UnitPrice >= Convert.ToDecimal(filter.Prices[0]) && p.UnitPrice <= Convert.ToDecimal(filter.Prices[1]) : true) && (filter.Colors.Count() >= 1 ? filter.Colors.Any(c => p.ColorId == c) : true)).OrderBy(o => o.Id).Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();                
        //        response.PageNumber = filter.PageNumber;
        //        response.PageSize = filter.PageSize;
        //        response.TotalPages = (int)Math.Ceiling(count / (double)filter.PageSize);
        //        response.TotalRecords = count;
        //        return response;
        //    }
        //}

        //public PagedResponse<Product> PaginationQuery(int pageNumber, int pageSize, int parentCategoryId)
        //{
        //    using (ECommerceContext context = new ECommerceContext())
        //    {
        //        PagedResponse<Product> response = new PagedResponse<Product>();
        //        int count = context.Products.AsQueryable().Where(p => p.ParentCategoryId == parentCategoryId).Count();
        //        response.Data = context.Products.AsQueryable().Where(p => p.ParentCategoryId == parentCategoryId).OrderBy(o => o.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        //        response.PageNumber = pageNumber;
        //        response.PageSize = pageSize;
        //        response.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        //        response.TotalRecords = count;
        //        return response;
        //    }
        //}


        //public PagedResponse<Product> SearchQuery(int pageNumber, string filter)
        //{
        //    using (ECommerceContext context = new ECommerceContext())
        //    {
        //        PagedResponse<Product> response = new PagedResponse<Product>();
        //        int count = 5;
        //        var result = context.Products.AsQueryable().Where(p => p.Name.ToLower().Contains(filter)).OrderBy(o => o.Id).Skip((pageNumber - 1) * 50).Take(50).ToList();
        //        response.Data = result;
        //        response.PageNumber = pageNumber;
        //        response.PageSize = 50;
        //        response.TotalPages = (int)Math.Ceiling(count / (double)50);
        //        response.TotalRecords = count;
        //        return response;
        //    }
        //}
    }
}
