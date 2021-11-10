using Business.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Concrete
{
    public class ParentCategoryManager : IParentCategoryService
    {
        IParentCategoryDal _parentCategoryDal;
        public ParentCategoryManager(IParentCategoryDal parentCategoryDal)
        {
            _parentCategoryDal = parentCategoryDal;
        }

        public List<ParentCategory> GetAll()
        {
            return _parentCategoryDal.GetAll();
        }

        public ParentCategory GetById(int categoryId)
        {
            return _parentCategoryDal.Get(c => c.Id == categoryId);
        }

        public List<ParentCategory> GetParentsWithCategory(int categoryId)
        {
            return _parentCategoryDal.GetAll(x => x.CategoryId == categoryId);
        }
    }
}
