using Core.Entities;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTOs
{
    public class CategoryWithParentsDto : IDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ParentCategory> ParentCategories { get; set; }
    }
}
