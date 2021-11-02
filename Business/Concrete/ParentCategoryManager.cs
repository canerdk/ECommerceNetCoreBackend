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
            throw new NotImplementedException();
        }

        public List<ParentCategory> GetParentsWithCategory(int categoryId)
        {
            return _parentCategoryDal.GetAll(x => x.CategoryId == categoryId);
        }
    }
}
