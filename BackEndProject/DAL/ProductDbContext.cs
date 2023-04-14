using BackEndProject.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BackEndProject.ViewModel;

namespace BackEndProject.DAL
{
    public class ProductDbContext : IdentityDbContext<User>
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Color> Colors { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<GlobalTab> GlobalTabs { get; set; }
        public DbSet<Instruction> Instructions { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductSizeColor> ProductSizeColors { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Basket> Baskets { get; set; }

        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Order> Orders { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var item in modelBuilder.Model
                               .GetEntityTypes()
                                       .SelectMany(e => e.GetProperties()
                                                   .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?))))
            {
                item.SetColumnType("decimal(6,2)");
            }

            modelBuilder.Entity<Collection>().
                    HasIndex(s => s.Name).
                    IsUnique();
            modelBuilder.Entity<Tag>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<Color>().HasIndex(t => t.Name).IsUnique();
            modelBuilder.Entity<Size>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<Setting>().HasIndex(s => s.Key).IsUnique();
            base.OnModelCreating(modelBuilder);
        }




        public DbSet<BackEndProject.ViewModel.ProductVM>? ProductVM { get; set; }
    }
}
