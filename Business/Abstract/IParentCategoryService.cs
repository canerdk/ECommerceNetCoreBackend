using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IParentCategoryService
    {
        List<ParentCategory> GetAll();
        List<ParentCategory> GetParentsWithCategory(int categoryId);
        ParentCategory GetById(int categoryId);
    }
}
