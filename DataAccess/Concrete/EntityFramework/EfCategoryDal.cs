using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfCategoryDal : EfEntityRepositoryBase<Category, ECommerceContext>, ICategoryDal
    {
        //public List<CategoryWithParentsDto> GetAllCategoriesWithParents()
        //{
        //    using (ECommerceContext context = new ECommerceContext())
        //    {
        //        var result = (from c in context.Categories
        //                      join p in context.ParentCategories
        //                      on c.Id equals p.CategoryId into grp
        //                      select new CategoryWithParentsDto
        //                      {
        //                          Id = c.Id,
        //                          Name = c.Name,
        //                          ParentCategories = grp.ToList()
        //                      });
        //        return result.ToList();
        //    }
        //}
    }
}
