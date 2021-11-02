using Business.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Concrete
{
    public class CategoryManager : ICategoryService
    {
        ICategoryDal _categoryDal;
        IParentCategoryService _parentCategoryService;

        public CategoryManager(ICategoryDal categoryDal, IParentCategoryService parentCategoryService)
        {
            _categoryDal = categoryDal;
            _parentCategoryService = parentCategoryService;
        }

        public List<Category> GetAll()
        {
            return _categoryDal.GetAll();
        }

        public List<CategoryWithParentsDto> GetAllCategoriesWithParents()
        {
            List<CategoryWithParentsDto> categoryWithParents = new List<CategoryWithParentsDto>();
            List<Category> categories = _categoryDal.GetAll();
            foreach (var category in categories)
            {
                CategoryWithParentsDto categoryWithProductsDto = new CategoryWithParentsDto()
                {
                    Id = category.Id,
                    Name = category.Name,
                    ParentCategories = GetParentCategories(category.Id)
                };
                categoryWithParents.Add(categoryWithProductsDto);
            }
            return categoryWithParents;
        }

        public Category GetById(int categoryId)
        {
            return _categoryDal.Get(x => x.Id == categoryId);
        }

        private List<ParentCategory> GetParentCategories(int categoryId)
        {
            return _parentCategoryService.GetParentsWithCategory(categoryId);
        }
    }
}
