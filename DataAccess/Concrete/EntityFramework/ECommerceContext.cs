using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Concrete.EntityFramework
{
    public class ECommerceContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server=127.0.0.1;Database=ECommerce;User Id=postgres;Password=g;");
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ParentCategory> ParentCategories { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<CategoryProduct> CategoriesProducts { get; set; }
    }
}
